
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WOA3 {
	public class Constants {
		public const float TILE_SIZE = 32f;
		public const float BOUNDING_SPHERE_SIZE = Constants.TILE_SIZE + Constants.TILE_SIZE /4;
		public const int RESOLUTION_X = 1280;
		public const int RESOLUTION_Y = 768;
		
		public static Color TEXT_COLOUR = Color.Blue;

		public static SpriteFont FONT;

		public static string MAP_DIRECTORY = "WOAContent\\Data\\";
	}
}
