using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using GWNorthEngine.Scripting;
using GWNorthEngine.Input;

using WOA3.Logic;
using WOA3.Model;

namespace WOA3.Map {
	public class MapEditor {
		public enum MappingState {
			None,
			PlayerStart,
			Spike,
			Fish,
			Crate,
			Monster,
			Goal,
			SpikeLauncher,
			HealthKit
		};
#if DEBUG
		#region Debug class variables
		//singleton instance
		private static MapEditor instance = new MapEditor();

		private EditorCreator objectCreator;
		private EditorDeleter objectDeleter;
		private List<CreatedType> createdObjects;

		private struct CreatedType {
			public MappingState objType;
			public Point position;
			public Point endPosition;
			public string subType;
		}
		#endregion Debug class variables
#endif

		#region Class variables
		private MappingState mappingState;
		private Point endPosition;
		private const string COMMAND_NONE = "none";
		private const string COMMAND_PLAYER_POSITION = "playerposition";
		private const string COMMAND_SPIKE = "spike";
		private const string COMMAND_FISH = "fish";
		private const string COMMAND_GOAL = "goal";
		private const string COMMAND_MONSTER = "monster";
		public const string COMMAND_CRATE = "crate";
		public const string COMMAND_SPIKE_LAUNCHER = "spikelauncher";
		public const string COMMAND_HEALTH_KIT = "healthkit";
		public const string XML_X = "X";
		public const string XML_Y = "Y";
		public const string XML_TYPE = "Type";
		public const string XML_PATH_START = "PathStart";
		public const string XML_PATH_END = "PathEnd";
		public const string MONSTER_TYPE_ZOOM = "Zoom";
		#endregion Class variables
		
#if DEBUG
		#region Class properties
		public string SpikeType { get; set; }
		public string MonsterType { get; set; }
		public string LaunchDirection { get; set; }
		#endregion Class properties

		#region Constructor
		public MapEditor() {
			
		}
		#endregion Constructor

		#region Support methods
		public static MapEditor getInstance() {
			return instance;
		}

		public void init(EditorCreator objectCreator, EditorDeleter objectDeleter) {
			this.objectCreator = objectCreator;
			this.objectDeleter = objectDeleter;
			this.createdObjects = new List<CreatedType>();
		}

		public void editMapHelp() {
			Console.WriteLine("Format: [Information] - [Command]");
			Console.WriteLine("Turn Mapping off - " + COMMAND_NONE);
			Console.WriteLine("Players starting position - " + COMMAND_PLAYER_POSITION);
			Console.WriteLine("Spike (ensure SpikeType is set) - " + COMMAND_SPIKE);
			Console.WriteLine("SpikeType - SpikesBottom | SikesLeft | SpikesTop | SpikesRight | SpikesMidUp | SpikesMidDown");
			Console.WriteLine("Fish - " + COMMAND_FISH);
			Console.WriteLine("Crate - " + COMMAND_CRATE);
			Console.WriteLine("Goal - " + COMMAND_GOAL);
			Console.WriteLine("Health Kit - " + COMMAND_HEALTH_KIT);
			Console.WriteLine("SpikeLauncher - " + COMMAND_SPIKE_LAUNCHER);
			Console.WriteLine("LaunchDirection - Up | Down | Left | Right");
			Console.WriteLine("Monster(Left click starts, Left click + E = end) - " + COMMAND_MONSTER);
			Console.WriteLine("MonsterType - " + MONSTER_TYPE_ZOOM);
			Console.WriteLine("Path Length - Length");
		}

		public void editMap(string value) {
			value = value.ToLower();
			MappingState oldState = this.mappingState;
			switch (value) {
				case COMMAND_NONE:
					this.mappingState = MappingState.None;
					break;
				case COMMAND_PLAYER_POSITION:
					this.mappingState = MappingState.PlayerStart;
					break;
				case COMMAND_FISH:
					this.mappingState = MappingState.Fish;
					break;
				case COMMAND_CRATE:
					this.mappingState = MappingState.Crate;
					break;
				case COMMAND_GOAL:
					this.mappingState = MappingState.Goal;
					break;
				case COMMAND_SPIKE:
					this.mappingState = MappingState.Spike;
					break;
				case COMMAND_MONSTER:
					this.mappingState = MappingState.Monster;
					break;
				case COMMAND_SPIKE_LAUNCHER:
					this.mappingState = MappingState.SpikeLauncher;
					break;
				case COMMAND_HEALTH_KIT:
					this.mappingState = MappingState.HealthKit;
					break;
				default:
					Console.WriteLine("Failed to recognize your command, try using the editMapHelp()");
					break;
			}
			if (this.mappingState != oldState) {
				ScriptManager.getInstance().log("Changed to edit: " + this.mappingState.ToString());
			}
			Console.WriteLine("Mapping: " + this.mappingState.ToString());
		}

