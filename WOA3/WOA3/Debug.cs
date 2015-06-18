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
		public static Texture2D debugLine;

#endif
	}
}
