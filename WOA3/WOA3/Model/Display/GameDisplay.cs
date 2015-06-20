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
using WOA3.Logic.GameStateMachine;
using WOA3.Engine;
using WOA3.Map;

namespace WOA3.Model.Display {
	public class GameDisplay : IRenderable {
		#region Class variables
		protected ContentManager content;
		protected string mapName;
		private bool mapLoaded;

		protected StaticDrawable2D bottomBorder;
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
		private List<Line2D> linesOfSight = new List<Line2D>();
		private List<Line2D> closestsGhosts = new List<Line2D>();
#endif
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameDisplay(GraphicsDevice graphics, ContentManager content, String mapName) {
			this.content = content;
			this.mapName = mapName;
			init(true);
			Constants.ALLOW_MOB_ATTACKS = false;
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
#if DEBUG
			/*Ghost g = new Ghost(content, new Vector2(672, 360), this.ghostObserverHandler, this.mobsInRange, this.ghostDeathFinish, 10f);
			this.allGhosts.Add(g);*/
#endif
			StaticDrawable2DParams botParms = new StaticDrawable2DParams() {
				Position = new Vector2(0f, 672),
				Texture = LoadingUtils.load<Texture2D>(content, "BottomBorder"),
			};
			this.bottomBorder = new StaticDrawable2D(botParms);
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
			foreach (var ghost in selectedGhosts) {
				ghost.performSkills();
			}

			foreach (var mob in mobs) {
				mob.performSkills();
				mob.update(elapsed);
			}
			//check for dead
			handleDead(this.allGhosts);
			handleDead(this.mobs);
		}


		private ClosestSeeable getMobsFieldOfView(Mob mob) {
			ClosestSeeable closestSeeable = null;
			foreach (var ghost in allGhosts) {
				if (ghost.isVisible()) {
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
							ClosestSeeable candidate = new ClosestSeeable() { Ghost = ghost, Distance = (float)distanceToTarget, Mob = mob };

#if DEBUG
							// 1 Add it to the FOV list
							Line2DParams parms = new Line2DParams() {
								Position = candidate.Ghost.Position,
								EndPosition = candidate.Mob.Position,
								LightColour = Color.Purple,
								Texture = Debug.debugChip
							};
							linesOfSight.Add(new Line2D(parms));
#endif
							// are we the first candidate?
							if (closestSeeable != null) {

								//2 Figure out if it is closer than previous candidates
								if (candidate.Distance < closestSeeable.Distance) {
									closestSeeable = candidate;
								}
							} else {
								closestSeeable = candidate;
							}
						}
					}
				}
			}
#if DEBUG
			if (closestSeeable != null) {
				Line2DParams parms = new Line2DParams() {
					Position = closestSeeable.Ghost.Position,
					EndPosition = closestSeeable.Mob.Position,
					LightColour = Color.Yellow,
					Texture = Debug.debugChip
				};
				closestsGhosts.Add(new Line2D(parms));
			}
#endif
			return closestSeeable;
		}


		private void updateFieldOfView(float elapsed) {
			//foreach (var ghost in allGhosts) {
			Wall collidedWith = null;
			//if (ghost.isVisible()) {
			foreach (var mob in mobs) {
				ClosestSeeable closestSeeable = getMobsFieldOfView(mob);
				// are we against a wall
				bool hitWall = false;
				foreach (Wall wall in map.Walls) {
					// is the ghost in a wall?
					if (wall.BBox.Intersects(mob.BBox)) {
					//if (wall.BBox.Intersects(mob.BoundingSphere)) {
						hitWall = true;
						collidedWith = wall;
					}
				}

				// can we even see anything?
				if (closestSeeable != null) {

					// did we hit a wall?
					if (hitWall) {
						// are we already pathing around it?
						//if (!mob.isPathing()) {
						/*if (!mob.isLost()) {
							// chart a path to the last known location
							mob.Unsubscribe();
						}*/
					//	if (!mob.isPathing()) {
							// chart a path to the last known location
						//	mob.Unsubscribe(closestSeeable.Ghost.Position);
					//	}
					} else {
						// we aren't hitting a wall, track the target
						//mob.Subscribe(this.ghostObserverHandler, closestSeeable.Ghost);
					}

					if (!hitWall) {
						mob.Subscribe(this.ghostObserverHandler, closestSeeable.Ghost);
					} else {
						// if we are hitting a wall and can still see the mob, route a path to it
						mob.Unsubscribe(closestSeeable.Ghost.Position);
					}


					/*if (!mob.isPathing()) {
						mob.pathToWaypoint();
					}*/
				} else {
					// if we cannot see a close mob, than go to the last know location
					/*if (!mob.isLost()) {
						mob.lostTarget();
					}*/
					//if (!mob.isPathing() && !mob.isIdle()) {
						mob.Unsubscribe();
					//}
				}
				if (hitWall && mob.isTracking()) {
					mob.collision();
				}
				//}
				//}
			}
		}

		private class ClosestSeeable {
			public Ghost Ghost { get; set; }
			public float Distance { get; set; }
			public Mob Mob { get; set; }
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
			linesOfSight.Clear();
			closestsGhosts.Clear();

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
			/*Point mouse = InputManager.getInstance().MousePosition.toPoint();
			GWNorthEngine.AI.AStar.BasePathFinder.TypeOfSpace space = AIManager.getInstance().Board[mouse.Y, mouse.X];
			Debug.log(space.ToString());*/
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
			this.bottomBorder.render(spriteBatch);
			foreach (var dead in this.theDead) {
				if (dead != null) {
					dead.render(spriteBatch);
				}
			}
#if DEBUG
			if (Debug.debugOn) {
				foreach (var line in linesOfSight) {
					line.render(spriteBatch);
				}

				foreach (var line in closestsGhosts) {
					line.render(spriteBatch);
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
