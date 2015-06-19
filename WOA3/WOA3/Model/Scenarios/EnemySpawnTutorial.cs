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
	public class EnemySpawnTutorial : MovementTutorial {

		public EnemySpawnTutorial(ContentManager content, string scenarioName, Ghost ghost, Mob mob)
			: base(content, scenarioName, ghost, mob) {
			this.goal = new BoundingBox(new Vector3(665, 440, 0), new Vector3(745, 510, 0));
			mob.Inactive = false;
		}

		public override void update(float elapsed) {
			if (this.ghost.BBox.Intersects(goal) && !this.ghost.isVisible() && this.mob.isIdle()) {
				this.Completed = true;
			}
		}
	}
}
