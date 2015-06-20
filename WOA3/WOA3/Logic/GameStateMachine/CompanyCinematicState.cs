using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.GameStateMachine {
	class CompanyCinematicState : BaseGameState {

		public CompanyCinematicState( GraphicsDevice device, ContentManager content)
			: base( device, content, null) {
			
		}
		
		protected override IRenderable createInstance() {
			return new Cinematic(content, "Logo");
		}

		public override void goToPreviousState() {
			changeState(GameStateMachine.getInstance().MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			goToPreviousState();
			base.goToNextState();
		}
	}
}
