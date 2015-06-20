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
		private StaticDrawable2D image;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialComplete(ContentManager content)
			: base(content, "GeneralBackground") {

			VisualCallback setPrevipousState = delegate() {
				GameStateMachine.getInstance().goToPreviousState();
			};
			VisualCallback setNextState = delegate() {
				GameStateMachine.getInstance().LevelContext = null;
				GameStateMachine.getInstance().goToNextState();
			};

			List<ButtonRequest> requests = new List<ButtonRequest>();

			requests.Add(new ButtonRequest("Menu", setPrevipousState));
			requests.Add(new ButtonRequest("Torment", setNextState));
			base.createButtons(requests.ToArray());

			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Tut_Finish");
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 2)
			};
			this.image = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			this.image.render(spriteBatch);
		}
		#endregion Support methods
	}
}
