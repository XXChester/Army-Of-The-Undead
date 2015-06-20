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
	public abstract class BaseTutorialScenario {
		private StaticDrawable2D tutorialHelp;
		protected Ghost ghost;
		protected Mob mob;

		public bool Completed { get; set; }


		public BaseTutorialScenario(ContentManager content, string scenarioName, Ghost ghost, Mob mob) {
			Constants.ALLOW_MOB_ATTACKS = false;
			Constants.ALLOW_PLAYER_ATTACKS = false;

			this.ghost = ghost;
			this.mob = mob;

			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Tut_" + scenarioName);
			Vector2 position = new Vector2(Constants.RESOLUTION_X / 2, 0f);
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2(texture.Width / 2f, 0f)
			};
			this.tutorialHelp = new StaticDrawable2D(parms);
		}
		public abstract void update(float elapsed);

		public virtual void init() {

		}

		public virtual void render(SpriteBatch spriteBatch) {
			this.tutorialHelp.render(spriteBatch);
		}

	}
}
