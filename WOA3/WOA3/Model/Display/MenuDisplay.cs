﻿using System;
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
using WOA3.Model.Skills;
using WOA3.Logic.AI;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Logic.StateMachine;


namespace WOA3.Model.Display {
public 	class MenuDisplay : BaseMenu {
		#region Class variables
		private GameStateMachine stateMachine;

		private List<TexturedEffectButton> buttons;
		private readonly string[] BUTTON_NAMES = { "Tutorial", "PlayGame", "Exit" };
		private readonly Color DEFAULT = Color.Red;
		private readonly Vector2 DEFAULT_SCALE = new Vector2(1f, .75f);
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public MenuDisplay(ContentManager content, GameStateMachine stateMachine)
			: base(content, "Monster1", new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 8 * 3)) {
				this.stateMachine = stateMachine;
			

				float leftSideX = Constants.RESOLUTION_X / 2 - 63;
				Vector2 position = new Vector2(leftSideX, 475f);

				this.buttons = new List<TexturedEffectButton>();
				for (int i = 0; i < this.BUTTON_NAMES.Length; i++) {
					this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(position.X, (position.Y + 75f * i)), BUTTON_NAMES[i]));
				}
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			base.update(elapsed);
			foreach (TexturedEffectButton button in this.buttons) {
				button.update(elapsed);
				button.processActorsMovement(InputManager.getInstance().MousePosition);
			}
			if (InputManager.getInstance().wasLeftButtonPressed()) {
				MainMenuState state = (MainMenuState)this.stateMachine.CurrentState;
				foreach (TexturedEffectButton button in this.buttons) {
					if (button.isActorOver(InputManager.getInstance().MousePosition)) {
						// we clicked a button
						if (button.Texture.Name.Equals(BUTTON_NAMES[0])) {
							state.pushToTutorial();
						} else if (button.Texture.Name.Equals(BUTTON_NAMES[1])) {
							this.stateMachine.goToNextState();
						} else if (button.Texture.Name.Equals(BUTTON_NAMES[2])) {
							this.stateMachine.goToPreviousState();
						}
						break;
					}
				}
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			foreach (TexturedEffectButton button in this.buttons) {
				button.render(spriteBatch);
#if DEBUG
				if (Debug.debugOn) {
					DebugUtils.drawRectangle(spriteBatch, button.PickableArea, Debug.DEBUG_BBOX_Color, Debug.debugChip);
				}
#endif
			}
		}
		#endregion Support methods
	}
}
