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


namespace WOA3.Model.Skills {
	public class CorpseExplode : Skill {
		ContentManager content;
		Vector2 position;

		private const int COOL_DOWN = 1;
		public CorpseExplode(ContentManager content, float damage, Vector2 position) : base(damage, 1, null) {
			this.content = content;
			this.position = position;
		}

		protected override void initVisuals() {
			base.initVisuals();
			BaseParticle2DEmitterParams parms = new BaseParticle2DEmitterParams() {
				ParticleTexture = LoadingUtils.load<Texture2D>(content, "bloodDrop"),
				SpawnDelay = 0f
			};
			emitters.Add(new BloodEmitter(parms, position));
		}
	}
}
