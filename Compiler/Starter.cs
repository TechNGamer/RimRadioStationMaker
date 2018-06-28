using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Common;
using Microsoft.CSharp;
using Newtonsoft.Json;
using Utilities.Logging;
using Utilities.Saving;

namespace Compiler {
	public static class Starter {
		public static readonly string[] NEEDED_FOLDERS = { "Assemblies", "About", "Sounds", "Textures" };

		public static readonly string RESOURCE_FOLDER_PATH = Path.Combine( Environment.CurrentDirectory, "Resources" ),
			DOCS_FOLDER = Path.Combine( RESOURCE_FOLDER_PATH, "Docs" ),
			HELP_FILE_PATH = Path.Combine( DOCS_FOLDER, "help.txt" ),
			RAW_SCRIPTS = Path.Combine( RESOURCE_FOLDER_PATH, "RawScripts" ),
			RAW_CSHARP_SCRIPT = Path.Combine( RAW_SCRIPTS, "MusicPack.cs" ),
			ASSEMBLY_FOLDER = Path.Combine( RESOURCE_FOLDER_PATH, "Assembly" ),
			RIMRADIO_ASSEMBLY = Path.Combine( ASSEMBLY_FOLDER, "RimRadio.dll" );

		public static readonly CompilerParameters PARAMETERS = new CompilerParameters();

		private static bool verbose = false, runRimWorld = false;
		private static string saveJsonLoc, gameLoc, gameAssembly, unityAssembly, modsFolder;

		private static List<string> names = new List<string>();

		public static void Main() {
			try {
				ProcessArgs();

				if( verbose ) {
					Console.WriteLine( "Done processing arguments. Continueing." );
				}

				PARAMETERS.ReferencedAssemblies.Add( RIMRADIO_ASSEMBLY );
				PARAMETERS.ReferencedAssemblies.Add( gameAssembly );
				PARAMETERS.ReferencedAssemblies.Add( unityAssembly );
				PARAMETERS.ReferencedAssemblies.Add( typeof( Path ).Assembly.Location );
				PARAMETERS.GenerateExecutable = false;
				PARAMETERS.CompilerOptions = "/optimize";

				RunCompiler();
			} catch( Exception e ) {
				Console.WriteLine( $"{e.Message}\n{e.StackTrace}" );
			}

			new Log( "null" ).ProcessExit( null, new EventArgs() );

			Console.Write( "Press any key to continue. . ." );
			Console.ReadKey( true );
		}

		private static void RunCompiler() {
			CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			SaveManager saveManager = SaveManager.Singleton;
			SaveState saveState;
			string fileContent, modFolder;

			for( int i = 0; i < names.Count; ++i ) {

				saveState = saveManager[ names[ i ] ];
				names[ i ] = names[ i ].Replace( ' ', '_' );

				if( verbose ) {
					Console.WriteLine( $"Processing profile '{i} - {saveState.name}'" );
				}

				modFolder = CreateOutputFolder( names[ i ] );

				if( verbose ) {
					Console.WriteLine( "Reading raw C# file into memory for modifications." );
				}

				fileContent = File.ReadAllText( RAW_CSHARP_SCRIPT );
				fileContent = fileContent.Replace( "MusicPackName", names[ i ] );

				if( verbose ) {
					Console.WriteLine( "Preparing to compile code to assembly file." );
				}

				if( File.Exists( PARAMETERS.OutputAssembly ) ) {
					File.Delete( PARAMETERS.OutputAssembly );
				}

				PARAMETERS.OutputAssembly = Path.Combine( Path.Combine( modFolder, NEEDED_FOLDERS[ 0 ] ), $"{names[ i ]}.dll" );

				CompilerResults results = codeProvider.CompileAssemblyFromSource( PARAMETERS, fileContent );

				if( results.Errors.Count > 0 ) {
					foreach( CompilerError error in results.Errors ) {

						Console.ForegroundColor = error.IsWarning ? ConsoleColor.Yellow : ConsoleColor.Red;

						Console.WriteLine( $"Error Number: {error.ErrorNumber}\nError Message: {error.ErrorText}\nLine: {error.Line} Column: {error.Column}" );
					}

					Console.ResetColor();
				}

				CopyFiles( saveState, modFolder );

				CreateRRSFile( modFolder, saveState );
			}
		}

		private static void CreateRRSFile( string modFolder, SaveState saveState ) {
			BinaryFormatter formatter = new BinaryFormatter();

			RawStation rawStation = new RawStation() {
				stationName = saveState.name,
				iconFileName = saveState.iconPath.Substring( saveState.iconPath.LastIndexOf( Path.DirectorySeparatorChar ) + 1 )
			};

			JsonSerializerSettings settings = new JsonSerializerSettings() {
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Include
			};

			modFolder = Path.Combine( modFolder, "Sounds" );

			foreach( SongInfo song in saveState.songs ) {
				RawSong rSong = new RawSong() {
					album = song.Album,
					artist = song.Artist,
					title = song.Title,
					fileName = song.FileName
				};

				rawStation.rawSongs.Add( rSong );
			}

			modFolder = Path.Combine( modFolder, "music.rrb" );

			formatter.Serialize( File.Create( modFolder ), rawStation );
		}

