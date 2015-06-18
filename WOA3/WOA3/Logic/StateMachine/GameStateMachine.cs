using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Logic.StateMachine {
	public class GameStateMachine : State, IDisposable {

		public BaseGameState CompanyCinematic { get; set; }
		public BaseGameState GameDevCinematic { get; set; }
		public BaseGameState MainMenu { get; set; }
		public BaseGameState GameDisplay { get; set; }

		internal State CurrentState { get; set; }

		public GameStateMachine(GraphicsDevice device, ContentManager content) {
			this.CompanyCinematic = new CompanyCinematicState(this, device, content);
			this.GameDevCinematic = new GameDevSplashState(this, device, content);
			this.GameDisplay = new GameDisplayState(this, device, content);

			this.CurrentState = this.GameDisplay;
		}

		public void goToNextState() {
			CurrentState.goToNextState();
		}

		public Model.Display.IRenderable getCurrentScreen() {
			return CurrentState.getCurrentScreen();
		}

		public void goToPreviousState() {
			CurrentState.goToPreviousState();
		}
		
		public void setStates() {
			throw new NotImplementedException();
		}

		public void Dispose() {
			this.CompanyCinematic.Dispose();
			this.GameDevCinematic.Dispose();
			this.GameDisplay.Dispose();
		}
	}
}
