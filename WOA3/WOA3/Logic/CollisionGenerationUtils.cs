using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WOA3.Logic {
	public class CollisionGenerationUtils {

		private static int[] INTERNAL_CORNER_TILES = { 1, 2, 3, 4 };
		private static int[] CORNER_TILES = { 21, 22, 23, 24 };
		private static int[] UP_DOWN_TILES = { 6, 7, 9, 12, 13 };
		private static int[] LEFT_RIGHT_TILES = { 5, 8, 10, 11 , 14};

		public static BoundingBox generateBoundingBoxesForTexture(Texture2D texture, Vector2 position) {
			BoundingBox bbox = getBBox(position);
			if (texture.Name.StartsWith("Tile")) {
				float halfTile = Constants.TILE_SIZE / 2;
				float quarterTile = Constants.TILE_SIZE / 4;
				Vector2 newPosition = Vector2.Subtract(position, new Vector2(halfTile));
				int tileNumber = int.Parse(texture.Name.Replace("Tile", ""));
				if (CORNER_TILES.Contains(tileNumber)) {
					bbox = getBBox(position);
				} else if (UP_DOWN_TILES.Contains(tileNumber)) {
					bbox = new BoundingBox(
						new Vector3(Vector2.Add(newPosition, new Vector2(halfTile, 0f)), 0f),
						new Vector3(Vector2.Add(newPosition, new Vector2(quarterTile, Constants.TILE_SIZE)), 0f));
				} else if (LEFT_RIGHT_TILES.Contains(tileNumber)) {
					bbox = new BoundingBox(
						new Vector3(Vector2.Add(newPosition, new Vector2(0f, halfTile)), 0f),
						new Vector3(Vector2.Add(newPosition, new Vector2(Constants.TILE_SIZE, quarterTile)), 0f));
				} else if (INTERNAL_CORNER_TILES.Contains(tileNumber)) {
					bbox = getBBox(position); ;
				} else {
					throw new Exception("Cannot load BBox for: " + texture.Name);
				}
			}
			return bbox;
		}

		public static BoundingBox getBBox(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE / 2)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE / 2)), 0f));
		}

		public static BoundingBox getBBoxHalf(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE / 4)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE / 4)), 0f));
		}

		public static BoundingBox getBBox64(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE )), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE)), 0f));
		}
	}
}