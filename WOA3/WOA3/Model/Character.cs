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
	public abstract class Character : Entity {
		#region Class variables
		protected RadiusRing rangeRing;
		private HealthBar healthBar;
		private CharactersInRange charactersInRange;
		private OnDeath onDeath;

		private readonly float SPEED;
		#endregion Class variables

		#region Class propeties
		//public Character Target { get; set; }
		public BoundingSphere Range { get { return this.rangeRing.BoundingSphere; } }
		public CharactersInRange CharactersInRange { get { return this.charactersInRange; } }
		public bool AmIDead { get { return this.healthBar.Health <= 0f; } }
		public float Health { get { return this.healthBar.Health; } }
		#endregion Class properties

		#region Constructor
		public Character(ContentManager content, Vector2 position, float speed, CharactersInRange charactersInRange, OnDeath onDeath, float health)
			: base(content) {
			this.SPEED = speed;
			this.rangeRing = new RadiusRing(content, position);
			this.charactersInRange = charactersInRange;
			this.healthBar = new HealthBar(content, position, health);
			this.onDeath = onDeath;
		}
		#endregion Constructor

		#region Support methods
		/*protected override BoundingBox getBBox() {
			return CollisionGenerationUtils.getCharacterBBox(this.Position);
		}*/

		protected abstract Skill getDeathSkill();
		public abstract List<SkillResult> performSkills();
		public abstract bool isVisible();


		public void damage(float amount) {
			this.healthBar.damage(amount);
		}

		public virtual Skill die() {
			if (this.onDeath != null) {
				this.onDeath.Invoke(this);
			}
			return getDeathSkill();
		}

		public override void update(float elapsed) {
			this.rangeRing.updatePosition(base.Position);
			this.healthBar.Position = base.Position;
			this.healthBar.update(elapsed);
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.healthBar.render(spriteBatch);
			base.render(spriteBatch);
#if DEBUG
			if (Debug.debugOn) {
				DebugUtils.drawBoundingBox(spriteBatch, CollisionGenerationUtils.getBBoxHalf(base.Position), Color.Yellow, Debug.debugChip);
			}
#endif
		}
		#endregion Support methods
	}
}
