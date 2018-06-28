using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Utilities.Cache;
using Utilities.Logging;
using Utilities.Persistence;

namespace Utilities.Saving {
	public class SaveManager : IEnumerable<SaveState> {

		public static string SaveLocation {
			get => PersistenceManager.SaveLocation;
		}

		public static string SaveFile {
			get => Path.Combine( SaveLocation, "saves.json" );
		}

		public static SaveManager Singleton {
			get {
				if( singleton is null ) {
					singleton = new SaveManager();
				}

				return singleton;
			}
		}

		private static SaveManager singleton;

		public string IconFile {
			get;
			private set;
		}

		public SaveState this[ int index ] {
			get {
				return saves[ index ];
			}
		}

		public SaveState this[ string name ] {
			get {
				foreach( SaveState save in saves ) {
					if( save.name == name ) {
						return save;
					}
				}

				throw new Exception( "Name is not in the list." );
			}
		}

		public int Count {
			get => saves.Count;
		}

		private readonly JsonSerializerSettings formatting;
		private readonly List<SaveState> saves;
		private readonly Log log;

		private SaveManager() {
			formatting = new JsonSerializerSettings();

			log = new Log( "Utilities.Saving.SaveManager" );

			formatting.Formatting = Formatting.Indented;
			formatting.NullValueHandling = NullValueHandling.Include;

			if( !Directory.Exists( SaveLocation ) ) {
				Directory.CreateDirectory( SaveLocation );
				File.Create( SaveFile ).Close();
				saves = new List<SaveState>();
			} else {
				try {
					string json = File.ReadAllText( SaveFile );
					saves = JsonConvert.DeserializeObject( json, typeof( List<SaveState> ), formatting ) as List<SaveState>;
				} catch( Exception e ) {
					saves = new List<SaveState>();

					log.Exception( $"{e.Message}\n{e.StackTrace}", e, false );
				}
			}
		}

		public void WriteSave( string _name, string _about, string[,] data, string _iconPath ) {
			List<SongInfo> _songs = new List<SongInfo>();

			for( int r = 0; r < data.GetLength( 0 ) - 1; ++r ) {
				_songs.Add( new SongInfo {
					Artist = data[ r, 0 ],
					Album = data[ r, 1 ],
					Title = data[ r, 2 ],
					SongPath = data[ r, 3 ]
				} );
			}

			WriteSave( new SaveState {
				iconPath = _iconPath,
				about = _about,
				name = _name,
				songs = _songs
			} );
		}

		public void WriteSave( SaveState save ) {
			string jsonData;
			bool noneFound = true;

			for( int i = 0; i < saves.Count; ++i ) {
				if( saves[ i ].name == save.name ) {
					if( saves[ i ] != save ) {
						saves[ i ] = save;
					}

					noneFound = false;
					break;
				}
			}

			if( noneFound ) {
				saves.Add( save );
			}

			try {
				CacheManager.Singleton.CacheItems( saves );
			} catch( Exception e ) {
				log.Exception( "An error has occured. Please report this on GitHub's Issues page.", e );
				return;
			}

			jsonData = JsonConvert.SerializeObject( saves.ToArray(), formatting );

			File.WriteAllText( SaveFile, jsonData, Encoding.UTF8 );
		}

		public IEnumerator<SaveState> GetEnumerator() {
			foreach( SaveState save in saves ) {
				yield return save;
			}
		}

		public bool Contains( SaveState save ) {
			return Contains( save.name );
		}

		public bool Contains( string name ) {
			foreach( SaveState save in saves ) {
				if( save.name == name ) {
					return true;
				}
			}

			return true;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}