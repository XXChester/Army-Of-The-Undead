
using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Logic;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public class Box : Entity {

		#region Class variables

		#endregion Class variables

		#region Class propeties
	
		#endregion Class properties

		#region Constructor
		public Box(ContentManager content, Vector2 position)
			:base(content) {

			Texture2D texture = null;
			texture = LoadingUtils.load<Texture2D>(content, "Quad");

			StaticDrawable2DParams wallParams = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Scale = new Vector2(.5f),
				Origin = new Vector2(Constants.TILE_SIZE)
			};
			
			base.init(new StaticDrawable2D(wallParams));

		}
		#endregion Constructor

		#region Support methods

		#endregion Support methods
	}
}
