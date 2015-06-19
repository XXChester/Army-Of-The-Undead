﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.StateMachine {
	class GameOverState : BaseGameState {

		public GameOverState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content, null) {
		}

		protected override IRenderable createInstance() {
			return new GameOverDisplay(device, content, this.stateMachine);
		}

		public override void goToNextState() {
			changeState(stateMachine.MainMenu);
			base.goToNextState();
		}
		public override void goToPreviousState() {
			changeState(stateMachine.GameDisplay);
			base.goToPreviousState();
		}

		public override void setStates() {
			StateManager.getInstance().CurrentGameState = GameState.Active;
		}
	}
}