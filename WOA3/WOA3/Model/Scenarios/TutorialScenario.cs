﻿using System;
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
using WOA3.Model.Skills;
using WOA3.Logic.AI;
using WOA3.Engine;
using WOA3.Map;

namespace WOA3.Model.Scenarios {
	public class TutorialScenario {
		private StaticDrawable2D tutorialHelp;
		protected Ghost ghost;
		protected Mob mob;

		public bool Completed { get; set; }


		public TutorialScenario(ContentManager content, string scenarioName, Ghost ghost, Mob mob) {
			this.ghost = ghost;
			this.mob = mob;

			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Tutorial" + scenarioName);
			Vector2 position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y - texture.Height);
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2((position.X - texture.Width) / 2f, 0f)
			};
			this.tutorialHelp = new StaticDrawable2D(parms);
		}
		public virtual void update(float elapsed) {
			if (ghost.Selected) {
				this.Completed = true;
			}
		}

		public virtual void render(SpriteBatch spriteBatch) {
			this.tutorialHelp.render(spriteBatch);
		}

	}
}
