using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.GameStateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Scenarios;

namespace WOA3.Model {
	public class ModelGenerationUtil {
		public static TexturedEffectButton createButton(ContentManager content, Vector2 position, String name) {
			const float SPACE = 65f;
			PulseEffectParams effectParms = new PulseEffectParams {
				ScaleBy = 1f,
				ScaleDownTo = .9f,
				ScaleUpTo = 1.1f
			};

			Vector2 origin = new Vector2(128f, 64f);
			Vector2 scale = new Vector2(1f, .5f);
			TexturedEffectButtonParams buttonParms = new TexturedEffectButtonParams {
				Position = position,
				Origin = origin,
				Scale = scale,
				Effects = new List<BaseEffect> {
					new PulseEffect(effectParms)
				},
				PickableArea = CollisionGenerationUtils.getButtonRectangle(origin, position),
				ResetDelegate = delegate(StaticDrawable2D button) {
					button.Scale = scale;
				}
			};
			buttonParms.Texture = LoadingUtils.load<Texture2D>(content, name);
			buttonParms.Position = new Vector2(buttonParms.Position.X, buttonParms.Position.Y + SPACE * 1.3f);
			buttonParms.PickableArea = CollisionGenerationUtils.getButtonRectangle(origin, buttonParms.Position);
			return new TexturedEffectButton(buttonParms);
		}


		public static Base2DSpriteDrawable generateWaveEffect(ContentManager content, Vector2 position, Color light, out float effectLife) {
			int frames = 6;
			float speed = 15f;
			BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams() {
				AnimationState = AnimationState.PlayForwardOnce,
				TotalFrameCount = frames,
				FrameRate = speed,
			};
			Animated2DSpriteLoadSingleRowBasedOnTexture parms = new Animated2DSpriteLoadSingleRowBasedOnTexture() {
				AnimationParams = animationParms,
				Position = Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE)),
				LightColour = light,
				Origin = new Vector2(Constants.TILE_SIZE),
				Texture = LoadingUtils.load<Texture2D>(content, "WaveEffect")
			};
			effectLife = frames * speed;
			return new Animated2DSprite(parms);
		}
	}
}
