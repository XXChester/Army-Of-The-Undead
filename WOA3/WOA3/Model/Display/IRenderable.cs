
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Model.Display {
	public interface IRenderable {
		void render(SpriteBatch spriteBatch);
		void update(float elapsed);
	}
}
