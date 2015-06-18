using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.StateMachine {
	class GameDisplayState : BaseGameState {

		public GameDisplayState(GameStateMachine stateMachine, GraphicsDevice device,  ContentManager content)
			: base(stateMachine, device, content, null) {
				reset();
		}

		private IRenderable createInstance() {
			return new GameDisplay(device, content, "Test");
		}

		public override void goToNextState() {
			changeState(stateMachine.GameDevCinematic);
		}

		public override void setStates() {
			StateManager.getInstance().CurrentGameState = GameState.Active;
		}

		public override void reset() {
			if (this.display != null) {
				this.display.dispose();
			}
			this.display = createInstance();
#if DEBUG
			Console.Clear();
#endif
		}
	}
}
