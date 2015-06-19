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
		}

		protected override IRenderable createInstance() {
			return new GameDisplay(device, content, "Map" + stateMachine.LevelContext.MapIndex, stateMachine);
		}

		public override void goToPreviousState() {
			changeState(stateMachine.MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			LevelContext context = stateMachine.LevelContext;
			context.MapIndex += 1;
			stateMachine.LevelContext = context;
			changeState(stateMachine.GameDisplay);
			base.goToNextState();
		}

		public void goToGameOver() {
			LevelContext context = stateMachine.LevelContext;
			context.MapIndex = 1;
			stateMachine.LevelContext = context;
			changeState(stateMachine.GameOverState);
		}
	}
}
