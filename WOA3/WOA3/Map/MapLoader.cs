using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GWNorthEngine.AI.AStar;
using GWNorthEngine.AI.AStar.Params;
using GWNorthEngine.Tools;
using GWNorthEngine.Tools.TilePlacer;
using loader = GWNorthEngine.Tools.TilePlacer.MapLoader;

using WOA3.Model;
using WOA3.Map;
using WOA3.Logic;
using WOA3.Logic.AI;


namespace WOA3.Engine {
	public class MapLoader {

		private static object loadObject<T>(XmlDocument doc, string header, string[] searchStrings) {
			return loadObject<T>(doc, header, searchStrings, 0);
		}

		private static object loadObject<T>(XmlDocument doc, string header, string[] searchStrings, int nodeDepth) {
			T[] result = null;
			if (doc.GetElementsByTagName(header) != null && doc.GetElementsByTagName(header)[nodeDepth] != null) {
				XmlNodeList nodes = doc.GetElementsByTagName(header)[nodeDepth].ChildNodes;
				XmlNode node;
				Type type = typeof(T);
				MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
				for (int i = 0; i < nodes.Count; i++) {
					node = nodes[i];
					for (int j = 0; j < searchStrings.Length; j++) {
						if (node.Name == searchStrings[j]) {
							if (type == typeof(string)) {
								if (node.FirstChild != null && node.FirstChild.Value != null) {// we may just want the default
									if (result == null) {
										result = new T[searchStrings.Length];
									}
									result[j] = (T)((object)node.FirstChild.Value);
								}
							} else {
								if (node.FirstChild != null && node.FirstChild.Value != null) {// we may just want the default
									if (result == null) {
										result = new T[searchStrings.Length];
									}
									result[j] = (T)parseMethod.Invoke(type, new object[] { ((string)node.FirstChild.Value) });
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static object loadObject<T>(XmlDocument doc, MapEditor.MappingState header) {
			return loadObject<T>(doc, header.ToString(), new string[] { MapEditor.XML_X, MapEditor.XML_Y });
		}

		public static void loadMap(ContentManager content, string mapName, out Model.Map map) {
			LoadResult loadResult = loader.load(content, mapName);
			int height = loadResult.Height;
			int width = loadResult.Width;
			int wallLayerLocation = 1;

			// generate walls
			Layer wallLayer = loadResult.Layers[wallLayerLocation];
			MapTile[,] layerTiles = wallLayer.Tiles;
			List<Wall> walls = new List<Wall>();
			BasePathFinder.TypeOfSpace[,] aiSpaceTypes = new BasePathFinder.TypeOfSpace[height, width];
			foreach (var mapTile in layerTiles) {
				if (mapTile != null) {
					Vector2 newPosition = Vector2.Add(mapTile.WorldPosition, new Vector2(Constants.TILE_SIZE) /2);
					BoundingBox boundingBox = CollisionGenerationUtils.generateBoundingBoxesForTexture(mapTile.Texture, newPosition);
					walls.Add(new Wall(content, newPosition, mapTile.Texture, boundingBox));
					aiSpaceTypes[mapTile.Index.Y, mapTile.Index.X] = BasePathFinder.TypeOfSpace.Unwalkable;
				}
			}

			// load the AI for the map
			BasePathFinder.TypeOfSpace defaultSpaceType = (BasePathFinder.TypeOfSpace) Activator.CreateInstance(typeof(BasePathFinder.TypeOfSpace));
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (aiSpaceTypes[y, x] == defaultSpaceType) {
						aiSpaceTypes[y, x] = BasePathFinder.TypeOfSpace.Walkable;
					}
				}
			}

			AIManager.getInstance().init(height, width);
			AIManager.getInstance().Board = aiSpaceTypes;

			map = new Model.Map(walls);
		}
		
		public static void loadGenericPointList(XmlDocument doc, MapEditor.MappingState header, out List<Point> positions) {
			loadGenericPointList(doc, header.ToString(), out positions);
		}

		public static void loadGenericPointList(XmlDocument doc, string header, out List<Point> positions) {
			XmlNodeList nodes = doc.GetElementsByTagName(header);
			positions = new List<Point>();
			string[] xySearch = new string[] { MapEditor.XML_X, MapEditor.XML_Y };
			int[] intResults;
			for (int i = 0; i < nodes.Count; i++) {
				intResults = (int[])loadObject<int>(doc, header, xySearch, i);
				positions.Add(new Point(intResults[0], intResults[1]));
			}
		}

		public static void loadPlayerInformation(XmlDocument doc, ref Point playerStart) {
			try {
				int[] result = (int[])loadObject<Int32>(doc, MapEditor.MappingState.PlayerStart);
				playerStart = new Point(result[0], result[1]);
			} catch (Exception) {
				// do nothing else with the error as the map is obviously in development stages
			}
		}

		public static void loadSpecializedInformation(XmlDocument doc, MapEditor.MappingState state,
			ref List<SpecializedLoadResult> spikeInfos) {
			try {
				List<Point> points;
				loadGenericPointList(doc, state, out points);
				XmlNodeList nodes = doc.GetElementsByTagName(state.ToString());
				string[] searchStrings = new string[] { MapEditor.XML_TYPE };
				string[] results;
				SpecializedLoadResult loadResult;
				for (int i = 0; i < nodes.Count; i++) {
					results = (string[])loadObject<string>(doc, state.ToString(), searchStrings, i);
					loadResult = new SpecializedLoadResult();
					loadResult.Start = points[i];
					loadResult.Type = results[0];
					spikeInfos.Add(loadResult);
				}
			} catch (Exception) {
				// do nothing else with the error as the map is obviously in development stages
			}
		}

		public static void loadMonsterInformation(XmlDocument doc, ref List<SpecializedLoadResult> monsterInfos) {
			try {
				XmlNodeList nodes = doc.GetElementsByTagName(MapEditor.MappingState.Monster.ToString());
				string[] typeSearchString = new string[] { MapEditor.XML_TYPE};
				string[] typeResults;
				List<Point> points;
				SpecializedLoadResult monsterInfo;
				for (int i = 0; i < nodes.Count; i++) {
					points = null;
					typeResults = (string[])loadObject<string>(doc, MapEditor.MappingState.Monster.ToString(), typeSearchString, i);

					monsterInfo = new SpecializedLoadResult();
					monsterInfo.Type = typeResults[0];

					loadGenericPointList(doc, MapEditor.XML_PATH_START, out points);
					monsterInfo.Start = points[i];

					loadGenericPointList(doc, MapEditor.XML_PATH_END, out points);
					monsterInfo.End = points[i];

					monsterInfos.Add(monsterInfo);
				}
			} catch (Exception) {
				// do nothing else with the error as the map is obviously in development stages
			}
		}
	}
}
