using System;
using System.Collections.Generic;

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
using WOA3.Engine;

namespace WOA3.Model.Display {
	public class GameDisplay : IRenderable {
		#region Class variables
		private ContentManager content;
		private BoundingBox boundary;

		private Goof chaser;
		private List<Box> walls = new List<Box>();
		private List<Ghost> allGhosts = new List<Ghost>();
		private List<Ghost> selectedGhosts = new List<Ghost>();
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameDisplay(GraphicsDevice graphics, ContentManager content) {
			this.content = content;
			init(true);
		}
		#endregion Constructor

		#region Support methods
		private void init(bool fullRegen = false) {
			Vector3 min = new Vector3(0f);
			Vector3 max = new Vector3(Constants.RESOLUTION_X, Constants.RESOLUTION_Y, 0f);
			this.boundary = new BoundingBox(min, max);
			this.allGhosts.Add(new Ghost(content, new Vector2(100f)));
			this.allGhosts.Add(new Ghost(content, new Vector2(700f)));
			this.selectedGhosts.Add(this.allGhosts[0]);
			this.chaser = new Goof(content, new Vector2(500f), null);// this.orb);

			for (int i = 1; i < 50; i++) {
				this.walls.Add(new Box(content, new Vector2((i * 12) + 200f, (i * 12f) + 100f)));
			}

			if (fullRegen) {
				/*this.backGround = new BackGround(this.content);
				this.hud = new HUD(this.content);
				this.foodManager = new FoodManager(content, this.rand);
				this.portals = new PortalManager(content, this.rand);
				this.walls = new WallManager(content, this.rand);*/
			}


#if DEBUG
			Debug.debugLine = LoadingUtils.load<Texture2D>(this.content, "Chip");
#endif
		}

		private struct ClosestSeeable {
			public Ghost Ghost { get; set; }
			public float Distance { get; set; }
		}

		public void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == GameState.Active) {
				this.chaser.update(elapsed);

				SoundManager.getInstance().update();


				// cast a ray from our chaser to the target. If this ray hits the target, test it against all other objects
				Nullable<ClosestSeeable> closestSeeable = null;
				foreach (var ghost in allGhosts) {
					ghost.update(elapsed);
					Vector2 direction = Vector2.Subtract(ghost.Position, chaser.Position);
					Nullable<float> distanceToTarget = castRay(ghost, chaser.Position, direction);


					if (distanceToTarget != null) {
						bool canSee = true;
						foreach (Box wall in walls) {
							Nullable<float> distance = castRay(wall, chaser.Position, direction);
							if (distance != null && distance < distanceToTarget) {
								canSee = false;
							}
						}
						if (canSee) {
							if (closestSeeable == null || ((ClosestSeeable)closestSeeable).Distance > distanceToTarget) {
								closestSeeable = new ClosestSeeable() { Ghost = ghost, Distance = (float)distanceToTarget };
							}
						}


						/*Vector2 distanceDelta = Vector2.Subtract(ghost.Position, chaser.Position);
						double angle = Math.Atan2(distanceDelta.X, distanceDelta.Y);
						if (InputManager.getInstance().wasKeyPressed(Keys.D1)) {
							SkillResult result =boo.perform(ghost.Position, angle);
							if (result != null) {
								((Scareable)chaser).scare(result.Damage);
							} else {
								Console.WriteLine("I can't use that right now");
							}
						}*/
						//Console.WriteLine("Angle: " + angle);
					}
				} if (closestSeeable == null) {
					this.chaser.lostTarget();
				} else {
					this.chaser.trackTarget(((ClosestSeeable)closestSeeable).Ghost);
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
			if (InputManager.getInstance().wasKeyPressed(Keys.NumPad0)) {
				Debug.debugOn = !Debug.debugOn;
			}
#endif
		}

		private Nullable<float> castRay(Entity entity, Vector2 position, Vector2 direction) {
			Ray ray = new Ray(new Vector3(position, 0f), new Vector3(direction, 0f));
			return ray.Intersects(entity.BBox);
		}

		public void render(SpriteBatch spriteBatch) {
			foreach (var ghost in this.allGhosts) {
				ghost.render(spriteBatch);
			}
			this.chaser.render(spriteBatch);

			foreach (Box box in this.walls) {
				box.render(spriteBatch);
			}

#if DEBUG
			if (Debug.debugOn) {
				DebugUtils.drawBoundingBox(spriteBatch, this.boundary, Constants.DEBUG_BBOX_Color, Debug.debugLine);
			}
#endif
		}
		#endregion Support methods
	}
}
