using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using WOA3.Logic;
using WOA3.Logic.Behaviours;
using WOA3.Logic.Skills;

namespace WOA3.Model {
	public class Ghost : Entity {

		#region Class variables
		private Tracking seeking;

		private Dictionary<Keys, Skill> skills;
		#endregion Class variables

		#region Class propeties
		public bool Selected { get; set; }
		#endregion Class properties

		#region Constructor
		public Ghost(ContentManager content, Vector2 position)
			:base(content) {

			Texture2D texture = null;
			texture = LoadingUtils.load<Texture2D>(content, "Ghost");

			StaticDrawable2DParams wallParams = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2(Constants.TILE_SIZE)
			};

			base.init(new StaticDrawable2D(wallParams));

			this.seeking = new Tracking(position, 2f);
		}

		private void initSkills() {
			this.skills = new Dictionary<Keys, Skill>();
			this.skills.Add(Keys.D1, new Boo());
		}
		#endregion Constructor

		#region Support methods
		protected override BoundingBox getBBox() {
			return PositionUtils.getBBox32(this.Position);
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			this.seeking.update(elapsed);
			base.Position = this.seeking.Position;
			// update our skills
			foreach (var skill in skills) {
				skill.Value.update(elapsed);
			}


			if (InputManager.getInstance().wasRightButtonPressed() && Selected) {
				this.seeking.Target = GWNorthEngine.Input.InputManager.getInstance().MousePosition;
			}
		}
		#endregion Support methods
	}
}
