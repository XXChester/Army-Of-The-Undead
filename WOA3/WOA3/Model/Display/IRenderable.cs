
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Model.Display {
	public interface IRenderable {
		void update(float elapsed);
		void render(SpriteBatch spriteBatch);
		void dispose();
	}
}
