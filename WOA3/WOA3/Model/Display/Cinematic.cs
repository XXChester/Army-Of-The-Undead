
using GWNorthEngine.Input;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.StateMachine;

namespace WOA3.Model.Display {
	public class Cinematic : IRenderable {
		#region Class variables
		private StaticDrawable2D cinematic;
		private float elapsedWaitTime;
		private const float WAIT_TIME = 3000f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Cinematic(ContentManager content, string imageName) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, imageName);
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 2)
			};
			this.cinematic = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {
			//if (StateManager.getInstance().CurrentTransitionState == TransitionState.None) {
				this.elapsedWaitTime += elapsed;
				if (this.elapsedWaitTime >= WAIT_TIME) {
					GameStateMachine.getInstance().CurrentState.goToNextState();
				}
			//}
			/*if (StateManager.getInstance().CurrentTransitionState == TransitionState.None || StateManager.getInstance().CurrentTransitionState == TransitionState.TransitionIn) {
				if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = GameState.MainMenu;
				}
			}*/
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.cinematic != null) {
				this.cinematic.render(spriteBatch);
			}
		}
		#endregion Support methods


		public void dispose() {
			cinematic.dispose();
		}
	}
}
