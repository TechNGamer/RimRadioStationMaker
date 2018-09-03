using System;
using System.Collections.Generic;
using System.ComponentModel;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Win32;

using Utilities.Cache;
using Utilities.Helper.Walkers;
using Utilities.Logging;
using Utilities.Registry;
using Utilities.Resources.Language;
using Utilities.Saving;

/* Commenting done comment created 1:07am on 6/24/2018.
 ▄▄▄▄▄▄▄▄▄▄▄  ▄         ▄  ▄▄▄▄▄▄▄▄▄▄▄ 
▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░░░░░░░░░░░▌
▐░█▀▀▀▀▀▀▀█░▌▐░▌       ▐░▌▐░█▀▀▀▀▀▀▀█░▌
▐░▌       ▐░▌▐░▌       ▐░▌▐░▌       ▐░▌
▐░▌       ▐░▌▐░▌   ▄   ▐░▌▐░▌       ▐░▌
▐░▌       ▐░▌▐░▌  ▐░▌  ▐░▌▐░▌       ▐░▌
▐░▌       ▐░▌▐░▌ ▐░▌░▌ ▐░▌▐░▌       ▐░▌
▐░▌       ▐░▌▐░▌▐░▌ ▐░▌▐░▌▐░▌       ▐░▌
▐░█▄▄▄▄▄▄▄█░▌▐░▌░▌   ▐░▐░▌▐░█▄▄▄▄▄▄▄█░▌
▐░░░░░░░░░░░▌▐░░▌     ▐░░▌▐░░░░░░░░░░░▌
 ▀▀▀▀▀▀▀▀▀▀▀  ▀▀       ▀▀  ▀▀▀▀▀▀▀▀▀▀▀

 ▄▄▄▄▄▄▄▄▄▄▄  ▄▄       ▄▄       ▄▄▄▄▄▄▄▄▄▄▄       ▄▄▄▄▄▄▄▄▄▄▄  ▄         ▄  ▄▄▄▄▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄▄▄▄  ▄         ▄  ▄ 
▐░░░░░░░░░░░▌▐░░▌     ▐░░▌     ▐░░░░░░░░░░░▌     ▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░▌
 ▀▀▀▀█░█▀▀▀▀ ▐░▌░▌   ▐░▐░▌     ▐░█▀▀▀▀▀▀▀█░▌     ▐░█▀▀▀▀▀▀▀▀▀ ▐░▌       ▐░▌▐░█▀▀▀▀▀▀▀█░▌▐░█▀▀▀▀▀▀▀█░▌▐░▌       ▐░▌▐░▌
     ▐░▌     ▐░▌▐░▌ ▐░▌▐░▌     ▐░▌       ▐░▌     ▐░▌          ▐░▌       ▐░▌▐░▌       ▐░▌▐░▌       ▐░▌▐░▌       ▐░▌▐░▌
     ▐░▌     ▐░▌ ▐░▐░▌ ▐░▌     ▐░█▄▄▄▄▄▄▄█░▌     ▐░█▄▄▄▄▄▄▄▄▄ ▐░▌       ▐░▌▐░█▄▄▄▄▄▄▄█░▌▐░█▄▄▄▄▄▄▄█░▌▐░█▄▄▄▄▄▄▄█░▌▐░▌
     ▐░▌     ▐░▌  ▐░▌  ▐░▌     ▐░░░░░░░░░░░▌     ▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░▌
     ▐░▌     ▐░▌   ▀   ▐░▌     ▐░█▀▀▀▀▀▀▀█░▌     ▐░█▀▀▀▀▀▀▀▀▀ ▐░▌       ▐░▌▐░█▀▀▀▀█░█▀▀ ▐░█▀▀▀▀█░█▀▀  ▀▀▀▀█░█▀▀▀▀ ▐░▌
     ▐░▌     ▐░▌       ▐░▌     ▐░▌       ▐░▌     ▐░▌          ▐░▌       ▐░▌▐░▌     ▐░▌  ▐░▌     ▐░▌       ▐░▌      ▀
 ▄▄▄▄█░█▄▄▄▄ ▐░▌       ▐░▌     ▐░▌       ▐░▌     ▐░▌          ▐░█▄▄▄▄▄▄▄█░▌▐░▌      ▐░▌ ▐░▌      ▐░▌      ▐░▌      ▄
▐░░░░░░░░░░░▌▐░▌       ▐░▌     ▐░▌       ▐░▌     ▐░▌          ▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░▌       ▐░▌     ▐░▌     ▐░▌
 ▀▀▀▀▀▀▀▀▀▀▀  ▀         ▀       ▀         ▀       ▀            ▀▀▀▀▀▀▀▀▀▀▀  ▀         ▀  ▀         ▀       ▀       ▀
 */

