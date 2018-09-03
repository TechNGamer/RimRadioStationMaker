using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Logging;
using Utilities.Persistence;
using Utilities.Saving;

namespace Utilities.Cache {
	public enum SizeMeasure {
		Biggest = -1, Byte = 1, KB = 1000, KiB = 1024, MB = 1000000, MiB = 1048576, GB = 1000000000, GiB = 1073741824
	}

	/// <summary>
	/// Manages the cache by coping, moving, and validating.
	/// </summary>
	public class CacheManager {
		/// <summary>
		/// Returns the singleton manager.
		/// </summary>
		public static CacheManager Singleton {
			get {
				if( singleton is null ) {
					singleton = new CacheManager();
				}

				return singleton;
			}
		}
		/// <summary>
		/// Returns the path to the cache location.
		/// </summary>
		public static string CacheLocation {
			get => PersistenceManager.CacheLocation;
		}

		// Holds the reference to the singleton.
		private static CacheManager singleton;

		private readonly Log log;

		// Empty constructor for now.
		private CacheManager() {
			log = new Log( "Utilities.Cache.CacheManager" );

			if( !Directory.Exists( CacheLocation ) ) {
				Directory.CreateDirectory( CacheLocation );
			}
		}

		/// <summary>
		/// Gets the size of the cache in bytes.
		/// </summary>
		/// <returns></returns>
		public long GetCacheSize( string path = "" ) {
			long size = 0; // Holds how much data.
			DirectoryInfo cacheDir;

			path = path is null || path == "" ? CacheLocation : path;

			cacheDir = new DirectoryInfo( path ); // Creates a directory info to get the file infos.

			if( path == "" || path.StartsWith( CacheLocation ) ) {
				foreach( FileSystemInfo fsInfo in cacheDir.GetFileSystemInfos() ) { // Itterates through the files and counting their size.
					log.Message( $"Looking at path: {fsInfo.FullName}." );

					if( fsInfo is FileInfo ) {
						size += ( fsInfo as FileInfo ).Length;

						log.Message( $"It's a file, new calculated size is {size} bytes." );
					} else {
						size += GetCacheSize( ( fsInfo as DirectoryInfo ).FullName );

						log.Message( $"It's a folder, recursion done, new calc size is {size} bytes." );
					}
				}
			} else {
				throw new Exception( "Invalid path. Path must refer to the cache." );
			}

			log.Message( $"Final calculated size is {size} bytes big." );

			return size; // Returns the raw value.
		}

		/// <summary>
		/// Returns the string verson of the size along with the size measurement.
		/// </summary>
		/// <param name="sizeMeasure">The measurement that is wanted.</param>
		/// <returns></returns>
		public string GetCacheSize( SizeMeasure sizeMeasure = SizeMeasure.Biggest ) {
			double size = GetCacheSize( CacheLocation ); // Get's the raw byte size from the other version.
			short times = 0; // Counts how many times it goes through the while loop.

			if( sizeMeasure == SizeMeasure.Biggest ) { // Checks to see if the parameter is set to the biggest.
				while( size / 1024 > 1f ) { // Loops until it's less than the size of the next step.
					++times; // Adds one to times.
					size /= 1024f; // Divides size by 1024.

					log.Message( $"Size can still be devided down. It's been devided {times} times." );
				}
			} else {
				size /= ( int )sizeMeasure; // Get's the division number.

				return $"{size:000.00} {Enum.GetName( sizeMeasure.GetType(), sizeMeasure )}"; // Returns the enum.
			}

			switch( times ) { // Places the size inside the proper string.
				case 0:
					return $"{size:0.00} Byte(s)";
				case 1:
					return $"{size:0.00} KiB";
				case 2:
					return $"{size:0.00} MiB";
				case 3:
					return $"{size:0.00} GiB";
				case 4:
					return $"{size:0.00} TiB";
				default:
					throw new Exception( "Size is bigger than TiB." );
			}
		}

		/// <summary>
		/// Takes everything in the <see cref="SaveState"/> and <see cref="SongInfo"/> and places those items in the cache while also ensuring no wasted space.
		/// </summary>
		/// <param name="saves">The list of saves.</param>
		internal void CacheItems( List<SaveState> saves ) {
			string copyHome;

			for( int i = 0; i < saves.Count; ++i ) {
				SaveState save = saves[ i ];

				if( !Directory.Exists( CacheLocation ) ) {
					Directory.CreateDirectory( CacheLocation );
				}

				if( saves[i].iconPath != null && !saves[ i ].iconPath.StartsWith( CacheLocation ) ) {
					copyHome = Path.Combine( CacheLocation, new FileInfo( saves[ i ].iconPath ).Name );

					try {
						if( saves[ i ].iconPath.StartsWith( CacheLocation ) ) {
							File.Move( saves[ i ].iconPath, copyHome );
						} else {
							File.Copy( saves[ i ].iconPath, copyHome, true );
						}

						save.iconPath = copyHome;
						saves[ i ] = save;
					} catch( Exception e ) {
						log.Exception( $"Path 1: {save.iconPath}\n\tPath 2: {copyHome}", e, false );
					}
				}

				CacheSongs( CacheLocation, ref save.songs );
			}
		}

		public void ClearCache() {
			throw new NotImplementedException();
		}

		#region Private Methods

		// TODO: Work on opimizing cache.
		private void OptimizeCache( ref List<SaveState> saves ) {
			throw new NotImplementedException();
		}

		private void CacheSongs( string profileHome, ref List<SongInfo> songs ) {
			string copyHome;

			for( int i = 0; i < songs.Count; ++i ) {
				if( !songs[ i ].SongPath.StartsWith( profileHome ) ) {
					SongInfo song = songs[ i ];
					copyHome = Path.Combine( profileHome, new FileInfo( songs[ i ].SongPath ).Name );

					try {
						if( songs[ i ].SongPath.StartsWith( CacheLocation ) ) {
							File.Move( song.SongPath, copyHome );
						} else {
							File.Copy( song.SongPath, copyHome, true );
						}

						song.SongPath = copyHome;
						songs[ i ] = song;
					} catch( Exception e ) {
						log.Exception( $"Path 1: {song.SongPath}\n\tPath 2: {copyHome}", e, false );
					}
				}
			}
		}

		// Gathers all the paths in the saves.
		private string[] GetAllPaths( List<SaveState> saves ) {
			List<string> paths = new List<string>();

			foreach( SaveState save in saves ) {
				paths.Add( save.iconPath );

				foreach( SongInfo song in save.songs ) {
					paths.Add( song.SongPath );
				}
			}

			return RemoveRedundent( paths );
		}

		// Removes all the redundent paths.
		private string[] RemoveRedundent( List<string> paths ) {
			for( int i = 0; i < paths.Count; ++i ) {
				for( int j = 0; j < paths.Count; ++j ) {
					if( j != i && paths[ j ] == paths[ i ] ) {
						paths.RemoveAt( j );
					}
				}
			}

			return RemoveCachedItems( paths );
		}

		// Removes items that have already been cached.
		private string[] RemoveCachedItems( List<string> paths ) {
			foreach( string path in paths ) {
				if( path.StartsWith( CacheLocation ) ) {
					paths.Remove( path );
				}
			}

			return paths.ToArray();
		}

		#endregion
	}
}