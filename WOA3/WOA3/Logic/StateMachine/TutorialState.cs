using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.StateMachine {
	class TutorialState : BaseGameState {
		public TutorialState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content, null) {
		}

		protected override IRenderable createInstance() {
			return new TutorialDisplay(device, content, this.stateMachine);
		}

		public override void goToPreviousState() {
			changeState(stateMachine.MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			changeState(stateMachine.TutorialComplete);
			base.goToNextState();
		}

		public override void setStates() {
			StateManager.getInstance().CurrentGameState = GameState.Active;
		}
	}
}
