using System;
using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.AI.AStar.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Engine;
using WOA3.Logic;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public class Goof : Entity, Scareable, IObserver<Ghost> {
		public enum State { Tracking, LostTarget, Stopped, Pathing }
		#region Class variables
		private TargetBehaviour activeBehaviour;
		private TargetBehaviour seekingBehaviour;
		private TargetBehaviour lostTargetBehaviour;
		private TargetBehaviour pathingBehaviour;
		private Entity tracking;
		private State previousState;
		private Text2D scaredText;
		private IDisposable unsubscriber;

		private const float SPEED = .1f;
		#endregion Class variables

		#region Class propeties
		public Vector2 LastKnownLocation { get; set; }
		public State CurrentState { get; set; }
		public ScaredFactor Scared { get; set; }
		#endregion Class properties

		#region Constructor
		public Goof(ContentManager content, Vector2 position)
			:base(content) {

			this.Scared = new ScaredFactor();
			
			Base2DSpriteDrawable character = getCharacterSprite(content, position);
			createScaredText(position);

			base.init(character);
			
			this.seekingBehaviour = new Tracking(position, SPEED);
			this.lostTargetBehaviour = new LostTarget(this.seekingBehaviour.Position, this.seekingBehaviour.Position, SPEED);
			this.activeBehaviour = this.seekingBehaviour;
			this.CurrentState = State.Tracking;
		}

		private Base2DSpriteDrawable getCharacterSprite(ContentManager content, Vector2 position) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Ghost");

			StaticDrawable2DParams characterParams = new StaticDrawable2DParams {
				Position = getTextPosition(position),
				Texture = texture,
				Origin = new Vector2(Constants.TILE_SIZE/2),
				LightColour = Color.Red
			};
			return new StaticDrawable2D(characterParams);
		}

		private Text2D createScaredText(Vector2 position) {
			Text2DParams textParams = new Text2DParams() {
				Position = position,
				LightColour = Constants.TEXT_COLOUR,
				WrittenText = this.Scared.Text,
				Origin = new Vector2(Constants.TILE_SIZE / 2),
				Font = Constants.FONT
			};
			this.scaredText = new Text2D(textParams);
			return this.scaredText;
		}
		#endregion Constructor

		#region Support methods
		private Vector2 getTextPosition(Vector2 position) {
			return Vector2.Subtract(position, new Vector2(-6f, Constants.TILE_SIZE));
		}
		private void swapBehaviours(TargetBehaviour newBehaviour, State state) {
			if (!state.Equals(previousState)) {
				this.LastKnownLocation = this.activeBehaviour.Target;
				newBehaviour.Target = this.LastKnownLocation;
				newBehaviour.Position = this.activeBehaviour.Position;
				this.activeBehaviour = newBehaviour;
				this.CurrentState = state;
			}
		}

		public void lostTarget() {
			swapBehaviours(lostTargetBehaviour, State.LostTarget);
		}

		public void trackTarget(Entity toTrack) {
			this.tracking = toTrack;
			swapBehaviours(seekingBehaviour, State.Tracking);
		}

		public void stop(Wall collisionWith) {
			// if this bounding box is not the same as the bounding box of our target, go to A* to find a path
			Vector2 direction = Vector2.Subtract(collisionWith.Position, Position);
			BoundingBox bbox = CollisionGenerationUtils.getBBox(LastKnownLocation);
			Nullable<float> distanceToTarget = CollisionUtils.castRay(bbox, Position, direction);
			Nullable<float> distanceToWall = CollisionUtils.castRay(collisionWith.BBox, Position, direction);
			if (distanceToTarget != null && distanceToTarget > distanceToWall) {
				AIManager.getInstance().requestPath(LastKnownLocation.toPoint(), delegate(Stack<Point> path) {
					if (this != null) {
						if (path != null) {
							Debug.log("A*ing this bitch");
							this.pathingBehaviour = new Pathing(Position, SPEED, path);
							this.activeBehaviour = this.pathingBehaviour;
							this.CurrentState = State.Pathing;
						} else {
							this.activeBehaviour.Target = this.Position;
						}
					}
				});
#if DEBUG
				Debug.log("We hit a wall but our target is further away so turn to A*");

#endif
			} /*else {
				this.activeBehaviour.Target = this.Position;
			}*/
		}

		public void scare(float amount) {
			this.Scared.scare(amount);
			this.scaredText.WrittenText = this.Scared.Text;
		}

		public bool isStopped() {
			return State.Stopped.Equals(this.CurrentState);
		}

		public override void update(float elapsed) {
			base.update(elapsed);

			if (this.tracking != null) {
				this.seekingBehaviour.Target = this.tracking.Position;
			}
			this.activeBehaviour.update(elapsed);
			base.Position = this.activeBehaviour.Position;
			this.scaredText.Position = getTextPosition(base.Position);
			this.scaredText.update(elapsed);

			this.previousState = CurrentState;
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			this.scaredText.render(spriteBatch);

#if DEBUG
			if (Debug.debugOn && !isStopped()) {
				BoundingBox bbox = CollisionGenerationUtils.getBBoxHalf(this.activeBehaviour.Target);
				DebugUtils.drawBoundingBox(spriteBatch, bbox, Color.Green, Debug.debugChip);
			}
#endif
		}

		public void Subscribe(GhostObservationHandler provider, Ghost ghost) {
			this.unsubscriber = provider.Subscribe(this, ghost);
			this.trackTarget(ghost);
		}

		public virtual void Unsubscribe() {
			this.unsubscriber.Dispose();
			this.lostTarget();
		}

		public void OnCompleted() {
			this.lostTarget();
		}

		public void OnError(Exception error) {
			throw new NotImplementedException();
		}

		public void OnNext(Ghost ghost) {
			// only act on this if it is our ghost that triggered it
			if (this.tracking != null && this.tracking.Equals(ghost)) {
				if (ghost.isVisible()) {
					this.trackTarget(ghost);
				} else {
					this.lostTarget();
				}
			}
		}
		#endregion Support methods
	}
}
