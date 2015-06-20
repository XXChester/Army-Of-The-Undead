using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using GWNorthEngine.Scripting;
using GWNorthEngine.Input;

using WOA3.Engine;
using WOA3.Logic;
using WOA3.Model;

namespace WOA3.Map {
	public class MapEditor {
		public enum MappingState {
			None,
			PlayerStart,
			Monster,
			Exit,
		};
#if DEBUG
		#region Debug class variables
		//singleton instance
		private static MapEditor instance = new MapEditor();

		private EditorCreator objectCreator;
		private List<CreatedType> createdObjects;

		private struct CreatedType {
			public MappingState objType;
			public MonsterType type;
			public Point position;
			public Point endPosition;
		}
		#endregion Debug class variables
#endif

		#region Class variables
		private MappingState mappingState;
		private MonsterType type;
		//private Point endPosition;
		private const string COMMAND_NONE = "none";
		private const string COMMAND_PLAYER_POSITION = "playerposition";
		/*private const string COMMAND_SPIKE = "spike";
		private const string COMMAND_FISH = "fish";
		private const string COMMAND_GOAL = "goal";*/
		//private const string COMMAND_MONSTER = "monster";
		private const string COMMAND_DEVIL = "devil";
		private const string COMMAND_YETI = "yeti";
		/*public const string COMMAND_CRATE = "crate";
		public const string COMMAND_SPIKE_LAUNCHER = "spikelauncher";
		public const string COMMAND_HEALTH_KIT = "healthkit";*/
		public const string XML_X = "X";
		public const string XML_Y = "Y";
		public const string XML_TYPE = "Type";
		public const string XML_PATH_START = "PathStart";
		public const string XML_PATH_END = "PathEnd";
		#endregion Class variables
		
#if DEBUG
		#region Class properties

		#endregion Class properties

		#region Constructor
		public MapEditor() {
			
		}
		#endregion Constructor

		#region Support methods
		public static MapEditor getInstance() {
			return instance;
		}

		public void init(EditorCreator objectCreator) {
			this.objectCreator = objectCreator;
			this.createdObjects = new List<CreatedType>();
		}

		public void editMapHelp() {
			Console.WriteLine("Format: [Information] - [Command]");
			Console.WriteLine("Turn Mapping off - " + COMMAND_NONE);
			Console.WriteLine("Players starting position - " + COMMAND_PLAYER_POSITION);
			/*Console.WriteLine("Spike (ensure SpikeType is set) - " + COMMAND_SPIKE);
			Console.WriteLine("SpikeType - SpikesBottom | SikesLeft | SpikesTop | SpikesRight | SpikesMidUp | SpikesMidDown");
			Console.WriteLine("Fish - " + COMMAND_FISH);
			Console.WriteLine("Crate - " + COMMAND_CRATE);
			Console.WriteLine("Goal - " + COMMAND_GOAL);
			Console.WriteLine("Health Kit - " + COMMAND_HEALTH_KIT);
			Console.WriteLine("SpikeLauncher - " + COMMAND_SPIKE_LAUNCHER);
			Console.WriteLine("LaunchDirection - Up | Down | Left | Right");
			Console.WriteLine("Monster(Left click starts, Left click + E = end) - " + COMMAND_MONSTER);
			Console.WriteLine("MonsterType - " + MONSTER_TYPE_ZOOM);*/
			Console.WriteLine("Devil spawn location - " + COMMAND_DEVIL);
			Console.WriteLine("Yeti spawn location - " + COMMAND_YETI);
			Console.WriteLine("Path Length - Length");
		}

		public void editMap(string value) {
			value = value.ToLower();
			switch (value) {
				case COMMAND_NONE:
					this.mappingState = MappingState.None;
					break;
				case COMMAND_PLAYER_POSITION:
					this.mappingState = MappingState.PlayerStart;
					break;
				case COMMAND_DEVIL:
					this.mappingState = MappingState.Monster;
					this.type = MonsterType.Devil;
					break;
				case COMMAND_YETI:
					this.mappingState = MappingState.Monster;
					this.type = MonsterType.Yeti;
					break;
				/*case COMMAND_MONSTER:
					this.mappingState = MappingState.Monster;
					break;*/
				default:
					Console.WriteLine("Failed to recognize your command, try using the editMapHelp()");
					break;
			}
			Console.WriteLine("Mapping: " + this.mappingState.ToString());
		}

		public void update() {
			Vector2 mousePos = InputManager.getInstance().MousePosition;
			Point indexPosition = mousePos.toPoint();
			if (mousePos.X >= 0 && mousePos.Y >= 0) {
				StringBuilder xml = new StringBuilder();

				if (InputManager.getInstance().wasLeftButtonPressed()) {
					Console.WriteLine(indexPosition);
					CreatedType type = new CreatedType();
					type.position = indexPosition;
					type.objType = this.mappingState;
					type.type = this.type;
					type.endPosition = mousePos.toPoint();
					this.objectCreator(this.mappingState, this.type, mousePos);
					this.createdObjects.Add(type);
				}
			}
		}
		#endregion Support methods

		#region Destructor
		public void logEntries() {
			Point indexPosition;
			Point endPosition;
			StringBuilder xml = new StringBuilder();
			if (this.createdObjects != null) {
				foreach (CreatedType type in this.createdObjects) {
					indexPosition = type.position;
					endPosition = type.endPosition;
					switch (type.objType) {
						case MappingState.Monster:
							xml.Append("\n\t\t<" + type.objType + ">");
							xml.Append("\n\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
							xml.Append("\n\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
							xml.Append("\n\t\t\t<" + XML_TYPE + ">" + type.type + "</" + XML_TYPE + ">");
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
		}
		#endregion Destructor
#endif
	}
}