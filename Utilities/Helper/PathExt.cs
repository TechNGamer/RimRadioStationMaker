using System.IO;

namespace Utilities.Helper {
	public static class Path {
		public static string Combine( this string[] paths ) {
			string strToReturn = System.IO.Path.Combine( paths[ 0 ], paths[ 1 ] );

			for( int i = 2; i < paths.Length; ++i ) {
				strToReturn = System.IO.Path.Combine( strToReturn, paths[ i ] );
			}

			return strToReturn;
		}
	}
}