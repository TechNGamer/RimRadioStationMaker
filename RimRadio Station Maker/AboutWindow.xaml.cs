using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RimRadioStationMaker {
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window {
		public AboutWindow() {
			InitializeComponent();

			image.Source = new BitmapImage( new Uri( Path.Combine( $"file://", Environment.CurrentDirectory, "Resources", "Icons", "RRSM.png" ) ) );
		}
	}
}
