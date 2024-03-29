﻿using System;
using System.Text;
using System.Windows.Forms;

namespace Utilities.Logging {
	public sealed class Log {
		public static string LogLocation {
			get => LogManager.LogLocation;
		}

		public static string NormalLogLocation {
			get => LogManager.NormalLogLocation;
		}

		public static string ErrorLogLocation {
			get => LogManager.ErrorLogLocation;
		}

		public static string ExceptionLogLocation {
			get => LogManager.ExceptionLogLocation;
		}

		public string RegisteredTo {
			get;
			private set;
		}

		private readonly LogManager logManager;

		public Log( string registerName ) {
			logManager = LogManager.Singleton;
			RegisteredTo = registerName;
		}

		public void Message( string message ) {
			logManager.Write( FormatMessage( message ), LogType.Normal );
		}

		public void Error( string message ) {
			logManager.Write( FormatMessage( message ), LogType.Error );
		}

		public void Exception( string mainMessage, Exception e, bool crash = true ) {
			string exceptionMessage = FormatException( e );
			if( mainMessage is null || mainMessage == "" ) {
				logManager.Write( exceptionMessage.Trim(), LogType.Exception );
			} else {
				logManager.Write( $"{FormatMessage( mainMessage )}{exceptionMessage}", LogType.Exception );
			}

			if( crash ) {
				DialogResult result = MessageBox.Show(
					$"Something happened.", MessageBoxButtons.YesNo, MessageBoxIcon.Stop );

				if( result == DialogResult.No ) {
					try {
						Environment.Exit( -1 );
					} catch {
						MessageBox.Show( $"Something happened while trying to quit.", "Something happened.", MessageBoxButtons.OK );
					}
				}
			}
		}

		public void PrintArray( object[] objects ) {
			StringBuilder stringBuilder = new StringBuilder( "object[]{" );

			for( int i = 0; i < objects.Length; ++i ) {
				stringBuilder.Append( $"{objects[ i ].ToString()}" );
				if(i != objects.Length - 1 ) {
					stringBuilder.Append( ", " );
				}
			}

			Message( stringBuilder.Append( "}" ).ToString() );
		}

		public void ProcessExit( object sender, EventArgs e ) {
			logManager.ProgramExit();
		}

		private string FormatMessage( string message ) {
			return $"{RegisteredTo}: {message}";
		}

		private string FormatException( Exception e ) {
			return $"\n{FormatMessage( $"Exception Message: {e.Message}" )}\n{FormatMessage( $"Stacktrace: {e.StackTrace}" )}";
		}
	}
}