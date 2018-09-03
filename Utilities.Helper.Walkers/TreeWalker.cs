using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities.Helper.Walkers {
	public static class TreeWalker {

		public static IEnumerable<T> GetVisualTree<T>( DependencyObject parent ) where T : DependencyObject {
			if( parent is null ) {
				throw new NullReferenceException();
			} else {
				for( int i = 0; i < VisualTreeHelper.GetChildrenCount( parent ); ++i ) {
					DependencyObject child = VisualTreeHelper.GetChild( parent, i );

					if( child != null && child is T ) {
						yield return child as T;
					}

					if( VisualTreeHelper.GetChildrenCount( child ) > 0 ) {
						foreach( DependencyObject childOfChild in GetVisualTree<T>( child ) ) {
							yield return childOfChild as T;
						}
					}
				}
			}
		}

		public static IEnumerable<T> GetMenuItems<T>( MenuItem menuItem ) where T : MenuItem {
			if( menuItem is null ) {
				throw new NullReferenceException();
			} else {
				for( int i = 0; i < menuItem.Items.Count; ++i ) {
					if( menuItem.Items[ i ] is T ) {
						yield return menuItem.Items[ i ] as T;
					}

					if(menuItem.Items[i] is MenuItem mI2 && mI2.Items.Count > 0 ) {
						yield return GetMenuItems<T>( mI2 ) as T;
					}
				}
			}
		}

		public static T[] GetVisualTreeArray<T>( DependencyObject parent ) where T : DependencyObject {
			List<T> ts = new List<T>();

			foreach( T t in GetVisualTree<T>( parent ) ) {
				ts.Add( t );
			}

			return ts.ToArray();
		}
	}
}