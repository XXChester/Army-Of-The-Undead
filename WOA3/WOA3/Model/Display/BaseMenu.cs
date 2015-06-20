using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Logic;

namespace WOA3.Model.Display {
	public class BaseMenu : IRenderable {
		public struct ButtonRequest {
			private string name;
			private VisualCallback callback;
			public string Name { get { return this.name; } }
			public VisualCallback Callback { get { return this.callback; } }

			public ButtonRequest(String name, VisualCallback callBack) {
				this.name = name;
				this.callback = callBack;
			}
		}
		#region Class variables
		private ContentManager content;
		private StaticDrawable2D background;
		private Base2DSpriteDrawable title;
		protected List<Button> buttons;

		private const float xBuffer = 96f;
		private const float leftSideX = xBuffer *2;
		private const float yBuffer = 128;
		private const float rightSideX = Constants.RESOLUTION_X - (leftSideX);
		private const float middle = Constants.RESOLUTION_X / 2 ;
		private const float y = Constants.RESOLUTION_Y - yBuffer;
		private readonly float[] POSITIONS = new float[]{ leftSideX, rightSideX, middle };
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public BaseMenu(ContentManager content, string backgroundName) {
			Vector2 position = new Vector2(Constants.RESOLUTION_X / 2, Constants.RESOLUTION_Y / 8 * 4);
			this.content = content;
			Texture2D texture = LoadingUtils.load<Texture2D>(content, backgroundName);
			StaticDrawable2DParams parms = new StaticDrawable2DParams {
				Texture = texture,
				Origin = new Vector2(texture.Width / 2, texture.Height / 2),
				Position = position
			};
			this.background = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		public void createButtons(ButtonRequest[] requests) {
			this.buttons = new List<Button>();
			if (requests.Length == 1) {
				ButtonRequest request = requests[0];
				TexturedEffectButton button = ModelGenerationUtil.createButton(content, new Vector2(middle, y), request.Name);
				this.buttons.Add(new Button(content, button, request.Callback));
			} else {
				for (int i = 0; i < requests.Length; i++) {
					ButtonRequest request = requests[i];
					TexturedEffectButton button = ModelGenerationUtil.createButton(content, new Vector2(POSITIONS[i], y), request.Name);
					this.buttons.Add(new Button(content, button, request.Callback));
				}
			}
		}

		public virtual void update(float elapsed) {
			this.background.update(elapsed);
			if (this.buttons != null) {
				foreach (var button in this.buttons) {
					button.update(elapsed);
				}
			}
		}

		public virtual void render(SpriteBatch spriteBatch) {
			if (this.background != null) {
				this.background.render(spriteBatch);
			}
			if (this.title != null) {
				this.title.render(spriteBatch);
			}
			if (this.buttons != null) {
				foreach (var button in this.buttons) {
					button.render(spriteBatch);
#if DEBUG
					if (Debug.debugOn) {
						DebugUtils.drawRectangle(spriteBatch, button.TexturedButton.PickableArea, Debug.DEBUG_BBOX_Color, Debug.debugChip);
					}
#endif
				}
			}
		}
		#endregion Support methods


		public void dispose() {
			this.background.dispose();
		}
	}
}
