using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Utilities.Cache;
using Utilities.Logging;
using Utilities.Persistence;
using Utilities.Resources.Language;

namespace Utilities.Saving {
	public class SaveManager : IEnumerable<SaveState> {

		/// <summary>
		/// Holds the location of the Save folder location.
		/// </summary>
		public static string SaveLocation {
			get => PersistenceManager.SaveLocation;
		}

		/// <summary>
		/// Holds the location of the Save file location.
		/// </summary>
		public static string SaveFile {
			get => Path.Combine( SaveLocation, "saves.json" );
		}

		/// <summary>
		/// This holds the instence of the class.
		/// </summary>
		public static SaveManager Singleton {
			get {
				if( singleton is null ) {
					singleton = new SaveManager();
				}

				return singleton;
			}
		}

		// Private holder.
		private static SaveManager singleton;

		/// <summary>
		/// Holds the path to the icon file.
		/// </summary>
		public string IconFile {
			get;
			private set;
		}

		/// <summary>
		/// A getter for getting the SaveState at that index.
		/// </summary>
		/// <param name="index">An int to get a SaveState from that position.</param>
		/// <returns>The SaveState at that index.</returns>
		public SaveState this[ int index ] {
			get {
				return saves[ index ];
			}
		}

		/// <summary>
		/// A getter for getting the SaveState by the name.
		/// </summary>
		/// <param name="name">The name of the SaveState.</param>
		/// <returns>The SaveState of that name.</returns>
		/// <exception cref="Exception">An exception is thrown if no SaveState by that name can be found.</exception>
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

		/// <summary>
		/// Returns the count of saves.
		/// </summary>
		public int Count {
			get => saves.Count;
		}

		private readonly JsonSerializerSettings formatting; // Holds the formatter.
		private readonly List<SaveState> saves; // Holds the list of SaveStates.
		private readonly Log log; // Holds the log object.

		// Private constructor so no other save manager can be made.
		private SaveManager() {
			// Creates a new JsonSerializerSettings object.
			formatting = new JsonSerializerSettings();
			// Creates a new log object for writing to a log.
			log = new Log( "Utilities.Saving.SaveManager" );
			// Sets the settings in the formatting object.
			formatting.Formatting = Formatting.Indented;
			formatting.NullValueHandling = NullValueHandling.Include;
			// Checks to see if the save directory exists.
			if( !Directory.Exists( SaveLocation ) ) {
				Directory.CreateDirectory( SaveLocation ); // Creates a new directory if it doesn't.
				File.Create( SaveFile ).Close(); // Creates a new file and promptly closes it.
				saves = new List<SaveState>(); // Creates a new List of SaveStates.
			} else {
				// In case something goes wrong.
				try {
					string json = File.ReadAllText( SaveFile ); // Holds the contents of the save file.

					// Checks to see if the json contains anything.
					if( json.Length > 0 ) {
						saves = JsonConvert.DeserializeObject( json, typeof( List<SaveState> ), formatting ) as List<SaveState>; // Deserializes the contents.
					} else {
						saves = new List<SaveState>(); // Creates an empty list.
					}
				} catch( Exception e ) {
					saves = new List<SaveState>(); // If exception is thrown, an empty list is created.

					log.Exception( $"{e.Message}\n{e.StackTrace}", e, false ); // Logs the exception.
				}
			}
		}

		/// <summary>
		/// Writes the contents of a save to the file.
		/// </summary>
		/// <param name="name">The name of the project.</param>
		/// <param name="about">The contents of the about box.</param>
		/// <param name="data">The information of the songs.</param>
		/// <param name="iconPath">The path to the icon.</param>
		public void WriteSave( string name, string about, string[,] data, string iconPath ) {
			List<SongInfo> songs = new List<SongInfo>(); // Creates a new empty list of SongInfo's.

			for( int r = 0; r < data.GetLength( 0 ) - 1; ++r ) { // iterates through the data and converts it to a SongInfo.
				songs.Add( new SongInfo {
					Artist = data[ r, 0 ],
					Album = data[ r, 1 ],
					Title = data[ r, 2 ],
					SongPath = data[ r, 3 ]
				} );
			}

			WriteSave( new SaveState { // Calls the other method.
				iconPath = iconPath,
				about = about,
				name = name,
				songs = songs
			} );
		}

		/// <summary>
		/// Writes the pre-made SaveState to the file.
		/// </summary>
		/// <param name="save">The pre-made SaveState.</param>
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
				log.Exception( LanguageManager.Singleton[ "en_US", "save_error" ], e );
				return;
			}

			jsonData = JsonConvert.SerializeObject( saves.ToArray(), formatting );

			File.WriteAllText( SaveFile, jsonData, Encoding.UTF8 );
		}

		/// <summary>
		/// To iterate through all the SaveStates.
		/// </summary>
		/// <returns>An IEnumerator to iterate through all the SaveStates.</returns>
		public IEnumerator<SaveState> GetEnumerator() {
			foreach( SaveState save in saves ) {
				yield return save;
			}
		}

		/// <summary>
		/// Checks to see if a SaveState of that kind exists.
		/// </summary>
		/// <param name="save">The SaveState to check.</param>
		/// <returns>True if it finds one, false otherwise.</returns>
		public bool Contains( SaveState save ) {
			return Contains( save.name );
		}

		/// <summary>
		/// Checks to see if a SaveState of that name exists.
		/// </summary>
		/// <param name="name">The name to check for.</param>
		/// <returns>True if it finds one, false otherwise.</returns>
		public bool Contains( string name ) {
			foreach( SaveState save in saves ) {
				if( save.name == name ) {
					return true;
				}
			}

			return true;
		}

		// I don't understand this.
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}