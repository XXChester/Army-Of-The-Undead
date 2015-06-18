
namespace WOA3.Logic {
	public class StateManager {
		// singleton variable
		private static StateManager instance = new StateManager();

		#region Class variables
		private GameState currentGameState;
		private TransitionState currentTransitionState;
		#endregion Class variables

		#region Class propeties
		public GameState CurrentGameState {
			get { return this.currentGameState; }
			set {
				this.PreviousGameState = this.currentGameState;
				this.currentGameState = value;

				// some states sohuld not trigger a transition; these are listed here
				/*if (this.currentTransitionState != TransitionState.TransitionOut) {
					if (this.currentGameState != GameState.GameOver && this.currentGameState != GameState.Active &&
						this.currentGameState != GameState.Waiting && this.currentGameState != GameState.Options) {
						this.currentTransitionState = TransitionState.InitTransitionOut;
					}
				}*/
			}
		}
		public GameState PreviousGameState { get; set; }
		public TransitionState CurrentTransitionState {
			get { return this.currentTransitionState; }
			set {
				this.PreviousTransitionState = this.currentTransitionState;
				this.currentTransitionState = value;
			}
		}
		public TransitionState PreviousTransitionState { get; set; }
		#endregion Class properties

		#region Constructor
		public StateManager() {
			this.currentTransitionState = TransitionState.InitTransitionIn;
		}
		#endregion Constructor

		#region Support methods
		public static StateManager getInstance() {
			return instance;
		}
		#endregion Support methods
	}
}
