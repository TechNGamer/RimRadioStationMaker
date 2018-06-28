using System;
using System.IO;
using System.Linq;

namespace Utilities.Saving {
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
	/// <summary>
	/// SongInfo holds the information about a song for use later.
	/// </summary>
	[Serializable]
	public class SongInfo : IComparable {
		public string Artist { // Holds the name of the artist.
			get;
			set;
		}
		public string Album { // Holds the name of the album.
			get;
			set;
		}
		public string Title { // Holds the name of the song.
			get;
			set;
		}
		public string SongPath { // Holds the path to the song.
			get;
			set;
		}

		public string FileName { // Returns the name of the file.
			get {
				if( SongPath is null || SongPath == "" ) { // Checks if SongPath is null, empty, or contains invalid chars.
					return null;
				} else {
					return SongPath.Substring( SongPath.LastIndexOf( Path.DirectorySeparatorChar ) + 1 ); // Returns the file name.
				}
			}
		}

		/// <summary>
		/// <see cref="IComparable"/> for info on this method.
		/// </summary>
		/// <param name="obj">The object that is being compared to.</param>
		/// <returns>Returns an int about the difference they are.</returns>
		public int CompareTo( object obj ) {
			if( obj is SongInfo ) { // Checks to see if it's a SongInfo object.
				if( this == ( obj as SongInfo ) ) { // Checks to see if the object is itself.
					return 0;
				} else if( Title != ( obj as SongInfo ).Title ) { // Checks to see if titles are different.
					return Title.CompareTo( ( obj as SongInfo ).Title );
				} else if( Album != ( obj as SongInfo ).Album ) { // Checks to see if albums are different.
					return Album.CompareTo( ( obj as SongInfo ).Album );
				} else if( Artist != ( obj as SongInfo ).Artist ) { // Checks to see if artists are different.
					return Artist.CompareTo( ( obj as SongInfo ).Artist );
				} else if( FileName != ( obj as SongInfo ).FileName ) { // Checks if file names are different.
					return FileName.CompareTo( ( obj as SongInfo ).FileName );
				} else if( SongPath != ( obj as SongInfo ).SongPath ) { // Checks if song paths are different.
					return SongPath.CompareTo( ( obj as SongInfo ).SongPath );
				} else { // Throws an exception because there the same SongInfo.
					throw new Exception( "ÒwÓ How the fuck!? Something must be wrong with C#/the interpeter!" );
				}
			} else { // Returns 0 because why not?
				return 0;
			}
		}

		/// <summary>
		/// <see cref="Object"/> to learn about this.
		/// </summary>
		/// <param name="obj">Object to compare to.</param>
		/// <returns><see langword="true"/> if the same, false otherwise.</returns>
		public override bool Equals( object obj ) {
			if( obj is SongInfo ) { // Checks if obj is a SongInfo.
				return this == obj as SongInfo; // Returns the == operator's results.
			} else {
				return base.Equals( obj ); // Calls the base Equals and returns that.
			}
		}

		/// <summary>
		/// Checks to see if they are equal or not. <seealso cref="Equals(object)"/>
		/// </summary>
		/// <param name="s1">The first SongInfo to compare to.</param>
		/// <param name="s2">The second SongInfo to compare to.</param>
		/// <returns>Returns <see langword="true"/> if they are the same, otherwise <see langword="false"/></returns>
		public static bool operator ==( SongInfo s1, SongInfo s2 ) {
			return s1.Artist == s2.Artist && s1.Album == s2.Album && s1.Title == s2.Title && s1.SongPath == s2.SongPath;
		}

		/// <summary>
		/// Does the flip of the == operator.
		/// </summary>
		/// <param name="s1">The first SongInfo to compare to.</param>
		/// <param name="s2">The second SongInfo to compare to.</param>
		/// <returns>Returns <see langword="true"/> if their different, otherwise <see langword="false"/>.</returns>
		public static bool operator !=( SongInfo s1, SongInfo s2 ) {
			return !( s1 == s2 ); // Just binary invert the == result.
		}
	}
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
