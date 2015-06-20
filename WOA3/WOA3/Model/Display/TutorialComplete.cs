using System.Collections.Generic;
using GWNorthEngine.Input;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WOA3.Logic;
using WOA3.Logic.StateMachine;

namespace WOA3.Model.Display {
	public class TutorialComplete : BaseMenu {
		#region Class variables

		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialComplete(ContentManager content, GameStateMachine gameStateMachine) : base(content, "TutorialComplete", new Vector2(500f)) {

			VisualCallback setPrevipousState = delegate() {
				gameStateMachine.goToPreviousState();
			};
			VisualCallback setNextState = delegate() {
				gameStateMachine.LevelContext = null;
				gameStateMachine.goToNextState();
			};

			List<ButtonRequest> requests = new List<ButtonRequest>();

			requests.Add(new ButtonRequest("Menu", setPrevipousState));
			requests.Add(new ButtonRequest("Play", setNextState));
			base.createButtons(requests.ToArray());

			/*Texture2D texture = LoadingUtils.load<Texture2D>(content, "TutorialComplete");
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 2)
			};
			this.cinematic = new StaticDrawable2D(parms);*/
		}
		#endregion Constructor

		#region Support methods
		
		#endregion Support methods
	}
}
