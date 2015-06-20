using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.StateMachine {
	class GameDisplayState : BaseGameState {

		public GameDisplayState( GraphicsDevice device,  ContentManager content)
			: base(device, content, null) {
		}

		protected override IRenderable createInstance() {
			if (GameStateMachine.getInstance().LevelContext == null) {
				LevelContext context = new LevelContext() {
					Ghosts = new List<Model.Ghost>(),
					MapIndex = 1
				};
				GameStateMachine.getInstance().LevelContext = context;
			}
			return new GameDisplay(device, content, "Map" + GameStateMachine.getInstance().LevelContext.MapIndex);
		}

		public override void goToPreviousState() {
			changeState(GameStateMachine.getInstance().MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			LevelContext context = GameStateMachine.getInstance().LevelContext;
			context.MapIndex += 1;
			GameStateMachine.getInstance().LevelContext = context;
			changeState(GameStateMachine.getInstance().GameDisplay);
			base.goToNextState();
		}

		public void goToGameOver() {
			changeState(GameStateMachine.getInstance().GameOverState);
		}

		public override void reset() {
			base.reset();
		}
	}
}
