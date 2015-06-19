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
	public class GameDisplay : IRenderable {
		#region Class variables
		protected ContentManager content;
		protected string mapName;


		protected Map map;
		protected HUD hud;
		protected List<Ghost> allGhosts = new List<Ghost>();
		protected List<Ghost> selectedGhosts = new List<Ghost>();
		protected List<Mob> mobs = new List<Mob>();
		protected GhostObservationHandler ghostObserverHandler;
		protected CharactersInRange mobsInRange;
		protected CharactersInRange ghostsInRange;
		protected OnDeath mobDeathFinish;
		protected CollisionCheck collisionCheck;

#if DEBUG
		private EditorCreator editorsCreator;
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
		protected void init(bool fullRegen = false) {
			if (fullRegen) {
				this.allGhosts.Clear();
				this.mobs.Clear();
				this.map = null;
				/*this.backGround = new BackGround(this.content);
				this.hud = new HUD(this.content);
				this.foodManager = new FoodManager(content, this.rand);
				this.portals = new PortalManager(content, this.rand);
				this.walls = new WallManager(content, this.rand);*/
			}
			this.ghostObserverHandler = new GhostObservationHandler();
			this.hud = new HUD(content);
			initDelegates();
			loadMap();
			CombatManager.getInstance().init();
			EffectsManager.getInstance().init();
		}

		private void loadMap() {
#if DEBUG
			MapEditor.getInstance().init(this.editorsCreator);
#endif
			string suffix = ".xml";
			XmlReader xmlReader = XmlReader.Create(Constants.MAP_DIRECTORY + this.mapName + "Identifiers" + suffix);

			Point ghostStart = new Point();
			List<SpecializedLoadResult> monsterInfos = new List<SpecializedLoadResult>();
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlReader);

				// load the map information
				MapLoader.loadMap(this.content, Constants.MAP_DIRECTORY + this.mapName + suffix, out this.map);

				MapLoader.loadPlayerInformation(doc, ref ghostStart);

				MapLoader.loadSpecializedInformation(doc, MapEditor.MappingState.Monster, ref monsterInfos);
				//MapLoader.loadMonsterInformation(doc, ref monsterInfos);
				//MapLoader.loadGenericPointList(doc, MapEditor.MappingState.Monster, out monsterInfos);
			} finally {
				xmlReader.Close();
			}

			foreach (var mobInfo in monsterInfos) {
				if (mobInfo.TypeOfMob == MonsterType.Devil) {
					this.mobs.Add(new Devil(content, mobInfo.Start.toVector2(), this.ghostsInRange, this.mobDeathFinish, this.collisionCheck));
				} else if (mobInfo.TypeOfMob == MonsterType.Yeti) {
					this.mobs.Add(new Yeti(content, mobInfo.Start.toVector2(), this.ghostsInRange, this.mobDeathFinish, this.collisionCheck));
				}
			}
			this.allGhosts.Add(new Ghost(content, ghostStart.toVector2(), this.ghostObserverHandler, this.mobsInRange));
		}

		private List<Character> getCharactersInRange<T>(BoundingSphere range, List<T> all) where T : Character {
			List<Character> result = new List<Character>();
			for (int j = all.Count - 1; j >= 0; j--) {
				T inRange = all[j];
				if (inRange.BBox.Intersects(range)) {
					result.Add(inRange);
				}
			}
			return result;
		}

		private void initDelegates() {
			this.mobDeathFinish = delegate(Vector2 position) {
				this.allGhosts.Add(new Ghost(content, position, this.ghostObserverHandler, this.mobsInRange));
			};
			this.ghostsInRange = delegate(BoundingSphere range) {
				return getCharactersInRange<Ghost>(range, this.allGhosts);
			};
			this.mobsInRange = delegate(BoundingSphere range) {
				return getCharactersInRange<Mob>(range, this.mobs);
			};
			this.collisionCheck = delegate(Vector2 newPosition) {
				bool safe = true;
				BoundingBox newBoundingBox = CollisionGenerationUtils.getBBox(newPosition);
				foreach (Wall wall in map.Walls) {
					if (wall.BBox.Intersects(newBoundingBox)) {
						safe = false;
						break;
					}
				}
				return safe;
			};
#if DEBUG
			this.editorsCreator = delegate(MapEditor.MappingState type, MonsterType monsterType, Vector2 position) {
				switch (type) {
					case MapEditor.MappingState.Monster:
						if (monsterType == MonsterType.Devil) {
							this.mobs.Add(new Devil(content, position, this.ghostsInRange, this.mobDeathFinish, this.collisionCheck));
						} else if(monsterType == MonsterType.Yeti) {
							this.mobs.Add(new Yeti(content, position, this.ghostsInRange, this.mobDeathFinish, this.collisionCheck));
						}
						break;
				};
			};
#endif
		}

		private void handleDead<T>(List<T> characters) where T : Character{
			for (int j = characters.Count - 1; j >= 0; j--) {
				T character = characters[j];
				if (character != null && character.Health.amIDead()) {
					characters.RemoveAt(j);
					character = null;
				}
			}
		}

		private void updateSkills(float elapsed) {
			foreach (var mob in mobs) {
				mob.performSkills();
				mob.update(elapsed);
			}
			foreach (var ghost in selectedGhosts) {
				ghost.performSkills();
			}

			//check for dead
			handleDead(this.allGhosts);
			handleDead(this.mobs);
		}

		/*
		 Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
		 Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
						
		 if (!hitWall) {
				// can we see the target
				Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
				// as soon as we cannot see the target, stop looking
				if (distance != null && distance < distanceToTarget) {
					canSee = false;
					toBreak = true;
				} else {
					canSee = true;
				}
			}
		 * */

		private bool isWallBetween(Wall wall, Mob mob, Ghost ghost) {
			bool wallBetween = false;
			Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
			Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
			Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
			if (distance != null && distance < distanceToTarget) {
				wallBetween = true;
			}

			/*Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
			//Nullable<float> distanceBetween = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
			Nullable<float> distanceToWall = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
			if (distanceToWall == null) {
				wallBetween = false;
			} else if (doesRayGoThroughWall != null && distanceBetween != null && distanceBetween.Value < doesRayGoThroughWall.Value) {
				wallBetween = false;
			}*/
			return wallBetween;
		}

		private ClosestSeeable getClosestGhost() {
			ClosestSeeable closestSeeable = null;
			foreach (var ghost in allGhosts) {
				foreach (var mob in mobs) {
					Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
					Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
					bool canSee = true;
					if (distanceToTarget != null) {
						foreach (Wall wall in map.Walls) {
							Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
							// as soon as we cannot see the target, stop looking
							if (distance != null && distance < distanceToTarget) {
								canSee = false;
								break;
							}
						}
						if (canSee) {
							if (closestSeeable == null || ((ClosestSeeable)closestSeeable).Distance > distanceToTarget) {
								closestSeeable = new ClosestSeeable() { Ghost = ghost, Distance = (float)distanceToTarget };
							}
						}
					}
				}
			}
			return closestSeeable;
		}

		private void updateFieldOfView(float elapsed) {
			// cast a ray from our chaser to the target. If this ray hits the target, test it against all other objects
			ClosestSeeable closestSeeable = getClosestGhost();
			foreach (var ghost in allGhosts) {
				bool ghostInWall = false;
				Wall collidedWith = null;
				if (ghost.isVisible()) {
				ghostInWall: if (ghostInWall) {
						// if the ghost is in a wall, skip him
						continue;
					}
					foreach (var mob in mobs) {
						Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
						Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
						bool pathing = mob.isPathing();
						bool hitWall = false;
						if (distanceToTarget != null) {
							foreach (Wall wall in map.Walls) {
								// is the ghost in a wall?
								if (wall.BBox.Intersects(ghost.BBox)) {
									ghostInWall = true;
									goto ghostInWall;
								}
								//if (wall.BBox.Intersects(mob.BBox)) {
								if (wall.BBox.Intersects(mob.BoundingSphere)) {
									hitWall = true;
									collidedWith = wall;
								}
							}
						}



						//if (!mob.isPathing()) {
						if (closestSeeable != null) {
								mob.Subscribe(this.ghostObserverHandler, ghost);
						/*} else if ((mob.isPathing() || mob.isTracking())  && closestSeeable == null) {
							mob.lostTarget();*/
						} else if (hitWall) {
							if (!mob.isPathing()) {
								mob.pathToWaypoint();
							}
						} else if (!mob.isLost()) {
							mob.lostTarget();
						}
					}
				}
			}
		}

		private class ClosestSeeable {
			public Ghost Ghost { get; set; }
			public float Distance { get; set; }
		}

		public virtual void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == GameState.Active) {
				foreach (var mob in mobs) {
					mob.update(elapsed);
				}
				foreach (var ghost in this.allGhosts) {
					ghost.update(elapsed);
				}

				SoundManager.getInstance().update();


				updateFieldOfView(elapsed);
				updateSkills(elapsed);

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
				this.hud.update(elapsed);
				CombatManager.getInstance().update(elapsed);
				EffectsManager.getInstance().update(elapsed);
			}
#if DEBUG
			MapEditor.getInstance().update();
#endif
		}

		public virtual void render(SpriteBatch spriteBatch) {
			EffectsManager.getInstance().render(spriteBatch);
			foreach (var ghost in this.allGhosts) {
				ghost.render(spriteBatch);
			}

			this.map.render(spriteBatch);

			foreach (var mob in mobs) {
				mob.render(spriteBatch);
			}
		//	this.hud.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			// AI
			AIManager.getInstance().Dispose();
		}
		#endregion Destructor
	}
}
