using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WOA3 {
	public class Debug {

#if DEBUG
		public static bool debugOn;
		public static bool showAIMap;
		public static bool mobsCanMove = true;
		public static Texture2D debugChip;
		public static Texture2D debugRing;

		public static Color DEBUG_BBOX_Color = Color.Red;
		public static Color DEBUG_PIVOT_Color = Color.White;
		public static Color DEBUG_RADIUS_COLOUR = Color.LightPink;
		public static Color DEBUG_AI_MAP_WALKABLE = Color.Green;

		public static void log(Object obj) {
			Console.WriteLine(obj);
		}

		public static void update() {
			if (InputManager.getInstance().wasKeyPressed(Keys.NumPad0)) {
				debugOn = !debugOn;
			} else if (InputManager.getInstance().wasKeyPressed(Keys.NumPad1)) {
				showAIMap = !showAIMap;
			} else if (InputManager.getInstance().wasKeyPressed(Keys.NumPad4)) {
				mobsCanMove = !mobsCanMove;
			}
		}

#endif
	}
}
