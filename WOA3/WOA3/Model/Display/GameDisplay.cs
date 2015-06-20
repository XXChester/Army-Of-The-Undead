using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.StateMachine;
using WOA3.Engine;
using WOA3.Map;

namespace WOA3.Model.Display {
	public class GameDisplay : IRenderable {
		#region Class variables
		protected ContentManager content;
		protected string mapName;
		private bool mapLoaded;

		protected Map map;
		protected HUD hud;
		protected List<Ghost> allGhosts = new List<Ghost>();
		protected List<Ghost> selectedGhosts = new List<Ghost>();
		protected List<Mob> mobs = new List<Mob>();
		protected List<Base2DSpriteDrawable> theDead = new List<Base2DSpriteDrawable>();
		protected GhostObservationHandler ghostObserverHandler;
		protected CharactersInRange mobsInRange;
		protected CharactersInRange ghostsInRange;
		protected OnDeath mobDeathFinish;
		protected OnDeath ghostDeathFinish;
		protected CollisionCheck collisionCheck;
		private List<Ghost> recentlySpawned;

#if DEBUG
		private EditorCreator editorsCreator;
		private List<Vector3[]> bboxes= new List<Vector3[]>();
#endif
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameDisplay(GraphicsDevice graphics, ContentManager content, String mapName) {
			this.content = content;
			this.mapName = mapName;
			init(true);
			Constants.ALLOW_MOB_ATTACKS = true;
			Constants.ALLOW_PLAYER_ATTACKS = true;
			this.mapLoaded = true;
		}
		#endregion Constructor

		#region Support methods
		protected void init(bool fullRegen = false) {
			if (fullRegen) {
				this.allGhosts.Clear();
				this.mobs.Clear();
				this.map = null;
				this.recentlySpawned = new List<Ghost>();
			}
			this.ghostObserverHandler = new GhostObservationHandler();
			this.hud = new HUD(content);
			initDelegates();
			loadMap();
			// if we have ghosts left over, we need to preserve them
			if (GameStateMachine.getInstance().LevelContext != null && GameStateMachine.getInstance().LevelContext.Ghosts != null) {
				Ghost primary = this.allGhosts[0];
				Ghost ghost = null;
				for (int i = 1; i <= GameStateMachine.getInstance().LevelContext.Ghosts.Count; i++) {
					ghost = GameStateMachine.getInstance().LevelContext.Ghosts[i - i];
					float factor = ghost.Health; 
					if (factor > 0) {
						Vector2 newPosition = Vector2.Add(primary.Position, new Vector2(i * (Constants.TILE_SIZE / 2)));
						this.allGhosts.Add(new Ghost(content, newPosition, this.ghostObserverHandler, this.mobsInRange, this.ghostDeathFinish, factor));
					}
				}
			}
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
				this.mobs.Add(createMob(mobInfo.Start.toVector2(), mobInfo.TypeOfMob));
			}
			this.allGhosts.Add(new Ghost(content, ghostStart.toVector2(), this.ghostObserverHandler, this.mobsInRange, this.ghostDeathFinish));
		}

		private Mob createMob(Vector2 position, MonsterType typeOfMob) {
			Mob mob = null;
			if (typeOfMob == MonsterType.Devil) {
				mob = new Devil(content, position, this.ghostsInRange, this.mobDeathFinish, this.collisionCheck);
			} else if (typeOfMob == MonsterType.Yeti) {
				mob = new Yeti(content, position, this.ghostsInRange, this.mobDeathFinish, this.collisionCheck);
			}
			return mob;
		}

		private List<Character> getCharactersInRange<T>(BoundingSphere range, List<T> all) where T : Character {
			List<Character> result = new List<Character>();
			for (int j = all.Count - 1; j >= 0; j--) {
				T inRange = all[j];
				if (inRange.isVisible()) {
					if (inRange.BBox.Intersects(range)) {
						result.Add(inRange);
					}
				}
			}
			return result;
		}

