using System;
using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.AI.AStar.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Engine;
using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;
using WOA3.Logic.StateMachine;

namespace WOA3.Model {
	public class Yeti : Mob {
		#region Class variables

		#endregion Class variables

		#region Class propeties
		
		#endregion Class properties

		#region Constructor
		public Yeti(ContentManager content, Vector2 position, CharactersInRange charactersInRange, OnDeath onDeath, CollisionCheck collisionCheck)
			:base(content, position, charactersInRange, onDeath, collisionCheck, "Monster2") {
		}
		#endregion Constructor

		#region Support methods
		protected override void initSkills() {
			SoundEffect sfx = LoadingUtils.load<SoundEffect>(content, "Curse");
			SkillFinished holySwirl = delegate() {
				float effectLife;
				Base2DSpriteDrawable shockWave = ModelGenerationUtil.generateWaveEffect(content, base.Position, Color.LightBlue, out effectLife);
				VisualEffect effect = new VisualEffect(shockWave, effectLife);
				EffectsManager.getInstance().Visuals.Add(effect);
			};
			// this skill scales based on the level, start at 2000, get to 1600
			int cooldown = 1800;
			int index = GameStateMachine.getInstance().LevelContext.MapIndex - 1;
			if (index != 0) {
				//cooldown = cooldown = 1600;
			}
			
			this.skills.Add(new HolySwirl(sfx, 2f, holySwirl, cooldown));
		}
		#endregion Support methods
	}
}
