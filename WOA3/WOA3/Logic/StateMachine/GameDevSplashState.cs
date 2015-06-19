﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;


namespace WOA3.Logic.StateMachine {
	class GameDevSplashState : BaseGameState {

		public GameDevSplashState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content)
			: base(stateMachine, device, content,null) {
		
		}

		protected override IRenderable createInstance() {
			return new Cinematic(content, "SplashScreen");
		}

		public override void goToPreviousState() {
			changeState(stateMachine.MainMenu);
			base.goToPreviousState();
		}

		public override void goToNextState() {
			goToPreviousState();
			base.goToPreviousState();
		}
	}
}
