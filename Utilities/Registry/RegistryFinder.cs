using Microsoft.Win32;

namespace Utilities.Registry {
	public static class RegistryFinder {
		/// <summary>
		/// Finds the name of the item in the registry.
		/// </summary>
		/// <param name="key">The parent to look in.</param>
		/// <param name="name">Name of the registry value.</param>
		/// <returns>The value in that registry entry.</returns>
		public static string FindProgramInRegistry( RegistryKey key, string name ) {
			string[] nameList = key.GetSubKeyNames();

			foreach(string subName in nameList ) {
				RegistryKey regKey = key.OpenSubKey( subName );

				try {
					if( regKey.GetValue( "DisplayName" ).ToString() == name ) {
						return regKey.GetValue( "InstallLocation" ).ToString();
					}
				} catch { }
			}

			return null;
		}
	}
}