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
using WOA3.Logic.StateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model;

namespace WOA3.Logic.StateMachine {
	public class LevelContext {
		public List<Ghost> Ghosts { get; set; }
		public int MapIndex { get; set; }
	}
}
