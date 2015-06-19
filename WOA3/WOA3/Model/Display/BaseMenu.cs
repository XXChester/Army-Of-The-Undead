using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Model.Display {
	public class BaseMenu : IRenderable {
		#region Class variables
		private StaticDrawable2D background;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public BaseMenu(ContentManager content, string backgroundName, Vector2 position) {
			Texture2D texture = LoadingUtils.load<Texture2D>(content, backgroundName);
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = position
			};
			this.background = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		public virtual void update(float elapsed) {
			this.background.update(elapsed);
		}

		public virtual void render(SpriteBatch spriteBatch) {
			if (this.background != null) {
				this.background.render(spriteBatch);
			}
		}
		#endregion Support methods


		public void dispose() {
			this.background.dispose();
		}
	}
}
