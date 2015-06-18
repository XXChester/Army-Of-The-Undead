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
	public class Mob : Character, IObserver<Ghost> {
		public enum State { Tracking, LostTarget, Stopped, Pathing, Idle }
		#region Class variables
		private TargetBehaviour activeBehaviour;
		private TargetBehaviour seekingBehaviour;
		private TargetBehaviour lostTargetBehaviour;
		private Pathing pathingBehaviour;
		private Entity tracking;
		private State previousState;
		private IDisposable unsubscriber;
		private List<Skill> skills;
		private OnDeath onDeath;

		private const float SPEED = .1f;
		#endregion Class variables

		#region Class propeties
		public Vector2 LastKnownLocation { get; set; }
		public State CurrentState { get; set; }
		public float CorpseExplosionDamage { get { return 3; } }
		#endregion Class properties

		#region Constructor
		public Mob(ContentManager content, Vector2 position, CharactersInRange charactersInRange, OnDeath onDeath)
			:base(content, position, SPEED, charactersInRange) {
			
			StaticDrawable2D character = getCharacterSprite(content, position);
			base.init(character);

			BehaviourFinished idleCallback = delegate() {
				Debug.log("Idling");
				this.CurrentState = State.Idle;
			};
			this.seekingBehaviour = new Tracking(position, SPEED, idleCallback);
			this.lostTargetBehaviour = new LostTarget(this.seekingBehaviour.Position, this.seekingBehaviour.Position, SPEED, idleCallback);
			this.pathingBehaviour = new Pathing(position, SPEED, idleCallback);
			this.activeBehaviour = this.seekingBehaviour;
			this.CurrentState = State.Idle;
			this.onDeath = onDeath;
			
			this.skills = new List<Skill>();
			this.skills.Add(new HolySwirl(2f));
		}

		private StaticDrawable2D getCharacterSprite(ContentManager content, Vector2 position) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Ghost");

			StaticDrawable2DParams characterParams = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2(Constants.TILE_SIZE/2),
				LightColour = Color.Red
			};
			return new StaticDrawable2D(characterParams);
		}
		#endregion Constructor

		#region Support methods
		private void swapBehaviours(TargetBehaviour newBehaviour, State state) {
			if (!state.Equals(previousState)) {
#if DEBUG
				Debug.log("old: " + previousState + "\tnew: " + state + "\tLKL: " + LastKnownLocation);
#endif
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

		public void pathToWaypoint() {
			if (!isPathing()) {
				this.pathingBehaviour.init(base.Position, this.LastKnownLocation);
				swapBehaviours(this.pathingBehaviour, State.Pathing);
			}
		}

		public void stop() {
#if DEBUG
			Debug.log("Stoppping");
#endif
			this.activeBehaviour.Target = this.Position;
			this.CurrentState = State.Stopped;
		}

		public bool isStopped() {
			return State.Stopped.Equals(this.CurrentState);
		}

		public bool isPathing() {
			return State.Pathing.Equals(this.CurrentState);
		}

		public bool isIdle() {
			return State.Idle.Equals(this.CurrentState);
		}

		public override Skill die() {
			if (this.onDeath != null) {
				this.onDeath.Invoke(base.Position);
			}
			return new CorpseExplode(CorpseExplosionDamage);
		}

		public override List<SkillResult> performSkills() {
			List<SkillResult> results = new List<SkillResult>();
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
			return results;
		}

		public override void update(float elapsed) {
			base.update(elapsed);

			if (!isIdle()) {
				// if we are tracking, update our target to the current prey's location
				if (this.tracking != null) {
					this.seekingBehaviour.Target = this.tracking.Position;
				}
				this.activeBehaviour.update(elapsed);
				base.Position = this.activeBehaviour.Position;
			}

			/*if (GWNorthEngine.Input.InputManager.getInstance().wasRightButtonPressed()) {
				this.activeBehaviour.Target = GWNorthEngine.Input.InputManager.getInstance().MousePosition;
				swapBehaviours(this.seekingBehaviour, State.Tracking);
			}*/
			/*if (!isPathing() && !isStopped() && activeBehaviour.Target.Equals(activeBehaviour.Position)) {
				stop();
			}*/

			this.previousState = CurrentState;
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			
#if DEBUG
			if (Debug.debugOn) {
				if (!isStopped()) {
					BoundingBox bbox = CollisionGenerationUtils.getBBoxHalf(this.activeBehaviour.Target);
					DebugUtils.drawBoundingBox(spriteBatch, bbox, Color.Green, Debug.debugChip);
				}
			}
			if (GWNorthEngine.Input.InputManager.getInstance().wasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space)) {
				Debug.log("Type: " + this.activeBehaviour +"\tpos: " + this.activeBehaviour.Position);
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