namespace RimRadioStationMaker {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private string currentIconPath;
		private bool editing;
		private int selection;

		private OpenFileDialog imageDialog, songFileDialog;
		private SongInfo song = new SongInfo();
		private Log log;

		private string emptyIconPath = Path.Combine( Environment.CurrentDirectory, "Resources", "Icons", "RRSM.png" );

		private ImageSource emptyIcon;
		private AboutWindow aboutWindow;

		// Singletons
		private SaveManager saveManager = SaveManager.Singleton;
		private CacheManager cacheManager = CacheManager.Singleton;
		private LanguageManager languageManager = LanguageManager.Singleton;

		public MainWindow() {

			InitializeComponent(); // Initializes the components. See the Microsoft .NET 4.5 docs on WPF.

			CreateObjectsAndAssign(); // Creates objects that are needed and also assigns what needs to be.

			PopulateOpenMenu(); // Populates the open menu item of all the saves.

			log.Message( "Initialization complete." ); // Logs that everything has finished initalizing.
		}

		#region Event Methods

		private void GetLanguage( object sender, RoutedEventArgs e ) {
			foreach( Visual v in TreeWalker.GetVisualTree<Visual>( this ) ) {
				log.Message( $"Visual v is of type '{v.GetType()}'." );

				if( v is Control c ) {
					log.Message( $"Visual v name is '{c.Name}'." );
				}

				if( v is GroupBox gP ) {
					AssignLanguageForGroupBox( gP );
				} else if( v is TextBlock tBl ) {
					AssignLanguageForTextBlock( tBl );
				} else if( v is TextBox tBx ) {
					AssignLanguageForTextBox( tBx );
				} else if( v is MenuItem mI ) {
					AssignLanguageForMenuItem( mI );
				} else if( v is ContentControl cC ) {
					AssignLanguageForContentControl( cC );
				}
			}
		}

		// Opens the proper save state for assigning.
		private void OpenMenuEntryClicked( object sender, RoutedEventArgs e ) {
			SaveState save;
			string tempPath;

			if( sender is MenuItem ) { // Checks to make sure sender is a MenuItem.
				foreach( MenuItem menuItem in openFileMenuItem.Items ) { // Itterates through the items in the open menu item.
					if( menuItem.Header as string == ( sender as MenuItem ).Header as string ) { // Checks to see if there headers are the same.
						save = saveManager[ menuItem.Header as string ]; // Pulls the save from the SaveManager.
						tempPath = currentIconPath;

						currentIconPath = save.iconPath; // Assigns the current icon path to the one in the save object.

						try {
							ApplyImage(); // Applys the new image.
						} catch( ArgumentNullException e1 ) {
							// Tells the user that their is no thumbnail.
							MessageBox.Show( this, "You must have an thumbnail to show.", "No thumbnail", MessageBoxButton.OK, MessageBoxImage.Error );
							log.Exception( "No thumbnail.", e1, false ); // Logs the exception.

							currentIconPath = tempPath; // Resets the image path.
						} catch( Exception e1 ) {
							// Tells the user that an unexpected exception happened of unknown cause.
							MessageBox.Show( this, $"Something happened\nException: {e1.GetType().FullName}", "Something happened", MessageBoxButton.OK, MessageBoxImage.Error );
							log.Exception( "Something happened while applying the image, and thus, this exception.", e1, false ); // Logs the exception.

							currentIconPath = tempPath; // Resets the image path.
							return; // Returns to make sure no damage has been done.
						}

						aboutTextBox.Text = save.about; // Assigns the data in the about text box to the one in the save object.
						stationNameBox.Text = save.name; // Assigns the stationNameBox text to the one in the save object.

						PopulateGrid( save.songs ); // Populates the grid.
					}
				}
			}
		}

