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
		private ContentManager content;
		private string mapName;


		private Map map;
		private HUD hud;
		private List<Ghost> allGhosts = new List<Ghost>();
		private List<Ghost> selectedGhosts = new List<Ghost>();
		private List<Mob> mobs = new List<Mob>();
		private GhostObservationHandler ghostObserverHandler;

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
		private void init(bool fullRegen = false) {
			initDelegates();
			loadMap();
			this.ghostObserverHandler = new GhostObservationHandler();
			this.hud = new HUD(content);

			if (fullRegen) {
				/*this.backGround = new BackGround(this.content);
				this.hud = new HUD(this.content);
				this.foodManager = new FoodManager(content, this.rand);
				this.portals = new PortalManager(content, this.rand);
				this.walls = new WallManager(content, this.rand);*/
			}
		}

		private void loadMap() {
#if DEBUG
			MapEditor.getInstance().init(this.editorsCreator);
#endif
			string suffix = ".xml";
			XmlReader xmlReader = XmlReader.Create(Constants.MAP_DIRECTORY + this.mapName + "Identifiers" + suffix);

			Point ghostStart = new Point();
			List<Point> monsterInfos = new List<Point>();
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlReader);

				// load the map information
				MapLoader.loadMap(this.content, Constants.MAP_DIRECTORY + this.mapName + suffix, out this.map);

				MapLoader.loadPlayerInformation(doc, ref ghostStart);

				MapLoader.loadGenericPointList(doc, MapEditor.MappingState.Monster, out monsterInfos);
			} finally {
				xmlReader.Close();
			}

			foreach (var mobInfo in monsterInfos) {
				this.mobs.Add(new Mob(content, mobInfo.toVector2()));
			}
			this.allGhosts.Add(new Ghost(content, ghostStart.toVector2(), this.ghostObserverHandler));
		}

		private void initDelegates() {
#if DEBUG
			this.editorsCreator = delegate(MapEditor.MappingState type, Vector2 position) {
				switch (type) {
					case MapEditor.MappingState.Monster:
						this.mobs.Add(new Mob(content, position));
						break;
				};
			};
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
					mob.damage(damage);
					if (mob.Health.amIDead()) {
						SkillResult deathEffect = mob.die();
						foreach (var inRange in allGhosts) {
							if (inRange.BBox.Intersects(deathEffect.BoundingSphere)) {
								inRange.damage(deathEffect.Damage);
							}
						}
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
							bool pathing = mob.isPathing();
							bool hitWall = false;
							bool toBreak = false;
							foreach (Wall wall in map.Walls) {
								if (!mob.isIdle()) {
									// are we going to collide with a wall?
									/*if (!mob.isPathing() && wall.BBox.Intersects(mob.BoundingSphere)) {
										pathing = true;
										toBreak = true;
									}*/
									if (!mob.isPathing() && wall.BBox.Intersects(mob.BBox)) {
										pathing = true;
										hitWall = true;
										toBreak = true;
									}
								}

								Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
								if (distance != null && distance < distanceToTarget) {
									canSee = false;
								}
								if (toBreak) {
									break;
								}
							}
							if (canSee && !pathing) {
								if (closestSeeable == null || ((ClosestSeeable)closestSeeable).Distance > distanceToTarget) {
									closestSeeable = new ClosestSeeable() { Ghost = ghost, Distance = (float)distanceToTarget };
								}
							}
							float t = 0f;
							if (closestSeeable != null) {
								t = closestSeeable.Value.Distance;
							}
						//	Debug.log("canSee: " + canSee + "\tpathing: " + pathing + "\thitWall: " + hitWall + "\tidle: " + mob.isIdle());

							if (pathing && !canSee) {
								mob.pathToWaypoint();
							} else {
								if (!pathing) {
									if (hitWall) {
										mob.stop();
									}
									if (closestSeeable == null) {
										if (!mob.isStopped() && !mob.isIdle()) {
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
			}
#if DEBUG
			Debug.update();
			MapEditor.getInstance().update();
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
