using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.GameStateMachine {
	class ExitGameState : BaseGameState {

		public ExitGameState(GraphicsDevice device, ContentManager content)
			: base(device, content,null) {
		}
		
		protected override IRenderable createInstance() {
			return this.display;
		}
	}
}