		// Overrides the base method to check if the user wants to close.
		protected override void OnClosing( CancelEventArgs e ) {
			MessageBoxResult result;
			log.Message( "Checking if user want's to close." ); // Logs a message, duh. -w-

			// Asks the user if they really want to quit the program.
			result = MessageBox.Show( this, "Do you want to save all data before exiting?", "OwO Notices you closing.",
				MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Yes );

			if( result == MessageBoxResult.Yes ) { // If the user clicks yes, the program procedes to save any data and closes.
				SaveMenuClicked( null, new RoutedEventArgs() ); // Calls the method that already handles saving.
			} else if( result == MessageBoxResult.Cancel ) { // Verify's the response back.
				e.Cancel = true; // If response is no, it aborts the closing process.
				return;
			}

			log.ProcessExit( null, new EventArgs() );
			Application.Current.Shutdown();

			base.OnClosing( e );
		}

		// Used to get the icon that will represent the radio station.
		private void SelectImageClick( object sender, RoutedEventArgs e ) {
			bool? result = imageDialog.ShowDialog( this ); // Gets a nullable bool from the dialog on if it was successful.

			if( !( result is null ) && ( bool )result ) { // Checks to make sure that the result is not null and that it's true.
				log.Message( $"{imageDialog.FileName} is valid." ); // Again, logs a message. -w-

				currentIconPath = imageDialog.FileName; // Assigns the new image.

				ApplyImage(); // Apply's the new image.
			}
		}

		// Used to add the info that the use put in to the grid.
		private void AddSong( object sender, RoutedEventArgs e ) {
			// Calls a helper method that checks if any text boxes are empty or null.
			if( AnyTextBoxEmpty() ) {
				log.Message( "Not all fields have a value." ); // Do I have to comment all of these?

				// Tels the use that not all text boxes have data in them.
				MessageBox.Show( this, "Please make sure all fields have a value.", "UwU Notice Empty Field", MessageBoxButton.OK, MessageBoxImage.Error );
			} else if( editing ) {
				CommitChange(); // Calls the helper method.
			} else {
				AddSongToGrid(); // Calls the helper method.
			}
		}

		// Used to edit the song.
		private void EditSong( object sender, RoutedEventArgs e ) {
			SongInfo song;
			if( rrsDataGrid.Items.Count > 0 && rrsDataGrid.SelectedIndex > -1 ) { // Checks to see if the grid is empty or not and if selected index is not -1.
				selection = rrsDataGrid.SelectedIndex; // Assigns the selected index to a variable for use later.

				song = rrsDataGrid.Items[ selection ] as SongInfo; // Uses the temp variable to store the song so no memory is wasted.

				// Stores all the data in the song in the text boxes for modifying.
				songArtistInput.Text = song.Artist;
				songAlbumInput.Text = song.Album;
				songTitleInput.Text = song.Title;
				songFilePathInput.Text = song.FileName;

				songFileDialog.FileName = song.SongPath; // Adds the song file's path the dialog for safe keeping.

				editing = true; // Enables a flag.

				addSongButton.Content = "Commit Change"; // Changes the content of the button.
			} else {
				// Tells the user that they muse select a song for editing.
				MessageBox.Show( this, "You need to create a song entry in order for you to edit it.", "UwU No song selectable", MessageBoxButton.OK, MessageBoxImage.Exclamation );
			}
		}

		// Used to remove a song.
		private void RemoveSong( object sender, RoutedEventArgs e ) {
			if( rrsDataGrid.SelectedIndex > -1 ) { // Checks to make sure that the selected index is not -1.
				rrsDataGrid.Items.RemoveAt( rrsDataGrid.SelectedIndex ); // Proceeds to remove the song at the selected index.

				if( editing ) { // If it's in edit mode, it switches back to regular mode.
					CreateNewSong(); // Calls the method that makes a new song info object.
				}
			} else {
				// Tells the user that they cannot delete a song entry unless they select one.
				MessageBox.Show( this, "You need to create a song entry in order for you to delete it.", "UwU No song selectable", MessageBoxButton.OK, MessageBoxImage.Exclamation );
			}
		}

