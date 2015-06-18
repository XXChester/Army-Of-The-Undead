
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WOA3 {
	public class Constants {
		public const float TILE_SIZE = 16f;
		public const int RESOLUTION_X = 1280;
		public const int RESOLUTION_Y = 768;
		
		public static Color TEXT_COLOUR = Color.DarkBlue;

		public static SpriteFont FONT;


#if DEBUG
		public static Color DEBUG_BBOX_Color = Color.Red;
		public static Color DEBUG_PIVOT_Color = Color.White;
		public static Color DEBUG_RADIUS_COLOUR = Color.LightPink;
#endif
	}
}
