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
		protected GameStateMachine stateMachine;
		protected ContentManager content;
		protected IRenderable display;

		public BaseGameState(GameStateMachine stateMachine, GraphicsDevice device, ContentManager content, IRenderable display) {
			this.device = device;
			this.content = content;
			this.stateMachine = stateMachine;
			this.display = display;
		}

		protected void changeState(State newState) {
			stateMachine.CurrentState = newState;
		}

		public virtual void goToNextState() {
			
		}

		public virtual Model.Display.IRenderable getCurrentScreen() {
			return this.display;
		}

		public virtual void goToPreviousState() {
			
		}

		public virtual void setStates() {
			StateManager.getInstance().CurrentGameState = GameState.Waiting;
		}

		public void Dispose() {
			this.display.dispose();
		}
	}
}