		public void update(Vector2 positionOffset) {
			Vector2 mousePos = new Vector2((InputManager.getInstance().MouseX - positionOffset.X), 
					(InputManager.getInstance().MouseY - positionOffset.Y));
			Point indexPosition = new Point((int)(mousePos.X / Constants.TILE_SIZE), (int)(mousePos.Y / Constants.TILE_SIZE));
			if (mousePos.X >= 0 && mousePos.Y >= 0) {
				StringBuilder xml = new StringBuilder();
				string subType = null;
				if (this.mappingState == MappingState.Monster) {
					subType = this.MonsterType;
				} else if (this.mappingState == MappingState.Spike) {
					subType = this.SpikeType;
				} else if (this.mappingState == MappingState.SpikeLauncher) {
					subType = this.LaunchDirection;
				}

				if (InputManager.getInstance().wasRightButtonPressed()) {
					// delete
					for (int i = 0; i < this.createdObjects.Count; i++) {
						if (this.createdObjects[i].position == indexPosition) {
							// only delete new objects
							this.objectDeleter(indexPosition);
							this.createdObjects.RemoveAt(i);
							break;
						}
					}
				}
				if (InputManager.getInstance().wasLeftButtonPressed()) {
					Console.WriteLine(indexPosition);
					if (InputManager.getInstance().isKeyDown(Keys.E)) {
						this.endPosition = indexPosition;
					} else {
						CreatedType type = new CreatedType();
						type.position = indexPosition;
						type.subType = subType;
						type.objType = this.mappingState;
						type.endPosition = this.endPosition;
						this.objectCreator(this.mappingState, indexPosition, endPosition, subType);
						this.createdObjects.Add(type);
					}
				}
			}
		}
		#endregion Support methods

		#region Destructor
		public void logEntries() {
			Point indexPosition;
			Point endPosition;
			StringBuilder xml = new StringBuilder();
			foreach (CreatedType type in this.createdObjects) {
				indexPosition = type.position;
				endPosition = type.endPosition;
				switch (type.objType) {
					case MappingState.Spike:
					case MappingState.SpikeLauncher:
						xml.Append("\n\t\t<" + type.objType + ">");
						xml.Append("\n\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
						xml.Append("\n\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
						xml.Append("\n\t\t\t<" + XML_TYPE + ">" + type.subType + "</" + XML_TYPE + ">");
						xml.Append("\n\t\t</" + type.objType + ">");
						break;
					case MappingState.Monster:
						xml.Append("\n\t\t<" + type.objType + ">");
						xml.Append("\n\t\t\t<" + XML_TYPE + ">" + type.subType + "</" + XML_TYPE + ">");
						xml.Append("\n\t\t\t<" + XML_PATH_START + ">");
						xml.Append("\n\t\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
						xml.Append("\n\t\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
						xml.Append("\n\t\t\t</" + XML_PATH_START + ">");
						xml.Append("\n\t\t\t<" + XML_PATH_END + ">");
						xml.Append("\n\t\t\t\t<" + XML_X + ">" + endPosition.X + "</" + XML_X + ">");
						xml.Append("\n\t\t\t\t<" + XML_Y + ">" + endPosition.Y + "</" + XML_Y + ">");
						xml.Append("\n\t\t\t</" + XML_PATH_END + ">");
						xml.Append("\n\t\t</" + type.objType + ">");
						break;
					default:
						if (type.objType != MappingState.None) {
							xml.Append("\n\t\t<" + type.objType + ">");
							xml.Append("\n\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
							xml.Append("\n\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
							xml.Append("\n\t\t</" + type.objType + ">");
						}
						break;
				}
			}
			if (this.createdObjects.Count >= 1) {
				ScriptManager.getInstance().log(xml.ToString());
			}
		}
		#endregion Destructor
#endif
	}
}