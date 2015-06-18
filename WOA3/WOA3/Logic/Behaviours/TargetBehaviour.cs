using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WOA3.Logic.Behaviours {
	interface TargetBehaviour : Behaviour {

		Vector2 Target { get; set; }
	}
}
