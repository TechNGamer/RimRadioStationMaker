using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using Utilities.Persistence;

namespace Utilities.Logging {
	public enum LogType {
		Normal, Error, Exception
	}

	internal class LogManager {
		public static LogManager Singleton {
			get {
				if( singleton is null ) {
					singleton = new LogManager();
				}

				return singleton;
			}
		}

		public static string LogLocation {
			get => PersistenceManager.LogLocation;
		}

		public static string NormalLogLocation {
			get => PersistenceManager.NormalLog;
		}

		public static string ErrorLogLocation {
			get => PersistenceManager.ErrorLog;
		}

		public static string ExceptionLogLocation {
			get => PersistenceManager.ExceptionLog;
		}

		private static LogManager singleton;

		public Encoding LogEncoding {
			get => Encoding.UTF8;
		}

		private readonly Thread logWriter;
		// Holds the messages while waiting to be writen.
		private readonly Queue<string> normalMessages, errorMessages, exceptionMessages;
		// ManualResetEvent used for multi-threading.
		private readonly ManualResetEvent waiting = new ManualResetEvent( false );
		private readonly ManualResetEvent normalMessageWaiting = new ManualResetEvent( false );
		private readonly ManualResetEvent errorMessageWaiting = new ManualResetEvent( false );
		private readonly ManualResetEvent exceptionMessageWaiting = new ManualResetEvent( false );
		private readonly ManualResetEvent terminate = new ManualResetEvent( false );
		// Writers to the logs.
		private readonly FileStream normal, error, exception;

		private LogManager() {
			// Creates the queues.
			normalMessages = new Queue<string>();
			errorMessages = new Queue<string>();
			exceptionMessages = new Queue<string>();
			// Ensures folder structure exists.
			if( !Directory.Exists( LogLocation ) ) {
				Directory.CreateDirectory( LogLocation );
			}
			// Assigns the file streams.
			try {
				normal = File.Create( NormalLogLocation );
				error = File.Create( ErrorLogLocation );
				exception = File.Create( ExceptionLogLocation );
			} catch {
				return;
			}
			// Assigns the thread.
			logWriter = new Thread( () => WriteToLog() ) {
				Priority = ThreadPriority.Lowest
			};
			// Starts the thread.
			logWriter.Start();
		}

		public void ProgramExit() {
			terminate.Set();
		}

		public void Write( string message, LogType type ) {
			switch( type ) {
				case LogType.Normal:
					lock( normalMessages ) {
						normalMessages.Enqueue( message );
					}
					normalMessageWaiting.Set();
					break;
				case LogType.Error:
					lock( errorMessages ) {
						errorMessages.Enqueue( message );
					}
					errorMessageWaiting.Set();
					break;
				case LogType.Exception:
					lock( exceptionMessages ) {
						exceptionMessages.Enqueue( message );
					}
					exceptionMessageWaiting.Set();
					break;
			}
		}

		#region Threaded Methods

		private void WriteToLog() {
			while( true ) {
				int i = WaitHandle.WaitAny( new WaitHandle[] { normalMessageWaiting, errorMessageWaiting, exceptionMessageWaiting, terminate } );

				switch( i ) {
					case 0:
						WriteOutToNormal();
						normalMessageWaiting.Reset();
						break;
					case 1:
						WriteOutToError();
						errorMessageWaiting.Reset();
						break;
					case 2:
						WriteOutToCritical();
						exceptionMessageWaiting.Reset();
						break;
					case 3:
						Close();
						return;
				}
			}
		}

		private void Close() {
			normal.Close();
			error.Close();
			exception.Close();
		}

		private void WriteOutToCritical() {
			byte[] data;
			while( exceptionMessages.Count > 0 ) {
				lock( exceptionMessages ) {
					data = LogEncoding.GetBytes( $"{DateTime.Now.ToString( "0:MM/dd/yy HH:mm:ss" )}: {exceptionMessages.Dequeue()}\n" );
				}

				exception.Write( data, 0, data.Length );
			}

			exception.Flush();
		}

		private void WriteOutToError() {
			byte[] data;
			while( errorMessages.Count > 0 ) {
				lock( errorMessages ) {
					data = LogEncoding.GetBytes( $"{DateTime.Now.ToString( "0:MM/dd/yy HH:mm:ss" )}: {errorMessages.Dequeue()}\n" );
				}

				error.Write( data, 0, data.Length );
			}

			error.Flush();
		}

		private void WriteOutToNormal() {
			byte[] data;
			while( normalMessages.Count > 0 ) {
				lock( normalMessages ) {
					data = LogEncoding.GetBytes( $"{DateTime.Now.ToString( "0:MM/dd/yy HH:mm:ss" )}: {normalMessages.Dequeue()}\n" );
				}

				normal.Write( data, 0, data.Length );
			}

			normal.Flush();
		}

		#endregion

	}
}