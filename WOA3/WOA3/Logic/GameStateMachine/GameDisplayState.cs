using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.GameStateMachine {
	class GameDisplayState : BaseGameState {

		public GameDisplayState( GraphicsDevice device,  ContentManager content)
			: base(device, content, null) {
		}

		protected override IRenderable createInstance() {
			if (GameStateMachine.getInstance().LevelContext == null) {
				resetContext();
			}
		//	return new GameDisplay(device, content, "Map" + GameStateMachine.getInstance().LevelContext.MapIndex);
			return new GameDisplay(device, content, "Pathing");
			//return new GameDisplay(device, content, "Map4");
		}

		public override void goToPreviousState() {
			changeState(GameStateMachine.getInstance().MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			LevelContext context = GameStateMachine.getInstance().LevelContext;
			context.MapIndex += 1;
			if (context.MapIndex == 5) {
				resetContext();
				changeState(GameStateMachine.getInstance().GameFinishedState);
			} else {
				changeState(GameStateMachine.getInstance().GameDisplay);
				GameStateMachine.getInstance().LevelContext = context;
			}
			// don't call the base or the context will be reset
			//base.goToNextState();
		}

		public void goToGameOver() {
			changeState(GameStateMachine.getInstance().GameOverState);
		}

		public override void reset() {
			base.reset();
		}
	}
}
