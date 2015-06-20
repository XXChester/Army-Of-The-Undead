using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.StateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Scenarios;

namespace WOA3.Model.Display {
	public class GameOverDisplay : BaseMenu {
		#region Class variables
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameOverDisplay(GraphicsDevice graphics, ContentManager content, GameStateMachine stateMachine)
			: base(content, "Monster1", new Vector2()) {			

			
			VisualCallback setPrevipousState = delegate() {
				stateMachine.goToPreviousState();
			};
			VisualCallback setNextState = delegate() {
				stateMachine.goToNextState();
			};

			List<ButtonRequest> requests = new List<ButtonRequest>();
			requests.Add(new ButtonRequest("Menu", setNextState));
			requests.Add(new ButtonRequest("Restart", setPrevipousState));
			base.createButtons(requests.ToArray());
		}
		#endregion Constructor

		#region Support methods
		#endregion Support methods
	}
}
