using System.Collections.Generic;
using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace WOA3.Logic {
	public class SoundManager {
		#region Class variables
		// singleton instance
		private static SoundManager instance = new SoundManager();
		#endregion Class variables

		#region Class properties
		public SFXEngine sfxEngine { get; set; }
		public MusicEngine MusicEngine { get; set; }
		#endregion Class properties

		#region Constructor
		public SoundManager() {

		}

		public void init(ContentManager content) {
			SFXEngineParams parms = new SFXEngineParams();
			//parms.Muted = true;
#if !DEBUG
			parms.Muted = false;
#endif
			this.sfxEngine = new SFXEngine(parms);
			MusicEngineParams musicParms = new MusicEngineParams {
				Muted = false,
				PlayList = new List<Song> {
					LoadingUtils.load<Song>(content, "Whispers")
				}
			};
			this.MusicEngine = new MusicEngine(musicParms);
		}
		#endregion Constructor

		#region Support methods
		public static SoundManager getInstance() {
			return instance;
		}

		public void update() {
			this.sfxEngine.update();
			this.MusicEngine.update();
		}
		#endregion Support methods
	}
}
