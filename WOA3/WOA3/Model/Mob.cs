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
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public abstract class Mob : Character, IObserver<Ghost> {
		public enum State { Tracking, Pathing, Idle }
		#region Class variables
		private TargetBehaviour activeBehaviour;
		private TargetBehaviour seekingBehaviour;
		private TargetBehaviour lostTargetBehaviour;
		private TargetBehaviour idleBehaviour;
		private Pathing pathingBehaviour;
		private Entity tracking;
		private State state;
		private State previousState;
		private IDisposable unsubscriber;
		protected List<Skill> skills;
		private Point previousPoint;
		private SoundEffect explosionSfx;

		private const float SPEED = .05f;//.1f;
		#endregion Class variables

		#region Class propeties
		public Vector2 LastKnownLocation { get; set; }
		public float CorpseExplosionDamage { get { return 3; } }
		public BoundingSphere BoundingSphere { get; set; }
		public bool Inactive { get; set; }
		public State CurrentState {
			get { return this.state; }
			set {
				this.previousState = this.state;
				this.state = value;
			}
		}
		#endregion Class properties

		#region Constructor
		public Mob(ContentManager content, Vector2 position, CharactersInRange charactersInRange, OnDeath onDeath, CollisionCheck collisionCheck, String monsterName)
			:base(content, position, SPEED, charactersInRange, onDeath, 10f) {
			
			StaticDrawable2D character = getCharacterSprite(content, position, monsterName);
			base.init(character);

			BehaviourFinished idleCallback = delegate() {
				swapBehaviours(this.idleBehaviour, State.Idle);
			};
			BehaviourFinished restartPathing = delegate() {
#if DEBUG
				Debug.log("Restarting");
#endif
				//this.activeBehaviour.Target = this.LastKnownLocation;
				pathToWaypoint(this.LastKnownLocation);
			};
			this.seekingBehaviour = new Tracking(position, SPEED, idleCallback, collisionCheck);
			this.lostTargetBehaviour = new LostTarget(this.seekingBehaviour.Position, this.seekingBehaviour.Position, SPEED, idleCallback);
			this.pathingBehaviour = new Pathing(position, SPEED, idleCallback, collisionCheck, restartPathing);
			this.idleBehaviour = new IdleBehaviour(position);
			this.activeBehaviour = this.idleBehaviour;
			this.CurrentState = State.Idle;
			updateBoundingSphere();
			this.previousPoint = base.Position.toPoint();
			this.LastKnownLocation = base.Position;
			
			this.skills = new List<Skill>();
			this.explosionSfx = LoadingUtils.load<SoundEffect>(content, "CorpseExplosion");
			initSkills();
		}
		private void updateBoundingSphere() {
				this.BoundingSphere = new BoundingSphere(new Vector3(Position, 0f), Constants.BOUNDING_SPHERE_SIZE);
		}

		private StaticDrawable2D getCharacterSprite(ContentManager content, Vector2 position, String monsterName) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, monsterName);

			StaticDrawable2DParams characterParams = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2(Constants.TILE_SIZE/2),
			};
			return new StaticDrawable2D(characterParams);
		}
		#endregion Constructor

		#region Support methods
		protected abstract void initSkills();
		private void swapBehaviours(TargetBehaviour newBehaviour, State state) {
			//if (!state.Equals(previousState)) {
				Debug.log("state changed from: " + previousState + "\t to: " + state);
				// reset the last known location ONLY if we were tracking
				if (isTracking()) {
					this.LastKnownLocation = this.activeBehaviour.Target;
					//this.tracking = null;
				}
				newBehaviour.Target = this.LastKnownLocation;
				newBehaviour.Position = this.activeBehaviour.Position;
				this.activeBehaviour = newBehaviour;
				this.CurrentState = state;
		//	}
			if (!state.Equals(State.Pathing)) {
			//	this.pathingBehaviour.stop();
			}
		}

		private void trackTarget(Entity toTrack) {
			//if (!toTrack.Equals(tracking)) {
				this.tracking = toTrack;
				swapBehaviours(this.seekingBehaviour, State.Tracking);
			//pathToWaypoint(toTrack.Position);
		//	}
		}


		/*public void lostTarget() {
			swapBehaviours(this.lostTargetBehaviour, State.LostTarget);
		}*/

		public void idle() {
			swapBehaviours(this.idleBehaviour, State.Idle);
		}

		public void pathToWaypoint(Vector2 lastKnowPosition) {
			// if the last known location isn't the same as our currnet LKL, re-calculate
			if (!lastKnowPosition.Equals(LastKnownLocation)) {
				//this.activeBehaviour.Target = this.LastKnownLocation;
				this.LastKnownLocation = lastKnowPosition;
				this.pathingBehaviour.init(base.Position, this.LastKnownLocation);
				swapBehaviours(this.pathingBehaviour, State.Pathing);

				/*this.CurrentState = State.Pathing;
				this.pathingBehaviour.init(base.Position, this.activeBehaviour.Target);
				this.activeBehaviour = this.pathingBehaviour;*/
			}
		}

		public void collision() {
				this.pathingBehaviour.init(base.Position, this.LastKnownLocation);
				swapBehaviours(this.pathingBehaviour, State.Pathing);
		}


	/*	public void stop() {
#if DEBUG
			Debug.log("Stoppping");
#endif
			this.activeBehaviour.Target = this.Position;
			this.CurrentState = State.Stopped;
		}

		public bool isStopped() {
			return State.Stopped.Equals(this.CurrentState);
		}
	 	
		public bool isLost() {
			return State.LostTarget.Equals(this.CurrentState);
		}
		*/

		public bool isPathing() {
			return State.Pathing.Equals(this.CurrentState) && this.pathingBehaviour.Targets.Count > 0;
		}

		public bool isTracking() {
			return State.Tracking.Equals(this.CurrentState);
		}

		public bool isIdle() {
			return State.Idle.Equals(this.CurrentState);
		}

		protected override Skill getDeathSkill() {
			return new CorpseExplode(this.explosionSfx, CorpseExplosionDamage);
		}

		public override bool isVisible() {
			return true;
		}

		public override List<SkillResult> performSkills() {
				List<SkillResult> results = new List<SkillResult>();
				if (!Inactive) {
					if (Constants.ALLOW_MOB_ATTACKS) {
						List<Character> charactersInRange = this.CharactersInRange.Invoke(this.Range);
						if (charactersInRange.Count > 0) {
							foreach (var skill in skills) {
								if (skill.CoolDownOver) {
									CombatManager.getInstance().CombatRequests.Add(new CombatRequest() {
										Skill = skill,
										Source = this,
										Targets = charactersInRange
									});
								}
							}
						}
					}
				}
			return results;
		}

		public override void update(float elapsed) {
			if (!Inactive) {
				base.update(elapsed);

				//Problem is the setting of the last known location when we switch behaviours
#if DEBUG
				if (Debug.mobsCanMove) {
#endif
					//if (!isIdle()) {
						// if we are tracking, update our target to the current prey's location
						if (this.tracking != null) {
							this.seekingBehaviour.Target = this.tracking.Position;
						}
						this.activeBehaviour.update(elapsed);
						base.Position = this.activeBehaviour.Position;
				//	}
					updateBoundingSphere();
#if DEBUG
				}
#endif

				// update our skills
				foreach (var skill in skills) {
					skill.update(elapsed);
				}

				/*if (GWNorthEngine.Input.InputManager.getInstance().wasRightButtonPressed()) {
					pathToWaypoint(GWNorthEngine.Input.InputManager.getInstance().MousePosition);
				}*/
				/*if (!isPathing() && !isStopped() && activeBehaviour.Target.Equals(activeBehaviour.Position)) {
					stop();
				}*/

				//this.previousState = CurrentState;
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			if (!Inactive) {
				base.render(spriteBatch);

#if DEBUG
				if (Debug.debugOn) {
					BoundingBox bbox = CollisionGenerationUtils.getBBoxHalf(this.activeBehaviour.Target);
					//BoundingBox bbox = CollisionGenerationUtils.getBBox(this.activeBehaviour.Target);
					DebugUtils.drawBoundingBox(spriteBatch, bbox, Color.Green, Debug.debugChip);
					DebugUtils.drawBoundingSphere(spriteBatch, BoundingSphere, Color.Pink, Debug.debugRing);
					if (this.pathingBehaviour.Targets.Count > 0) {
						bbox = CollisionGenerationUtils.getBBox(this.pathingBehaviour.EndTarget);
						DebugUtils.drawBoundingBox(spriteBatch, bbox, Color.Orange, Debug.debugChip);
					}
				}
				if (GWNorthEngine.Input.InputManager.getInstance().wasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space)) {
					Debug.log("Type: " + this.activeBehaviour + "\tpos: " + this.activeBehaviour.Position);
				}
#endif
			}
		}

		public void Subscribe(GhostObservationHandler provider, Ghost ghost) {
			this.unsubscriber = provider.Subscribe(this, ghost);
			this.trackTarget(ghost);
		}

		public virtual void Unsubscribe() {
			Unsubscribe(this.LastKnownLocation);
		}

		public virtual void Unsubscribe(Vector2 lastKnowPosition) {
			if (this.unsubscriber != null) {
				this.unsubscriber.Dispose();
			}
			//this.lostTarget();
			/*if (this.tracking != null) {
				this.LastKnownLocation = this.tracking.Position;
			}*/
			// if the last known location isn't the same as our currnet LKL, re-calculate
			//if (!lastKnowPosition.Equals(LastKnownLocation)) {
				this.pathToWaypoint(lastKnowPosition);
			//}
			this.tracking = null;
		}

		public void OnCompleted() {
			//this.stop();
			this.idle();
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
					this.pathToWaypoint(ghost.Position);
				}
			}
		}
		#endregion Support methods
	}
}
