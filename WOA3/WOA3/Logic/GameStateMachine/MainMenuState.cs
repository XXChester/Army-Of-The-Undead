using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.GameStateMachine {
	class MainMenuState : BaseGameState {

		public MainMenuState(GraphicsDevice device, ContentManager content)
			: base(device, content, null) {
			
		}

		protected override IRenderable createInstance() {
			return new MenuDisplay(content);
		}

		public void pushToTutorial() {
			changeState(GameStateMachine.getInstance().Tutorial);
			base.goToPreviousState();
		}

		public override void goToPreviousState() {
			changeState(GameStateMachine.getInstance().ExitState);
		}
		public override void goToNextState() {
			changeState(GameStateMachine.getInstance().GameDisplay);
			base.goToNextState();
		}
	}
}