		// Used to choose the location of the song file.
		private void ChooseSongFile( object sender, RoutedEventArgs e ) {
			FileInfo file;
			bool? result = songFileDialog.ShowDialog( this ); // Gets a nullable bool from the dialog.

			if( !( result is null ) && ( bool )result ) { // Checks to ensure that the bool is not null and is true.
				file = new FileInfo( songFileDialog.FileName ); // Creates a FileInfo object for easier use.

				song.SongPath = file.FullName; // Uses the full path of the song file.
				songFilePathInput.Text = file.Name; // Puts the song file's name in the text box.

				songFileDialog.InitialDirectory = file.DirectoryName; // Links to where the user last was.
			}
		}

		// Used to close the program from the menu entry.
		private void CloseItemClick( object sender, RoutedEventArgs e ) {
			Close();
		}

		// Used to save the current state of the program/
		private void SaveMenuClicked( object sender, RoutedEventArgs e ) {
			SaveState save = new SaveState() { // Creates a new SaveState with all the data in it.
				about = aboutTextBox.Text,
				name = stationNameBox.Text,
				iconPath = currentIconPath,
				songs = new List<SongInfo>()
			};

			// Itterates through the grid and adds them all to the save state.
			foreach( object song in rrsDataGrid.Items ) {
				save.songs.Add( song as SongInfo );
			}

			saveManager.WriteSave( save ); // Calls the WriteSave function of the saveManager object.

			PopulateOpenMenu(); // Repopulates the open menu.
		}

