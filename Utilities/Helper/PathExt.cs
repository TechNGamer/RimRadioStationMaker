using System.IO;

namespace Utilities.Helper {
	public static class PathExt {
		public static string Combine( this string[] paths ) {
			string strToReturn = Path.Combine( paths[ 0 ], paths[ 1 ] );

			for( int i = 2; i < paths.Length; ++i ) {
				strToReturn = Path.Combine( strToReturn, paths[ i ] );
			}

			return strToReturn;
		}
	}
}