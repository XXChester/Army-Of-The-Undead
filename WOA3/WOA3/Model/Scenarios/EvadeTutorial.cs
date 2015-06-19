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
	public class EvadeTutorial : EnemySpawnTutorial {

		private bool hasBeenChased;

		public EvadeTutorial(ContentManager content, string scenarioName, Ghost ghost, Mob mob)
			: base(content, scenarioName, ghost, mob) {
			mob.Inactive = false;
			// 70, 250
			this.goal = new BoundingBox(new Vector3(70, 250, 0), new Vector3(150, 300, 0));
			this.hasBeenChased = false;
		}

		public override void update(float elapsed) {
			if (hasBeenChased && !this.ghost.isVisible() && this.mob.isIdle() && this.ghost.BBox.Intersects(this.goal)) {
				this.Completed = true;
			}

			if (this.mob.isTracking()) {
				this.hasBeenChased = true;
			}
		}
	}
}
