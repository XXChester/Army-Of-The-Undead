using System.Collections.Generic;
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
	public class TutorialComplete : BaseMenu {
		#region Class variables
		private float elapsedWaitTime;
		private GameStateMachine gameStateMachine;
		private List<TexturedEffectButton> buttons;

		private readonly string[] BUTTON_NAMES = { "Play", "Menu"};
		private const float WAIT_TIME = 1500f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialComplete(ContentManager content, GameStateMachine gameStateMachine) : base(content, "TutorialComplete", new Vector2(500f)) {
			float xBuffer = 256;
			float yBuffer = 128;
			float leftSideX = Constants.RESOLUTION_X - xBuffer;
			float y = Constants.RESOLUTION_Y - yBuffer;

			this.gameStateMachine = gameStateMachine;
			this.buttons = new List<TexturedEffectButton>();
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(leftSideX, y), BUTTON_NAMES[0]));
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(xBuffer, y), BUTTON_NAMES[1]));
			/*Texture2D texture = LoadingUtils.load<Texture2D>(content, "TutorialComplete");
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 2)
			};
			this.cinematic = new StaticDrawable2D(parms);*/
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			if (StateManager.getInstance().CurrentTransitionState == TransitionState.None) {
				this.elapsedWaitTime += elapsed;
				if (this.elapsedWaitTime >= WAIT_TIME) {
					StateManager.getInstance().CurrentGameState = GameState.MainMenu;
				}
			}
			if (StateManager.getInstance().CurrentTransitionState == TransitionState.None || StateManager.getInstance().CurrentTransitionState == TransitionState.TransitionIn) {
				if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = GameState.MainMenu;
				}
			}

			foreach (TexturedEffectButton button in this.buttons) {
				button.update(elapsed);
				button.processActorsMovement(InputManager.getInstance().MousePosition);
			}
			if (InputManager.getInstance().wasLeftButtonPressed()) {
				foreach (TexturedEffectButton button in this.buttons) {
					if (button.isActorOver(InputManager.getInstance().MousePosition)) {
						// we clicked a button
						if (button.Texture.Name.Equals(BUTTON_NAMES[0])) {
							this.gameStateMachine.LevelContext = null;
							this.gameStateMachine.goToNextState();
						} else if (button.Texture.Name.Equals(BUTTON_NAMES[1])) {
							this.gameStateMachine.goToPreviousState();
						}
						break;
					}
				}
			}
		}

		public  override void render(SpriteBatch spriteBatch) {
			foreach (TexturedEffectButton button in this.buttons) {
				button.render(spriteBatch);
#if DEBUG
				if (Debug.debugOn) {
					DebugUtils.drawRectangle(spriteBatch, button.PickableArea, Debug.DEBUG_BBOX_Color, Debug.debugChip);
				}
#endif
			}
			base.render(spriteBatch);
		}
		#endregion Support methods
	}
}
