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
using WOA3.Engine;
using WOA3.Map;
namespace WOA3.Model.Scenarios {
	public class KillingTutorial : TutorialScenario {
		List<Ghost> allGhosts;

		public KillingTutorial(ContentManager content, string scenarioName, Ghost ghost, Mob mob, List<Ghost> allGhosts)
			: base(content, scenarioName, ghost, mob) {
			mob.Inactive = false;
			Constants.ALLOW_PLAYER_ATTACKS = true;
			this.allGhosts = allGhosts;
		}

		public override void update(float elapsed) {
			if (this.mob.Health.amIDead() && this.allGhosts.Count == 2) {
				this.Completed = true;
			}
		}
	}
}
