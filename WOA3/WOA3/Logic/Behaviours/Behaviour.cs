using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Logic.Behaviours {
	public interface Behaviour {
		void update(float elapsed);

		Vector2 Position { get; set; }
	}
}
