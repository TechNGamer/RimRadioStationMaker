using System;
using System.Collections.Generic;

namespace Utilities.Saving {
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
	[Serializable]
	public struct SaveState {
		public string iconPath;
		public string about;
		public string name;
		public List<SongInfo> songs;

		/// <summary>
		/// Checks if obj is a SaveState, and if so just returns the result of the == on this and (SaveState)obj. Otherwise, it returns base.Equals(obj).
		/// </summary>
		/// <param name="obj">The object that it's being compared to.</param>
		/// <returns>True if equals, false otherwise.</returns>
		public override bool Equals( object obj ) {
			if(obj is SaveState ) {
				return this == ( (SaveState)obj );
			} else {
				return base.Equals( obj );
			}
		}

		/// <summary>
		/// Compares two SaveStates to see if their the same.
		/// </summary>
		/// <param name="s1">SaveState 1</param>
		/// <param name="s2">SaveState 2</param>
		/// <returns>True if every this is the same, false otherwise.</returns>
		public static bool operator ==( SaveState s1, SaveState s2 ) {
			if( s1.iconPath == s2.iconPath && s1.about == s2.about && s1.name == s2.name ) {
				return AllSameSongs( s1.songs, s2.songs );
			} else {
				return false;
			}
		}

		// Checks to see if all the songs are the same.
		private static bool AllSameSongs( List<SongInfo> songList1, List<SongInfo> songList2 ) {
			if( songList1.Count != songList2.Count ) {
				return false;
			} else {
				// Sorts both lists so they would be nearly identical.
				songList1.Sort();
				songList2.Sort();

				// Checks to see if both have the same things at the same places.
				for( int i = 0; i < songList1.Count; ++i ) {
					if( songList1[ i ] != songList2[ i ] ) {
						return false;
					}
				}
			}

			return true; // If it gets down here, it returns true.
		}

		/// <summary>
		/// Wanna know something? All I did was <code>return !(s1 == s2)</code>.
		/// </summary>
		/// <param name="s1">SaveState 1</param>
		/// <param name="s2">SaveState 2</param>
		/// <returns>Flip of what == does.</returns>
		public static bool operator !=( SaveState s1, SaveState s2 ) {
			return !( s1 == s2 );
		}
	}
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
}
