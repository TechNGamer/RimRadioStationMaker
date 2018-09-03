using System;
using System.Collections.Generic;
using System.IO;
using Utilities.Logging;

namespace Utilities.Resources.Language {
	// TODO: Work on LanguageManager.
	public class LanguageManager {

		#region Static Methods

		public static LanguageManager Singleton {
			get {
				if( singleton is null ) {
					singleton = new LanguageManager();
				}

				return singleton;
			}
		}

		public static string LanguageFolder {
			get => Helper.PathExt.Combine( new string[] { Environment.CurrentDirectory, "Resources", "Language" } );
		}

		private static LanguageManager singleton;

		#endregion

		public string this[ string language, string key ] {
			get {
				return GetMessage( language, key );
			}
		}

		public string LanguageUsed {
			get;
			set;
		}

		public string[] Keys {
			get {
				string[] keys = new string[ languagePair.Count ];

				languagePair.Keys.CopyTo( keys, 0 );

				return keys;
			}
		}

		private Dictionary<string, Dictionary<string, string>> languagePair; // Welp, this is confusing.
		private Log log;

		private LanguageManager() {
			languagePair = new Dictionary<string, Dictionary<string, string>>();
			log = new Log( "Utilities.Resources.Language.LanguageManager" );

			GetAllLanguages();

			LanguageUsed = Keys[ 0 ];
		}

		public bool ContainsLanguage( string lang ) {
			return languagePair.ContainsKey( lang );
		}

		public bool ContainsKey( string lang, string key ) {
			if( ContainsLanguage( lang ) ) {
				return languagePair[ lang ].ContainsKey( key );
			} else {
				throw new KeyNotFoundException();
			}
		}

		public bool ContainsValue( string lang, string value ) {
			if( ContainsLanguage( lang ) ) {
				return languagePair[ lang ].ContainsValue( value );
			} else {
				throw new KeyNotFoundException();
			}
		}

		public string GetMessage( string language, string key ) {
			if( ContainsKey( language, key ) ) {
				return languagePair[ language ][ key ];
			} else {
				throw new KeyNotFoundException();
			}
		}

		public string TryGetMessage( string language, string key ) {
			try {
				return GetMessage( language, key );
			} catch {
				log.Message( $"No key was found, returning '{key}' back." );
				return key;
			}
		}

		private void GetAllLanguages() {
			string[] langs = Directory.GetFiles( LanguageFolder, "*.lang" );

			foreach( string lang in langs ) {
				string key = null;
				Dictionary<string, string> temp = new Dictionary<string, string>();

				foreach( string line in File.ReadAllLines( lang ) ) {
					if( line.StartsWith( "key" ) ) {
						key = line.Split( '=' )[ 1 ];
					} else {
						string[] keyValue = line.Split( '=' );
						temp.Add( keyValue[ 0 ], keyValue[ 1 ] );
					}
				}

				if( key is null || key is "" ) {
					throw new Exception( "Key is empty" );
				} else {
					try {
						languagePair.Add( key, temp );
					} catch { }
				}
			}
		}
	}
}