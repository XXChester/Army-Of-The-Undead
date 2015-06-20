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
	public class HUDSelected : Entity {
		public HUDSelected(ContentManager content, Vector2 position) : base(content){
			StaticDrawable2DParams parms = new StaticDrawable2DParams() {
				Position = position,
				Texture = LoadingUtils.load<Texture2D>(content, "Ghost_Grey"),
				Scale = new Vector2(.5f),
			};
			base.init(new StaticDrawable2D(parms));
		}
	}
}
