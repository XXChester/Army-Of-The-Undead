using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Engine;
using WOA3.Map;

namespace WOA3.Model.Display {
	public class GameDisplay : IRenderable, IDisposable {
		#region Class variables
		private string mapName;
		private ContentManager content;
		private BoundingBox boundary;
		//private MapCollisionChecker mapCollisionChecker;


		private Map map;
		private List<Ghost> allGhosts = new List<Ghost>();
		private List<Ghost> selectedGhosts = new List<Ghost>();
		private List<Goof> mobs = new List<Goof>();
		private GhostObservationHandler ghostObserverHandler;

#if DEBUG
		private EditorCreator editorsCreator;
		private EditorDeleter editorsDeleter;
#endif
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameDisplay(GraphicsDevice graphics, ContentManager content, String mapName) {
			this.content = content;
			this.mapName = mapName;
			init(true);
		}
		#endregion Constructor

		#region Support methods
		private void init(bool fullRegen = false) {
			initDelegates();
			loadMap();
			Vector3 min = new Vector3(0f);
			Vector3 max = new Vector3(Constants.RESOLUTION_X, Constants.RESOLUTION_Y, 0f);
			this.boundary = new BoundingBox(min, max);
			this.ghostObserverHandler = new GhostObservationHandler();
			this.allGhosts.Add(new Ghost(content, new Vector2(100f), this.ghostObserverHandler));
			this.selectedGhosts.Add(this.allGhosts[0]);
			this.allGhosts[0].Selected = true;
			this.mobs.Add(new Goof(content, new Vector2(500f)));

			if (fullRegen) {
				/*this.backGround = new BackGround(this.content);
				this.hud = new HUD(this.content);
				this.foodManager = new FoodManager(content, this.rand);
				this.portals = new PortalManager(content, this.rand);
				this.walls = new WallManager(content, this.rand);*/
			}


#if DEBUG
			Debug.debugChip = LoadingUtils.load<Texture2D>(this.content, "Chip");
#endif
		}

		private void loadMap() {
#if DEBUG
			MapEditor.getInstance().init(this.editorsCreator, this.editorsDeleter);
#endif
			string suffix = ".xml";
			XmlReader xmlReader = XmlReader.Create(Constants.MAP_DIRECTORY + this.mapName + "Identifiers" + suffix);

			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlReader);

