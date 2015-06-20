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
	public class MovementTutorial : BaseTutorialScenario {
		protected Texture2D chip;
		protected BoundingBox goal;

		public MovementTutorial(ContentManager content, string scenarioName, Ghost ghost, Mob mob)
		:base(content, scenarioName, ghost, mob) {
				this.goal = new BoundingBox(new Vector3(240, 300, 0), new Vector3(320, 350, 0));
				this.chip = LoadingUtils.load<Texture2D>(content, "chip");
		}

		public override void update(float elapsed) {
			if (this.ghost.BBox.Intersects(goal)) {
				this.Completed = true;
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			DebugUtils.drawBoundingBox(spriteBatch, this.goal, Color.Green, chip);
		}
	}
}
