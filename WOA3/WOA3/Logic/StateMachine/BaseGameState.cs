using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Model.Display;

namespace WOA3.Logic.StateMachine {
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

		public virtual void goToNextState() {
			
		}

		public virtual Model.Display.IRenderable getCurrentScreen() {
			return this.display;
		}

		public virtual void goToPreviousState() {
			
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
