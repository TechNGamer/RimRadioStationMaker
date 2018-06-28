using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.Persistence {

	public enum ConfigurationFields {
		RimWorldExe, RimWorldAssembly, UnityAssembly
	}

	/// <summary>
	/// PersistenceManager holds all the configuration data along with the paths to each folder in the persistence location.
	/// </summary>
	public class PersistenceManager {

		#region Static Fields

		/// <summary>
		/// The reference to the singleton.
		/// </summary>
		public static PersistenceManager Singleton {
			get {
				if( singleton is null ) { // Checks if singleton is null.
					singleton = new PersistenceManager(); // Creates a new singleton.
				}

				return singleton; // Returns the singleton.
			}
		}

		private static PersistenceManager singleton; // Holds the persistent reference of the singleton.

		#region Folders

		/// <summary>
		/// Holds the location of where all the data is held.
		/// </summary>
		public static string PersistentLocation {
			get {
				return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "RRSM" );
			}
		}

		/// <summary>
		/// Holds the location of the main config directory.
		/// </summary>
		public static string ConfigLocation {
			get {
				return Path.Combine( PersistentLocation, "Config" );
			}
		}

		/// <summary>
		/// The location of the cache.
		/// </summary>
		public static string CacheLocation {
			get {
				return Path.Combine( PersistentLocation, "Cache" );
			}
		}

		/// <summary>
		/// Holds the location of the save folder.
		/// </summary>
		public static string SaveLocation {
			get {
				return Path.Combine( PersistentLocation, "Save" );
			}
		}

		/// <summary>
		/// Holds the location of the log folder.
		/// </summary>
		public static string LogLocation {
			get {
				return Path.Combine( PersistentLocation, "Log" );
			}
		}

		#endregion

		/// <summary>
		/// Holds the location of the config file.
		/// </summary>
		public static string ConfigFileLocation {
			get {
				return Path.Combine( ConfigLocation, "config.ini" );
			}
		}

		/// <summary>
		/// Holds the location of the normal log file.
		/// </summary>
		public static string NormalLog {
			get {
				return Path.Combine( LogLocation, "normal.log" );
			}
		}

		/// <summary>
		/// Holds the location of the error log file.
		/// </summary>
		public static string ErrorLog {
			get {
				return Path.Combine( LogLocation, "error.log" );
			}
		}

		/// <summary>
		/// Holds the location of the critical log file.
		/// </summary>
		public static string ExceptionLog {
			get {
				return Path.Combine( LogLocation, "exception.log" );
			}
		}

		#endregion

		private FileStream configStream; // The stream that will read and write to the config file.
		private readonly Dictionary<ConfigurationFields, string> settings;

		/// <summary>
		/// Private PersistenceManager constructor.
		/// </summary>
		private PersistenceManager() {
			settings = new Dictionary<ConfigurationFields, string>();

			if( CheckLocation() ) {
				configStream = File.Open( ConfigFileLocation, FileMode.Open );
			}
		}

		/// <summary>
		/// Retrives the value of the field passed to it.
		/// </summary>
		/// <param name="option">The option that is wanted.</param>
		/// <returns>Returns the value as a string, returns null if value is not in the dictionary.</returns>
		public string GetSettingValue( ConfigurationFields option ) {
			if( settings.ContainsKey( option ) ) {
				return settings[ option ];
			} else {
				return null;
			}
		}

		/// <summary>
		/// Set's a value for the config option.
		/// </summary>
		/// <param name="option">The config option that will represent the value.</param>
		/// <param name="value">The value.</param>
		public void SetSettingValue( ConfigurationFields option, string value ) {
			if( settings.ContainsKey( option ) ) {
				settings[ option ] = value;
			} else {
				settings.Add( option, value );
			}
		}

		// Private Methods

		/// <summary>
		/// Checks and creates if all files and folders are in the persistence location.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if all files and folders are there, false if it had to create any.</returns>
		private bool CheckLocation() {
			// Checks to see if the persistent location does not exist.
			if( !Directory.Exists( PersistentLocation ) ) {
				// Creates all the sub directories needed.
				Directory.CreateDirectory( CacheLocation );
				Directory.CreateDirectory( SaveLocation );
				Directory.CreateDirectory( LogLocation );
				Directory.CreateDirectory( ConfigLocation );
				// Assigns the config stream to the newly created file.
				configStream = File.Create( ConfigFileLocation );

				return false;
			} else if( !Directory.Exists( ConfigLocation ) ) { // Checks to see if the config directory exists.
				// Creates the config directory.
				Directory.CreateDirectory( ConfigLocation );
				// Assigns the config stream to the newly created file.
				configStream = File.Create( ConfigFileLocation );

				return false;
			}

			return true;
		}
	}
}