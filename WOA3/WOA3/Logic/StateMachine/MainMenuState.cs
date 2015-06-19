using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.StateMachine {
	class MainMenuState : BaseGameState {

		public MainMenuState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content, null) {
			
		}

		protected override IRenderable createInstance() {
			return new MenuDisplay(content, stateMachine);
		}

		public void pushToTutorial() {
			changeState(stateMachine.Tutorial);
			WOA3.Logic.StateManager.getInstance().CurrentGameState = GameState.Active;
			base.goToPreviousState();
		}

		public override void goToPreviousState() {
			StateManager.getInstance().CurrentGameState = GameState.Exit;
		}
		public override void goToNextState() {
			changeState(stateMachine.GameDisplay);
			WOA3.Logic.StateManager.getInstance().CurrentGameState = GameState.Active;
			base.goToNextState();
		}
	}
}
