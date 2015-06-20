using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.GameStateMachine;
using WOA3.Engine;
using WOA3.Map;
namespace WOA3.Model.Scenarios {
	public class ArmyTutorial : MovementTutorial {
		List<Ghost> allGhosts;

		public ArmyTutorial(ContentManager content, string scenarioName, Ghost ghost, Mob mob, List<Ghost> allGhosts, GameStateMachine gameStateMachine)
		:base(content, scenarioName, ghost, mob) {
				this.goal = new BoundingBox(new Vector3(1023, 120, 0), new Vector3(1103, 170, 0));
				this.allGhosts = allGhosts;
		}

		public override void update(float elapsed) {
			if (this.ghost.BBox.Intersects(goal) && this.allGhosts[1].BBox.Intersects(goal)) {
				this.Completed = true;
				GameStateMachine.getInstance().goToNextState();
			}
		}
	}
}
