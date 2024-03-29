﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Logic.GameStateMachine {
	public class GameStateMachine : State, IDisposable {

		private static GameStateMachine instance = new GameStateMachine();

		public static GameStateMachine getInstance() {
			return instance;
		}

		public BaseGameState CompanyCinematic { get; set; }
		public BaseGameState MainMenu { get; set; }
		public BaseGameState GameDisplay { get; set; }
		public BaseGameState Tutorial { get; set; }
		public BaseGameState TutorialComplete { get; set; }
		public BaseGameState GameOverState { get; set; }
		public BaseGameState GameFinishedState { get; set; }
		public BaseGameState ExitState { get; set; }

		public LevelContext LevelContext { get; set; }
		internal State CurrentState { get; set; }
		public TransitionState TransitionState { get; set; }

		public GameStateMachine() {
		
		}

		public void init(GraphicsDevice device, ContentManager content) {
			this.CompanyCinematic = new CompanyCinematicState(device, content);
			this.GameDisplay = new GameDisplayState(device, content);
			this.Tutorial = new TutorialState(device, content);
			this.TutorialComplete = new TutorialCompleteState(device, content);
			this.MainMenu = new MainMenuState(device, content);
			this.GameOverState = new GameOverState(device, content);
			this.GameFinishedState = new GameFinishedState(device, content);
			this.ExitState = new ExitGameState(device, content);

			this.LevelContext = null;
			//this.CurrentState = this.CompanyCinematic;
		//	this.CurrentState = this.MainMenu;
			this.CurrentState = this.GameDisplay;
		//	this.CurrentState = this.GameOverState;
		//	this.CurrentState = Tutorial;
		//	this.CurrentState = TutorialComplete;
		//	this.CurrentState = this.GameFinishedState;

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
			this.MainMenu.Dispose();
			this.GameDisplay.Dispose();
			this.GameOverState.Dispose();
			this.Tutorial.Dispose();
			this.TutorialComplete.Dispose();
		}
	}
}
