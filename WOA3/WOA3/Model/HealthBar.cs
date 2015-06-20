using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using WOA3.Logic;
namespace WOA3.Model {
	public class HealthBar {
		#region Class variables
		private StaticDrawable2D remainingHealth;
		private StaticDrawable2D maximumHealth;
		private float currentHealth;
		private Vector2 position;
		private readonly float MAX_HEALTH;
		private readonly Vector2 SCALE = new Vector2(31f, 7f);
		#endregion Class variables

		#region Class propeties
		public Vector2 Position {
			set {
				Vector2 pos = getPosition(value);
				this.position = pos;
				this.remainingHealth.Position = pos;
				this.maximumHealth.Position = pos;
			}
		}
		public float Health { get { return this.currentHealth; } }
		#endregion Class properties

		#region Constructor
		public HealthBar(ContentManager content, Vector2 position, float health) {
			this.MAX_HEALTH = 10f;
			this.currentHealth = health;
			Vector2 pos = getPosition(position);
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = LoadingUtils.load<Texture2D>(content, "Chip");
			parms.Position = new Vector2(pos.X, pos.Y);
			parms.Scale = new Vector2(SCALE.X, SCALE.Y);
			parms.LightColour = Color.Green;
			this.remainingHealth = new StaticDrawable2D(parms);

			parms.LightColour = Color.DarkRed;
			this.maximumHealth = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		private Vector2 getPosition(Vector2 position) {
			return Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE/2, Constants.TILE_SIZE));
		}

		public void damage(float damage) {
			this.currentHealth = MathHelper.Clamp(this.currentHealth - damage, 0f, MAX_HEALTH);
		}

		public void update(float elapsed) {
			float percentHurt = this.currentHealth / MAX_HEALTH;
			float newScaleX = SCALE.X * percentHurt;
			this.remainingHealth.Scale = new Vector2(newScaleX, this.remainingHealth.Scale.Y);
		}

		public void render(SpriteBatch spriteBatch) {
			this.maximumHealth.render(spriteBatch);
			this.remainingHealth.render(spriteBatch);
		}
		#endregion Support methods
	}
}
