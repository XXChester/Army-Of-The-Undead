using System.Collections.Generic;

using GWNorthEngine.Model;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Logic;

namespace WOA3.Model {
	public abstract class Entity {
		#region Class variables
		private Base2DSpriteDrawable image;
		protected ContentManager content;
		protected bool alwaysRender;
		protected List<FinishableEffect> managedEffects = new List<FinishableEffect>();
		#endregion Class variables

		#region Class propeties
		public BoundingBox BBox { get; set; }
		public float Rotation {
			get { return this.image.Rotation; }
			set { this.image.Rotation = value; }
		}
		public Vector2 Scale {
			get { return this.image.Scale; }
			set { this.image.Scale = value; }
		}
		public Vector2 Position {
			get { return this.image.Position; }
			set {
				this.image.Position = value;
				this.BBox = getBBox();
			}
		}
		public Color LightColour { get { return image.LightColour; } set { this.LightColour = LightColour; } }
		#endregion Class properties

		#region Constructor
		public Entity(ContentManager content, bool alwaysRender=false) : this(content, null, alwaysRender) { }
		public Entity(ContentManager content, Base2DSpriteDrawable image, bool alwaysRender = false) {
			this.content = content;
			this.alwaysRender = alwaysRender;
			init(image);
		}
		#endregion Constructor

		#region Support methods
		protected void init(Base2DSpriteDrawable image) {
			this.image = image;
			if (this.image != null) {
				this.Position = this.image.Position;
			}
		}

		protected virtual BoundingBox getBBox() {
			return CollisionGenerationUtils.getBBox(this.Position);
		}

		public void addEffect(BaseEffect effect) {
			if (effect.GetType().IsAssignableFrom(typeof(FinishableEffect))) {
				this.managedEffects.Add((FinishableEffect)effect);
			}
			this.image.addEffect(effect);
		}

		public virtual void update(float elapsed) {
			if (this.image != null) {
				this.image.update(elapsed);
				/*FinishableEffect effect = null;
				for (int i = this.managedEffects.Count -1; i >= 0; i--) {
					effect = this.managedEffects[i];
					if (effect.HasFinished) {
						this.image.Effects.Remove((BaseEffect)effect);
						this.managedEffects.Remove(effect);
					}
				}*/
			}
		}

		public virtual void render(SpriteBatch spriteBatch) {
			if (StateManager.getInstance().CurrentGameState == GameState.Active || alwaysRender) {
				if (this.image != null) {
					this.image.render(spriteBatch);
				}
			}
#if DEBUG
			if (Debug.debugOn) {
				DebugUtils.drawBoundingBox(spriteBatch, this.BBox, Debug.DEBUG_BBOX_Color, Debug.debugChip);
			}
#endif
		}
		#endregion Support methods
	}
}