		private void RunClicked( object sender, RoutedEventArgs e ) {
			Process compileProcess = new Process {
				StartInfo = new ProcessStartInfo() {
					Arguments = $"-rv --project \"{stationNameBox.Text}\" --save-json \"{SaveManager.SaveFile}\" --rimworld-loc \"{RegistryFinder.FindProgramInRegistry( RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Default ).OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall" ), "RimWorld" )}\"",
					UseShellExecute = true,
					FileName = "RRCSH-Compiler.exe"
					//Verb = "runas"
				}
			};

			SaveMenuClicked( null, new RoutedEventArgs() );

			compileProcess.Start();
		}

		private void AboutMenuClick( object sender, RoutedEventArgs e ) {
			aboutWindow.Show();
		}

		private void ClearCacheClicked( object sender, RoutedEventArgs e ) {
			MessageBoxResult result = MessageBox.Show( this, "OwO Are you sure you want to clear the cache, doing so will also delete saves.json causing you to lose all your SaveStates.",
				"OwO Attention Needed!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No );

			if( result == MessageBoxResult.Yes ) {
				try {
					cacheManager.ClearCache();
					MessageBox.Show( this, "UwU Cache cleared master.", "Cache has been cleared.", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK );
				} catch( Exception e1 ) {
					MessageBox.Show( this, "UwU Sorry master, cache was unable to be cleared. Some corruption and missing refrences might of occured. Please do not be mad, just restart the computer and try again. Maybe another program is using those files?", "UwU Unable to clear cache", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK );
					log.Exception( "An error has occured.", e1, false );
				}
			}
		}

		#endregion

		#region Helper Methods

		private void AssignLanguageForGroupBox( GroupBox groupBox ) {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, groupBox.Name ) ) {
				groupBox.Header = languageManager[ languageManager.LanguageUsed, groupBox.Name ];
				log.Message( $"GroupBox '{groupBox.Name}' lang was found, it now displays, '{groupBox.Header}'." );
			} else {
				log.Message( $"No lang entry found for '{groupBox.Name}'." );
			}
		}

		private void AssignLanguageForTextBlock( TextBlock textBlock ) {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, textBlock.Name ) ) {
				textBlock.Text = languageManager[ languageManager.LanguageUsed, textBlock.Name ];
				log.Message( $"TextBlock '{textBlock.Name}' lang was found, it now displays, '{textBlock.Text}'." );
			} else {
				log.Message( $"No lang entry found for '{textBlock.Name}'." );
			}
		}

		private void AssignLanguageForTextBox( TextBox textBox ) {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, textBox.Name ) ) {
				textBox.Text = languageManager[ languageManager.LanguageUsed, textBox.Name ];
				log.Message( $"TextBlock '{textBox.Name}' lang was found, it now displays, '{textBox.Text}'." );
			} else {
				log.Message( $"No lang entry found for '{textBox.Name}'." );
			}
		}

		private void AssignLanguageForContentControl( ContentControl contentControl ) {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, contentControl.Name ) ) {
				contentControl.Content = languageManager[ languageManager.LanguageUsed, contentControl.Name ];
				log.Message( $"ContentControl '{contentControl.Name}' lang was found, it now displays, '{contentControl.Content}'." );
			} else {
				log.Message( $"No lang entry found for '{contentControl.Name}'." );
			}
		}

		private void AssignLanguageForMenuItem( params MenuItem[] menuItems ) {
			foreach( MenuItem menuItem in menuItems ) {
				AssignLanguageForMenuItem( menuItem );
			}
		}

		private void AssignLanguageForMenuItem( MenuItem menuItem ) {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, menuItem.Name ) ) {
				menuItem.Header = languageManager[ languageManager.LanguageUsed, menuItem.Name ];
				log.Message( $"MenuItem '{menuItem.Name}' lang was found, it now displays, '{menuItem.Header}'." );
			} else {
				log.Message( $"No lang entry found for '{menuItem.Name}'." );
			}
		}

		private void CreateObjectsAndAssign() {
			log = new Log( "RimRadioStationMaker.MainWindow" ); // Creates a log object to enable logging to the log files.

			emptyIcon = new BitmapImage( new Uri( Path.Combine( $"file://", emptyIconPath ) ) ); // Gets a new imgae for the icon UI Object.

			aboutWindow = new AboutWindow();

			// Creates a new dialog for opening images.
			imageDialog = new OpenFileDialog {
				Filter = "Image Files|*.png;*.bitmap;*.jpg;*.jpeg;*.bmp", // Filter provides only those specific files.
				InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ), // Defaults to the picture directory.
				Multiselect = false, // Disables multiselect.
				CheckFileExists = true, // Enables checking that the file actually exists.
				CheckPathExists = true // Checks to make sure the path exists.
			};

			// Creates a new dialog for opening music files.
			songFileDialog = new OpenFileDialog {
				Filter = "Unity Supported Audio|*.wav;*.ogg", // Filter provides only files that Unity can open at runtime.
				InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ), // Sets the inital directory to the music folder at start.
				Multiselect = false, // Disables multiselect.
				CheckFileExists = true, // Enables checking that the file actually exists.
				CheckPathExists = true // Enables checking that the file actually exists.
			};

			stationIcon.Source = emptyIcon; // Displays the image.

			PopulateLanguageList();

			ChangeLanguage();

			Loaded += GetLanguage;
		}

		private void PopulateLanguageList() {
			MenuItem newMenuItem;

			foreach( string key in languageManager.Keys ) {
				newMenuItem = new MenuItem {
					Header = key
				};

				newMenuItem.Click += ChangeLanguageTo;

				changeLanguageMenuItem.Items.Add( newMenuItem );
			}
		}

		private void ChangeLanguageTo( object sender, RoutedEventArgs e ) {
			if( sender is MenuItem mI && !mI.IsChecked ) {
				languageManager.LanguageUsed = mI.Header as string;

				foreach( MenuItem menuItem in changeLanguageMenuItem.Items ) {
					if( menuItem.Header as string == languageManager.LanguageUsed ) {
						menuItem.IsChecked = true;
					} else {
						menuItem.IsChecked = false;
					}
				}

				ChangeLanguage();
			}
		}

		private void ChangeLanguage() {
			if( languageManager.ContainsKey( languageManager.LanguageUsed, clearCacheMenuItem.Name ) ) {
				clearCacheMenuItem.Header = $"{languageManager[ languageManager.LanguageUsed, clearCacheMenuItem.Name ]}\n{cacheManager.GetCacheSize( SizeMeasure.Biggest )}";
			}

			if( languageManager.ContainsKey( languageManager.LanguageUsed, changeLanguageMenuItem.Name ) ) {
				changeLanguageMenuItem.Header = languageManager[ languageManager.LanguageUsed, changeLanguageMenuItem.Name ];
			}

			AssignLanguageForMenuItem( openFileMenuItem, saveFileMenuItem, closeMenuItem );
			GetLanguage( this, new RoutedEventArgs() );
		}

		// Adds a song to the grid for displaying and safe keeping.
		private void AddSongToGrid() {
			// Assigns all the proper data.
			song.Artist = songArtistInput.Text;
			song.Album = songAlbumInput.Text;
			song.Title = songTitleInput.Text;

			// Itterates through the grid to see if another copy does not already exist.
			foreach( SongInfo iSong in rrsDataGrid.Items ) {
				if( iSong.SongPath == song.SongPath ) { // Checks to see if it's a copy.
					log.Message( "Song already exists." ); // Logs the message. -w-

					// Displays a message box telling the user that it already exists.
					MessageBox.Show( this, "A song with that same file exist. Please select a different file.", "UwU Notices diplication.", MessageBoxButton.OK, MessageBoxImage.Exclamation );
					return; // Returns to abort the execution of the rest of the method.
				}
			}

			rrsDataGrid.Items.Add( song ); // Adds the song to the grid.
			CreateNewSong(); // Creates a new song object.

			log.Message( "Added and created a new SongInfo object." ); // Logs the message. -w-
		}

		// Checks to make sure all text fields for information on the song are valid.
		private bool AnyTextBoxEmpty() {
			return songTitleInput.Text is null || songTitleInput.Text == "" || songArtistInput.Text is null || songArtistInput.Text == ""
				|| songAlbumInput.Text is null || songAlbumInput.Text == "" || songFilePathInput.Text is null || songFilePathInput.Text == "";
		}

		// Creates a new song object.
		private void CreateNewSong() {
			// Creates and assigns all the relivent data.
			song = new SongInfo() {
				Artist = song.Artist,
				Album = song.Album,
				Title = song.Title,
				SongPath = song.SongPath
			};

			editing = false; // Disables edit mode.

			// Creates empty text fields.
			songArtistInput.Text = "";
			songAlbumInput.Text = "";
			songTitleInput.Text = "";
			songFilePathInput.Text = "";
		}

		// Used for edit mode only.
		private void CommitChange() {
			log.Message( "Saving changes." ); // Logs the message. -w-
			song = rrsDataGrid.Items[ selection ] as SongInfo; // Creates a reference to the object.

			// Assigns all proper data.
			song.Artist = songArtistInput.Text;
			song.Album = songAlbumInput.Text;
			song.Title = songTitleInput.Text;
			song.SongPath = songFileDialog.FileName;

			// Changes the content of the add button back to "Add Song".
			addSongButton.Content = "Add Song";

			// Refreshes the grid view.
			rrsDataGrid.Items.Refresh();

			// Calls this method.
			CreateNewSong();
		}

		// Applies a new image to the icon.
		private void ApplyImage() {
			BitmapImage img = new BitmapImage(); // Creates a new BitmapImage object.

			// Begins loading the image into memory.
			img.BeginInit();
			img.CacheOption = BitmapCacheOption.OnLoad;
			img.UriSource = new Uri( Path.Combine( "file:///", currentIconPath ) );
			img.EndInit();
			// Ends loading the image into memory.

			stationIcon.Source = img; // Gets the icon and stores it.

			imageDialog.InitialDirectory = new FileInfo( currentIconPath ).DirectoryName; // Sets the directory to where it came from.
		}

		// Does as the name suggests. Populates the open menu item with all the saves.
		private void PopulateOpenMenu() {
			if( saveManager.Count > 0 ) { // Checks to see if there are any saves.
				foreach( SaveState save in saveManager ) { // Itterates through all the saves that SaveManager has.
					MenuItem newMenuItem = new MenuItem() { // Creates a new menu item with the name of the save as the header.
						Header = save.name
					};

					newMenuItem.Click += OpenMenuEntryClicked; // Links its click event to that method.

					openFileMenuItem.Items.Add( newMenuItem ); // Adds it to the open file menu item's collection.
				}
			} else {
				openFileMenuItem.IsEnabled = false; // Disables the menu.
			}
		}

		// Populates the grid with the songs in the list.
		private void PopulateGrid( List<SongInfo> songs ) {
			foreach( SongInfo song in songs ) {
				rrsDataGrid.Items.Add( song );
			}
		}

		#endregion
	}
}