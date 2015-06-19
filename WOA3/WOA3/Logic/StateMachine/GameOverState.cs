using System;
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
			LevelContext context = new LevelContext() {
				Ghosts = new List<Model.Ghost>(),
				MapIndex = 1
			}; 
			this.stateMachine.LevelContext = context;
			changeState(stateMachine.GameDisplay);
			base.goToPreviousState();
		}
	}
}
