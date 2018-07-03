using System;
using System.Collections.Generic;
using System.IO;

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
			get => Helper.Path.Combine( new string[] { Environment.CurrentDirectory, "Resources", "Language" } );
		}

		private static LanguageManager singleton;

		#endregion

		public string this[ string language, string key ] {
			get {
				if( ContainsKey( language, key ) ) {
					return languagePair[ language ][ key ];
				} else {
					throw new KeyNotFoundException();
				}
			}
		}

		private Dictionary<string, Dictionary<string, string>> languagePair; // Welp, this is confusing.

		private LanguageManager() {
			languagePair = new Dictionary<string, Dictionary<string, string>>();
			GetAllLanguages();
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

		public string GetMessage( string lang, string key ) {
			return this[ lang, key ];
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
					languagePair.Add( key, temp );
				}
			}
		}
	}
}