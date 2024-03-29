﻿using System;
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
	public class VisualEffect {
		private float elapsed;

		private readonly float TIME_TO_LIVE;

		public Base2DSpriteDrawable Image { get; set; }
		public bool UpForRemoval { get; set; }

		public VisualEffect(Base2DSpriteDrawable image, float timeToLive) {
			this.Image = image;
			this.TIME_TO_LIVE = timeToLive;
		}


		public void update(float elapsed) {
			this.elapsed += elapsed;
			if (this.elapsed <= TIME_TO_LIVE) {
				this.Image.update(elapsed);
			} else {
				this.UpForRemoval = true;
			}
		}

		public void render(SpriteBatch spriteBatch) {
			this.Image.render(spriteBatch);
		}
	}
}
