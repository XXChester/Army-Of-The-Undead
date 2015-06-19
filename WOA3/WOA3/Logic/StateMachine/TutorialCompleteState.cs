using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.StateMachine {
	class TutorialCompleteState : BaseGameState {

		public TutorialCompleteState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content,null) {
		}
		
		protected override IRenderable createInstance() {
			return new TutorialComplete(content, stateMachine);
		}

		public override void goToPreviousState() {
			changeState(stateMachine.MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			LevelContext context = new LevelContext() {
				Ghosts = new List<Model.Ghost>(),
				MapIndex = 1
			};
			this.stateMachine.LevelContext = context;
			changeState(stateMachine.GameDisplay);
			base.goToNextState();
		}
	}
}
