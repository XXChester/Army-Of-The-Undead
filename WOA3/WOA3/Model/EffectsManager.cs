using System;
using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Engine;
using WOA3.Logic;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;
using WOA3.Logic.Skills;

namespace WOA3.Model {
	public class EffectsManager {
		// singleton variable
		private static EffectsManager instance = new EffectsManager();

		#region Class variables

		#endregion Class variables

		#region Class propeties
		public List<VisualEffect> Visuals { get; set; }
		#endregion Class properties

		#region Constructor
		public EffectsManager() {
			
		}
		#endregion Constructor

		#region Support methods
		public static EffectsManager getInstance() {
			return instance;
		}

		
		public void init() {
			this.Visuals = new List<VisualEffect>();
		}

		public void update(float elapsed) {
			VisualEffect visual = null;
			for (int i = this.Visuals.Count - 1; i >= 0; i--) {
				visual = this.Visuals[i];
				if (visual != null) {
					visual.update(elapsed);
					if (visual.UpForRemoval) {
						this.Visuals.Remove(visual);
						visual = null;
					}
				}
			}
		}

		public void render(SpriteBatch spriteBatch) {
			foreach (var visual in this.Visuals) {
				if (visual != null) {
					visual.render(spriteBatch);
				}
			}
		}
		#endregion Support methods
	}
}