				// load the map information
				MapLoader.loadMap(this.content, Constants.MAP_DIRECTORY + this.mapName + suffix, out this.map);
			} finally {
				xmlReader.Close();
			}
		}

		private void initDelegates() {
			/*this.mapCollisionChecker = delegate(BoundingBox bbox, Vector2 objectsPosition) {
				return this.map.collisionDetected(bbox, objectsPosition);
			};*/
#if DEBUG
			this.editorsCreator = delegate(MapEditor.MappingState type, Point point, Point end, string subType) {
				/*switch (type) {
					case MapEditor.MappingState.Crate:
						this.levelObjects.Add(new Crate(this.content, new Vector2(point.X, point.Y),
							this.fishCounterIncrementer));
						break;
					case MapEditor.MappingState.Fish:
						this.levelObjects.Add(new Fish(this.content, new Vector2(point.X, point.Y),
							this.fishCounterIncrementer));
						break;
					case MapEditor.MappingState.HealthKit:
						this.levelObjects.Add(new HealthKit(this.content, new Vector2(point.X, point.Y),
							this.playerHealthModifier));
						break;
					case MapEditor.MappingState.Spike:
						this.levelObjects.Add(new Spike(this.content, subType, new Vector2(point.X, point.Y),
							this.playerHealthModifier));
						break;
					case MapEditor.MappingState.SpikeLauncher:
						this.spikeLaunchers.Add(new SpikeLauncher(this.content, new Vector2(point.X, point.Y),
							this.playerHealthModifier, (CardinalDirection)Enum.Parse(typeof(CardinalDirection),
							subType), this.mapCollisionChecker));
						break;
					case MapEditor.MappingState.Monster:
						if (subType == MapEditor.MONSTER_TYPE_ZOOM) {
							SpecializedLoadResult result = new SpecializedLoadResult();
							result.Start = point;
							result.End = end;
							result.Type = subType;
							this.monsters.Add(new Zoom(this.content, this.mapCollisionChecker, result, this.scoreIncrementer, this.playerHealthModifier));
						}
						break;
				};*/
			};

			/*this.editorsDeleter = delegate(Point point) {
				Vector2 min = new Vector2(point.X * Constants.TILE_SIZE, point.Y * Constants.TILE_SIZE);
				Vector2 max = Vector2.Add(min, new Vector2(Constants.TILE_SIZE - 1));
				Vector2 position;
				bool found = false;
				for (int i = this.levelObjects.Count - 1; i > 0; i--) {
					if (this.levelObjects[i] != null) {
						position = this.levelObjects[i].Position;
						if (position.X >= min.X && position.X <= max.X &&
							position.Y >= min.Y && position.Y <= max.Y) {
							this.levelObjects.RemoveAt(i);
							found = true;
							break;
						}
					}
				}
			};*/
#endif
		}

		private void updateSkills(float elapsed) {
			// did we use any skills?
			List<SkillResult> skillResults = new List<SkillResult>();
			foreach (var ghost in selectedGhosts) {
				skillResults.AddRange(ghost.performSkills());
			}

			// perform the damage against all mobs in the area
			for (int i = mobs.Count - 1; i >= 0; i--) {
				var mob = mobs[i];
				float damage = 0f;
				foreach (var skillResult in skillResults) {
					if (skillResult.BoundingSphere.Intersects(mob.BBox)) {
						damage += skillResult.Damage;
					}
				}
				if (damage > 0f) {
					mob.scare(damage);
					if (mob.Scared.amIDead()) {
						mobs.RemoveAt(i);
						this.allGhosts.Add(new Ghost(content, mob.Position, this.ghostObserverHandler));
					}
				}
			}
		}

		private void updateFieldOfView(float elapsed) {
			// cast a ray from our chaser to the target. If this ray hits the target, test it against all other objects
			Nullable<ClosestSeeable> closestSeeable = null;
			foreach (var ghost in allGhosts) {
				ghost.update(elapsed);
				if (ghost.isVisible()) {

					foreach (var mob in mobs) {
						Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
						Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);

						if (distanceToTarget != null) {
							bool canSee = true;
							foreach (Wall wall in map.Walls) {
								Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
								if (distance != null && distance < distanceToTarget) {
									canSee = false;
								}
							}
							if (canSee) {
								if (closestSeeable == null || ((ClosestSeeable)closestSeeable).Distance > distanceToTarget) {
									closestSeeable = new ClosestSeeable() { Ghost = ghost, Distance = (float)distanceToTarget };
								}
							}
							if (closestSeeable == null) {
								if (!mob.isStopped()) {
									mob.lostTarget();
								}
							} else {
								mob.Subscribe(this.ghostObserverHandler, ghost);
							}
						}
					}
				}
			}
		}

		private struct ClosestSeeable {
			public Ghost Ghost { get; set; }
			public float Distance { get; set; }
		}

		public void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == GameState.Active) {
				foreach (var mob in mobs) {
					mob.update(elapsed);
				}

				SoundManager.getInstance().update();


				updateFieldOfView(elapsed);
				updateSkills(elapsed);

				Wall collision = null;
				foreach (var mob in mobs) {
					if (!mob.isStopped()) {
						collision = this.map.collisionDetected(mob.BBox);
						if (collision != null) {
							mob.stop(collision);
						}
					}
				}

				if (InputManager.getInstance().wasLeftButtonPressed()) {
					if (!InputManager.getInstance().isKeyDown(Keys.LeftShift)) {
						foreach (var ghost in allGhosts) {
							ghost.Selected = false;
						}
						this.selectedGhosts.Clear();
					}
					Ghost selectedGhost = null;
					foreach (var ghost in allGhosts) {
						if (PickingUtils.pickVector(InputManager.getInstance().MousePosition, ghost.BBox)) {
							selectedGhost = ghost;
							ghost.Selected = true;
							this.selectedGhosts.Add(ghost);
						}
					}
				}
			}

			if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
				StateManager.getInstance().CurrentGameState = GameState.MainMenu;
			}
#if DEBUG
			Debug.update();
#endif
		}

		public void render(SpriteBatch spriteBatch) {
			foreach (var ghost in this.allGhosts) {
				ghost.render(spriteBatch);
			}

			this.map.render(spriteBatch);

			foreach (var mob in mobs) {
				mob.render(spriteBatch);
			}

#if DEBUG
			if (Debug.debugOn) {
				DebugUtils.drawBoundingBox(spriteBatch, this.boundary, Debug.DEBUG_BBOX_Color, Debug.debugChip);
			}
#endif
		}
		#endregion Support methods

		#region Destructor
		public void Dispose() {
			// AI
			AIManager.getInstance().Dispose();
		}
		#endregion Destructor
	}
}
