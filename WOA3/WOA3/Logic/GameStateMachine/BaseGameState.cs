using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.GameStateMachine {
	public abstract class BaseGameState : State, IDisposable {

		protected GraphicsDevice device;
		protected ContentManager content;
		protected IRenderable display;

		public BaseGameState(GraphicsDevice device, ContentManager content, IRenderable display) {
			this.device = device;
			this.content = content;
			this.display = display;
		}

		protected abstract IRenderable createInstance();

		protected void changeState(State newState) {
			setStates();
			GameStateMachine.getInstance().CurrentState = newState;
			GameStateMachine.getInstance().CurrentState.reset();
		}

		protected void resetContext() {
			LevelContext context = new LevelContext() {
				Ghosts = new List<Model.Ghost>(),
				MapIndex = 1
			};
			GameStateMachine.getInstance().LevelContext = context;
		}

		public virtual void goToNextState() {
			resetContext();
		}

		public virtual Model.Display.IRenderable getCurrentScreen() {
			return this.display;
		}

		public virtual void goToPreviousState() {
			resetContext();
		}

		public void setStates() {
			
		}

		public virtual void reset() {
			Dispose();
			this.display = createInstance();
#if DEBUG
			Console.Clear();
#endif
		}

		public void Dispose() {
			if (this.display != null) {
				//this.display.dispose();
			}
		}
	}
}
