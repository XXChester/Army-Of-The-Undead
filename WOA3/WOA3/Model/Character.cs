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
using WOA3.Model.Skills;

namespace WOA3.Model {
	public abstract class Character : Entity {
		#region Class variables
		protected RadiusRing rangeRing;
		private Text2D healthText;
		private CharactersInRange charactersInRange;

		private readonly float SPEED;
		#endregion Class variables

		#region Class propeties
		//public Character Target { get; set; }
		public ScaredFactor Health { get; set; }
		public BoundingSphere Range { get { return this.rangeRing.BoundingSphere; } }
		public CharactersInRange CharactersInRange { get { return this.charactersInRange; } }
		#endregion Class properties

		#region Constructor
		public Character(ContentManager content, Vector2 position, float speed, CharactersInRange charactersInRange)
			: base(content) {
			this.SPEED = speed;
			this.rangeRing = new RadiusRing(content, position);
			this.Health = new ScaredFactor();
			this.charactersInRange = charactersInRange;
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


		/*protected override BoundingBox getBBox() {
			return CollisionGenerationUtils.getCharacterBBox(this.Position);
		}*/

		public abstract List<SkillResult> performSkills();
		public abstract Skill die();


		public void damage(float amount) {
			this.Health.scare(amount);
			this.healthText.WrittenText = this.Health.Text;
		}

		public override void update(float elapsed) {
			this.rangeRing.updatePosition(base.Position);
			this.healthText.Position = getTextPosition(base.Position);
			this.healthText.update(elapsed);
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			//this.rangeRing.render(spriteBatch);
			this.healthText.render(spriteBatch);
			base.render(spriteBatch);
		}
		#endregion Support methods
	}
}
