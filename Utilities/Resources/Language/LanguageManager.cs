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
				if( languagePair.ContainsKey( language ) && languagePair[ language ].ContainsKey( key ) ) {
					return languagePair[ language ][ key ];
				} else {
					throw new KeyNotFoundException();
				}
			}
		}

		private Dictionary<string, Dictionary<string, string>> languagePair; // Welp, this is confusing.

		private LanguageManager() {
			languagePair = new Dictionary<string, Dictionary<string, string>>();
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