		private static void CopyFiles( SaveState saveState, string modFolder ) {
			string tempStr;

			foreach( SongInfo song in saveState.songs ) {
				tempStr = Path.Combine( Path.Combine( modFolder, "Sounds" ), song.FileName );

				if( verbose ) {
					Console.WriteLine( $"Coping file '{song.SongPath}' to '{tempStr}'" );
				}

				try {
					File.Copy( song.SongPath, tempStr );
				} catch( Exception e ) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine( $"{e.Message}\n Stacktrace: {e.StackTrace}" );
					Console.ResetColor();
				}
			}

			tempStr = Path.Combine( Path.Combine( modFolder, "Textures" ), saveState.iconPath.Substring( saveState.iconPath.LastIndexOf( Path.DirectorySeparatorChar ) + 1 ) );

			if( verbose ) {
				Console.WriteLine( $"Coping file '{saveState.iconPath}' to '{tempStr}'" );
			}

			try {
				File.Copy( saveState.iconPath, tempStr );
			} catch( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( $"{e.Message}\n Stacktrace: {e.StackTrace}" );
				Console.ResetColor();
			}
		}

		private static string CreateOutputFolder( string name ) {
			string modFolder = Path.Combine( modsFolder, name ), subFolder;

			if( verbose ) {
				Console.WriteLine( $"Looking for directory: {modFolder}" );
			}

			if( !Directory.Exists( modFolder ) ) {
				Console.WriteLine( "UwU Directory does not exist, creating." );

				Directory.CreateDirectory( modFolder );
			} else if( verbose ) {
				Console.WriteLine( "OwO Directory does exist." );
			}

			foreach( string folder in NEEDED_FOLDERS ) {
				subFolder = Path.Combine( modFolder, folder );

				if( verbose ) {
					Console.WriteLine( $"Checking to see if '{subFolder}' exists." );
				}

				if( !Directory.Exists( subFolder ) ) {
					if( verbose ) {
						Console.WriteLine( "UwU Directory does not exist, creating." );
					}

					Directory.CreateDirectory( subFolder );
				} else if( verbose ) {
					Console.WriteLine( "OwO Directory exists." );
				}
			}

			return modFolder;
		}

		private static void ProcessArgs() {
			if( Environment.GetCommandLineArgs().Length > 1 ) {
				for( int i = 1; i < Environment.GetCommandLineArgs().Length; ++i ) {
					string arg = Environment.GetCommandLineArgs()[ i ];
					if( arg.StartsWith( "-" ) && !arg.StartsWith( "--" ) ) {
						SingleVarProcessor( arg.ToCharArray() );
					} else {
						switch( arg ) {
							case "--verbose":
								verbose = true;
								break;
							case "--run":
								runRimWorld = true;
								break;
							case "--project":
								GetProjects( ref i );
								--i;
								break;
							case "--save-json":
								saveJsonLoc = Environment.GetCommandLineArgs()[ ++i ];
								break;
							case "--rimworld-loc":
								gameLoc = Environment.GetCommandLineArgs()[ ++i ];
								gameAssembly = Path.Combine( Path.Combine( Path.Combine( gameLoc, "RimWorldWin_Data" ), "Managed" ), "Assembly-CSharp.dll" );
								unityAssembly = Path.Combine( Path.Combine( Path.Combine( gameLoc, "RimWorldWin_Data" ), "Managed" ), "UnityEngine.dll" );
								modsFolder = Path.Combine( gameLoc, "Mods" );
								break;
							case "--help":
							default:
								PrintHelp();
								break;
						}
					}
				}
			} else {
				PrintHelp();
			}
		}

		private static void GetProjects( ref int startLocation ) {
			string[] args = Environment.GetCommandLineArgs();

			while( !args[ ++startLocation ].StartsWith( "-" ) ) {
				names.Add( args[ startLocation ] );
			}
		}

		private static void SingleVarProcessor( char[] args ) {
			for( int i = 1; i < args.Length; ++i ) {
				switch( args[ i ] ) {
					case 'r':
						runRimWorld = true;
						break;
					case 'v':
						verbose = true;
						break;
					case 'h':
					default:
						PrintHelp();
						return;
				}
			}
		}

		private static void PrintHelp() {
			Console.Write( File.ReadAllText( HELP_FILE_PATH ) );
			Environment.Exit( -1 );
		}
	}
}