		private void initDelegates() {
			this.mobDeathFinish = delegate(Character character) {
				String texture = "Monster1";
				if (character.GetType() == typeof(Yeti)) {
					texture = "Monster2";
				}
				texture += "Death";
				Vector2 position = character.Position;
				int frames = 10;
				float speed = 100f;
				BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams() {
					AnimationState = AnimationState.PlayForwardOnce,
					TotalFrameCount = frames,
					FrameRate = speed,
				};
				Animated2DSpriteLoadSingleRowBasedOnTexture parms = new Animated2DSpriteLoadSingleRowBasedOnTexture() {
					AnimationParams = animationParms,
					Position = position,
					LightColour = Color.White,
					Texture = LoadingUtils.load<Texture2D>(content, texture)
				};
				

				Animated2DSprite deathSprite = new Animated2DSprite(parms);
				this.recentlySpawned.Add(new Ghost(content, position, this.ghostObserverHandler, this.mobsInRange, this.ghostDeathFinish));
				this.theDead.Add(deathSprite);
			};
			this.ghostDeathFinish = delegate(Character character) {
				String texture = "GhostDeath";
				Vector2 position = character.Position;
				int frames = 10;
				float speed = 100f;
				BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams() {
					AnimationState = AnimationState.PlayForwardOnce,
					TotalFrameCount = frames,
					FrameRate = speed,
				};
				Animated2DSpriteLoadSingleRowBasedOnTexture parms = new Animated2DSpriteLoadSingleRowBasedOnTexture() {
					AnimationParams = animationParms,
					Position = position,
					LightColour = Color.White,
					Texture = LoadingUtils.load<Texture2D>(content, texture)
				};
				Ghost ghost = (Ghost)character;
				ghost.Selected = false;
				this.selectedGhosts.Remove(ghost);
				this.allGhosts.Remove(ghost);
				this.theDead.Add(new Animated2DSprite(parms));
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
				if (character != null && character.AmIDead) {
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
			Vector3 min, max;
			foreach (var ghost in allGhosts) {
				foreach (var mob in mobs) {
					min = Vector2.Min(ghost.Position, mob.Position).toVector3();
					max = Vector2.Max(ghost.Position, mob.Position).toVector3();
					BoundingBox bbox = new BoundingBox(min, max);
					Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
					Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
					bool canSee = true;
					if (distanceToTarget != null) {
						foreach (Wall wall in map.Walls) {
							if (wall.BBox.Intersects(bbox)) {
								canSee = false;
								break;
							} else {
								Nullable<float> distance = CollisionUtils.castRay(wall.BBox, mob.Position, direction);
								// as soon as we cannot see the target, stop looking
								if (distance != null && distance < distanceToTarget) {
									canSee = false;
									break;
								}
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
			bboxes = new List<Vector3[]>();
			// cast a ray from our chaser to the target. If this ray hits the target, test it against all other objects
			ClosestSeeable closestSeeable = getClosestGhost();
			foreach (var ghost in allGhosts) {
				Wall collidedWith = null;
				if (ghost.isVisible()) {
					foreach (var mob in mobs) {
						Vector3 min = Vector2.Min(ghost.Position, mob.Position).toVector3();
						Vector3 max = Vector2.Max(ghost.Position, mob.Position).toVector3();
						BoundingBox bbox = new BoundingBox(min, max);
						
#if DEBUG
						this.bboxes.Add(new Vector3[] { min, max });
#endif
						Vector2 direction = Vector2.Subtract(ghost.Position, mob.Position);
						Nullable<float> distanceToTarget = CollisionUtils.castRay(ghost.BBox, mob.Position, direction);
						bool pathing = mob.isPathing();
						bool hitWall = false;
						bool pathBlocked = false;
						if (distanceToTarget != null) {
							foreach (Wall wall in map.Walls) {
								// is the ghost in a wall?
								//if (wall.BBox.Intersects(mob.BBox)) {
								if (wall.BBox.Intersects(mob.BoundingSphere)) {
									hitWall = true;
									collidedWith = wall;
								}

								if (wall.BBox.Intersects(bbox)) {
									pathBlocked = true;
								}
							}
						}



						//if (!mob.isPathing()) {
						if (closestSeeable != null && !pathBlocked) {
								mob.Subscribe(this.ghostObserverHandler, ghost);
						/*} else if ((mob.isPathing() || mob.isTracking())  && closestSeeable == null) {
							mob.lostTarget();*/
						} else if (hitWall) {
							/*if (!mob.isPathing()) {
								mob.pathToWaypoint();
							}*/
							mob.lostTarget();
						} else if (!mob.isLost()) {
							mob.Unsubscribe();
						}
					}
				}
			}
		}

		private class ClosestSeeable {
			public Ghost Ghost { get; set; }
			public float Distance { get; set; }
		}

		protected virtual bool winConditionAchieved() {
			bool win = false;
			if (this.mapLoaded) {
				win = true;
				foreach (var mob in mobs) {
					if (!mob.AmIDead) {
						win = false;
					}
				}
			}
			return win;
		}

		protected bool looseConditionDetected() {
			bool loose = this.mapLoaded;
			foreach (var ghost in this.allGhosts) {
				if (!ghost.AmIDead) {
					loose = false;
				}
			}
			return loose;
		}

		public virtual void update(float elapsed) {
			foreach (var mob in mobs) {
				mob.update(elapsed);
			}
			foreach (var dead in this.theDead) {
				if (dead != null) {
					dead.update(elapsed);
				}
			}
			foreach (var ghost in this.allGhosts) {
				ghost.update(elapsed);
			}

			updateFieldOfView(elapsed);
			updateSkills(elapsed);

			CombatManager.getInstance().update(elapsed);
			EffectsManager.getInstance().update(elapsed);
			SoundManager.getInstance().update();

			if (looseConditionDetected()) {
				((GameDisplayState)GameStateMachine.getInstance().CurrentState).goToGameOver();
			} else if (winConditionAchieved()) {
				LevelContext context = new LevelContext() {
					Ghosts = allGhosts,
					MapIndex = GameStateMachine.getInstance().LevelContext.MapIndex
				};
				GameStateMachine.getInstance().LevelContext = context;
				GameStateMachine.getInstance().goToNextState();
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
			this.hud.update(elapsed);

			// ghosts spawned as part of this cycle are not applicable to the current frame so add them here
			if (this.recentlySpawned != null && this.recentlySpawned.Count > 0) {
				this.allGhosts.AddRange(this.recentlySpawned);
				this.recentlySpawned = new List<Ghost>();
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
			foreach (var dead in this.theDead) {
				if (dead != null) {
					dead.render(spriteBatch);
				}
			}
#if DEBUG
			if (Debug.debugOn) {
				foreach (var box in this.bboxes) {
					DebugUtils.drawVector3s(spriteBatch, box[1], box[0], Color.Pink, Debug.debugChip);
				}
			}
#endif
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
