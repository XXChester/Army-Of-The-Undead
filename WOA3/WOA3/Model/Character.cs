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
	public abstract class Character : Entity {
		#region Class variables
		protected RadiusRing rangeRing;
		private Text2D healthText;

		private readonly float SPEED;
		#endregion Class variables

		#region Class propeties
		//public Character Target { get; set; }
		public ScaredFactor Health { get; set; }
		#endregion Class properties

		#region Constructor
		public Character(ContentManager content, Vector2 position, float speed)
			: base(content) {
			this.SPEED = speed;
			this.rangeRing = new RadiusRing(content, position);
			this.Health = new ScaredFactor();
			createHealthText(position);
		}
		#endregion Constructor

		#region Support methods
		private Text2D createHealthText(Vector2 position) {
			Text2DParams textParams = new Text2DParams() {
				Position = getTextPosition(position),
				LightColour = Constants.TEXT_COLOUR,
				WrittenText = this.Health.Text,
				Origin = new Vector2(Constants.TILE_SIZE / 2),
				Font = Constants.FONT
			};
			this.healthText = new Text2D(textParams);
			return this.healthText;
		}

		private Vector2 getTextPosition(Vector2 position) {
			return Vector2.Subtract(position, new Vector2(-6f, Constants.TILE_SIZE));
		}

		public abstract List<SkillResult> performSkills();
		public abstract SkillResult die();


		public void damage(float amount) {
			this.Health.scare(amount);
			this.healthText.WrittenText = this.Health.Text;
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			this.rangeRing.updatePosition(base.Position);
			this.healthText.Position = getTextPosition(base.Position);
			this.healthText.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.rangeRing.render(spriteBatch);
			this.healthText.render(spriteBatch);
			base.render(spriteBatch);
		}
		#endregion Support methods
	}
}