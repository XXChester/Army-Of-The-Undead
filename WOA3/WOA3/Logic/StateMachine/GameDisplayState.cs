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
			return new GameDisplay(device, content, "Tutorial");
		}

		public override void goToPreviousState() {
			changeState(stateMachine.MainMenu);
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
