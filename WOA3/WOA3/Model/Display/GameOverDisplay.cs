using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;

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
using WOA3.Model.Scenarios;

namespace WOA3.Model.Display {
	public class GameOverDisplay : BaseMenu {
		#region Class variables
		private GameStateMachine gameStateMachine;
		private List<TexturedEffectButton> buttons;

		private readonly string[] BUTTON_NAMES = { "Restart", "Menu" };
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameOverDisplay(GraphicsDevice graphics, ContentManager content, GameStateMachine gameStateMachine): base(content, "Monster1", new Vector2()) {			
			float xBuffer = 256;
			float yBuffer = 128;
			float leftSideX = Constants.RESOLUTION_X  - xBuffer;
			float y = Constants.RESOLUTION_Y - yBuffer;

			this.gameStateMachine = gameStateMachine;
			this.buttons = new List<TexturedEffectButton>();
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(leftSideX,y), BUTTON_NAMES[0]));
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(xBuffer, y), BUTTON_NAMES[1]));
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			foreach (TexturedEffectButton button in this.buttons) {
				button.update(elapsed);
				button.processActorsMovement(InputManager.getInstance().MousePosition);
			}
			if (InputManager.getInstance().wasLeftButtonPressed()) {
				foreach (TexturedEffectButton button in this.buttons) {
					if (button.isActorOver(InputManager.getInstance().MousePosition)) {
						// we clicked a button
						if (button.Texture.Name.Equals(BUTTON_NAMES[0])) {
							this.gameStateMachine.goToPreviousState();
						} else if (button.Texture.Name.Equals(BUTTON_NAMES[1])) {
							this.gameStateMachine.goToNextState();
						}
						break;
					}
				}
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
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
