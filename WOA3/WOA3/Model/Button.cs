
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


using WOA3.Logic;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public class Button : Entity {
		#region Class variables
		//private BoundingBox staticBoundingBox;
		private SoundEffect sfx;
		private VisualCallback setState;
		#endregion Class variables

		#region Class propeties
		public Texture2D Texture { get { return this.Texture; } }
		public TexturedEffectButton TexturedButton { get; set; }
		#endregion Class properties

		#region Constructor
		public Button(ContentManager content, TexturedEffectButton button, VisualCallback setState)
			: base(content) {
			//this.staticBoundingBox = boundingBox;
			this.sfx = LoadingUtils.load<SoundEffect>(content, "ButtonClick");
			this.setState = setState;
			this.TexturedButton = button;

			base.init(button);
		}
		#endregion Constructor

		#region Support methods
		/*protected override BoundingBox getBBox() {
			return this.staticBoundingBox;
		}*/

		public void click() {
			SoundManager.getInstance().sfxEngine.playSoundEffect(this.sfx);
			this.setState.Invoke();
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			this.TexturedButton.processActorsMovement(InputManager.getInstance().MousePosition);
			if (InputManager.getInstance().wasLeftButtonPressed()) {
				if (TexturedButton.isActorOver(InputManager.getInstance().MousePosition)) {
					click();
				}
			}
		}
		#endregion Support methods
	}
}
