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
using WOA3.Model.Display;

namespace WOA3.Model {
	public class HUDIcon : Entity, IRenderable {
		
		private float timeHovering;
		private StaticDrawable2D coolDown;
		
		public const int ICON_SIZE = 64;

		public HUDIcon(ContentManager content, Vector2 position, string textureName) : base(content) {
			this.content = content;

			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Position = position;
			parms.Origin = new Vector2(ICON_SIZE / 2f);
			parms.Texture = LoadingUtils.load<Texture2D>(content, "Skill" + textureName);

			base.init(new StaticDrawable2D(parms));
			parms.Position = Vector2.Add(position, new Vector2(0f, ICON_SIZE / 2f));
			parms.Texture = LoadingUtils.load<Texture2D>(content, "CooldownChip");
			parms.Origin = new Vector2(ICON_SIZE /2f, 0f);
			parms.Rotation = MathHelper.ToRadians(180f);
			parms.Scale = getDefaultScale();
			this.coolDown = new StaticDrawable2D(parms);
			//this.information = new TextPopup(content, position);
		}

		private Vector2 getDefaultScale() {
			return new Vector2(1f, 0f);
		}
		
		protected override BoundingBox getBBox() {
			return CollisionGenerationUtils.getBBox64(this.Position);
		}

		public void setCooldown(float percentComplete) {
			float scale = 1f - (percentComplete / 100f);
			scale = MathHelper.Clamp(scale, 0f, 1f);
			this.coolDown.Scale = new Vector2(1f, scale);
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			this.coolDown.update(elapsed);
			//this.information.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			this.coolDown.render(spriteBatch);
			//this.information.render(spriteBatch);
		}

		public void dispose() {
			
		}
	}
}
