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
		// singleton variable
		private static SoundManager instance = new SoundManager();
		private List<SoundEmitter> emitters;
		private Vector2[] lastKnownListenersPositions;
		#endregion Class variables

		#region Class propeties
		public SFXEngine SFXEngine { get; set; }
		public MusicEngine MusicEngine { get; set; }
		#endregion Class properties

		#region Constructor
		public SoundManager() {
			this.lastKnownListenersPositions = new Vector2[] { new Vector2(-10000f) };
		}
		#endregion Constructor

		#region Support methods
		public static SoundManager getInstance() {
			return instance;
		}

		private void destroyEmitter(SoundEmitter emitter) {
			emitter.stopSoundEffect();
			this.emitters.Remove(emitter);
		}

		public void init(ContentManager content) {
			SFXEngineParams sfxEngineParms = new SFXEngineParams();
			sfxEngineParms.Muted = true;
			this.SFXEngine = new SFXEngine(sfxEngineParms);
			this.emitters = new List<SoundEmitter>();

			MusicEngineParams musicParms = new MusicEngineParams {
				Muted = true,
				PlayList = new List<Song> {
					LoadingUtils.load<Song>(content, "Background")
				}
			};
			this.MusicEngine = new MusicEngine(musicParms);
		}

		public void addEmitter(SoundEmitter emitter) {
			lock (this.emitters) {
				this.emitters.Add(emitter);
			}
		}

		public void removeEmitter(SoundEmitter emitter) {
			lock (this.emitters) {
				destroyEmitter(emitter);
			}
		}

		public void removeAllEmitters() {
			lock (this.emitters) {
				if (this.emitters != null) {
				for (int i = this.emitters.Count -1; i > 0; i--) {
					destroyEmitter(this.emitters[i]);
				}
			}
			}
		}

		public void playSoundEffect(SoundEmitter sfxEmitter, SoundEffect sfx, bool loop=false) {
			update(this.lastKnownListenersPositions);
			sfxEmitter.playSoundEffect(sfx, loop: loop);
		}

		public void update() {
			this.MusicEngine.update();
		}

		public void update(Vector2[] listenersPositions) {
			lock (this.emitters) {
				foreach (SoundEmitter emitter in this.emitters) {
					emitter.update(listenersPositions);
				}
			}
			this.lastKnownListenersPositions = listenersPositions;
		}
		#endregion Support methods
	}
}
