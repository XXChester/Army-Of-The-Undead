﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.StateMachine {
	class TutorialCompleteState : BaseGameState {

		public TutorialCompleteState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content, new TutorialComplete(content)) {
		}

		public override void goToNextState() {
			changeState(stateMachine.GameDisplay);
		}

		public override void setStates() {
			StateManager.getInstance().CurrentGameState = GameState.Active;
		}
	}
}