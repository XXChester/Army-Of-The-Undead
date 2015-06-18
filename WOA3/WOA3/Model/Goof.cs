
using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Logic;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public class Goof : Entity, Scareable {
		public enum State { Tracking, LostTarget }
		#region Class variables
		private Tracking activeBehaviour;
		private Tracking seekingBehaviour;
		private LostTarget lostTargetBehaviour;
		private Entity tracking;
		private State previousState;
		private Text2D scaredText;

		private const float SPEED = 1f;
		#endregion Class variables

		#region Class propeties
		public Vector2 LastKnownLocation { get; set; }
		public State CurrentState { get; set; }
		public ScaredFactor Scared { get; set; }
		#endregion Class properties

		#region Constructor
		public Goof(ContentManager content, Vector2 position, Entity tracking)
			:base(content) {

			this.tracking = tracking;
			this.Scared = new ScaredFactor();
			
			Base2DSpriteDrawable character = getCharacterSprite(content, position);
			createScaredText(position);

			base.init(character);
			
			this.seekingBehaviour = new Tracking(position, SPEED);
			this.lostTargetBehaviour = new LostTarget(this.seekingBehaviour.Position, this.seekingBehaviour.Target, SPEED);
			this.activeBehaviour = this.seekingBehaviour;
			this.CurrentState = State.Tracking;
		}

		private Base2DSpriteDrawable getCharacterSprite(ContentManager content, Vector2 position) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, "player1");

			StaticDrawable2DParams characterParams = new StaticDrawable2DParams {
				Position = getTextPosition(position),
				Texture = texture,
				Scale = new Vector2(.5f),
				Origin = new Vector2(Constants.TILE_SIZE)
			};
			return new StaticDrawable2D(characterParams);
		}

		private Text2D createScaredText(Vector2 position) {
			Text2DParams textParams = new Text2DParams() {
				Position = position,
				LightColour = Constants.TEXT_COLOUR,
				WrittenText = this.Scared.Text,
				Font = Constants.FONT
			};
			this.scaredText = new Text2D(textParams);
			return this.scaredText;
		}
		#endregion Constructor

		#region Support methods
		private Vector2 getTextPosition(Vector2 position) {
			return Vector2.Subtract(position, new Vector2(0f, 40f));
		}
		private void swapBehaviours(Tracking newBehaviour, State state) {
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

		private void fieldOfView() {
			// Point + Direction * T
			
			//Ray x = base.
		}

		public void scare(float amount) {
			this.Scared.scare(amount);
			this.scaredText.WrittenText = this.Scared.Text;
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
		}
		#endregion Support methods
	}
}
