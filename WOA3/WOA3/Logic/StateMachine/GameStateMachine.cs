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
		public BaseGameState Tutorial { get; set; }
		public BaseGameState TutorialComplete { get; set; }
		public BaseGameState GameOverState { get; set; }

		internal State CurrentState { get; set; }

		public GameStateMachine(GraphicsDevice device, ContentManager content) {
			this.CompanyCinematic = new CompanyCinematicState(this, device, content);
			this.GameDevCinematic = new GameDevSplashState(this, device, content);
			this.GameDisplay = new GameDisplayState(this, device, content);
			this.Tutorial = new TutorialState(this, device, content);
			this.TutorialComplete = new TutorialCompleteState(this, device, content);
			this.MainMenu = new MainMenuState(this, device, content);
			this.GameOverState = new GameOverState(this, device, content);

			//this.CurrentState = this.CompanyCinematic;
			//this.CurrentState = this.GameDevCinematic;
			//this.CurrentState = this.MainMenu;
			this.CurrentState = this.GameDisplay;
			//this.CurrentState = this.GameOverState;
			//this.CurrentState = Tutorial;
			//this.CurrentState = TutorialComplete;
			this.CurrentState.reset();

			setStates();
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
			CurrentState.setStates();
		}

		public void reset() {
			this.CurrentState.reset();
		}

		public void Dispose() {
			this.CompanyCinematic.Dispose();
			this.GameDevCinematic.Dispose();
			this.MainMenu.Dispose();
			this.GameDisplay.Dispose();
			this.Tutorial.Dispose();
			this.TutorialComplete.Dispose();
		}
	}
}
