using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batbot{
	class Debug{
		private static readonly string path = "Log-" + DateTime.Now.ToString("yyyy-MM-dd---HH-mm-ss") + ".txt";

		public enum Verbosity{
			Verbose,
			Info,
			Error
		}

		public static bool Initialize(){
			lock(path) System.IO.File.AppendAllText(path, path + "\n");
			return true;
		}

		public static void Log(string message, Verbosity verbosity = Verbosity.Info){
			lock(path){
				Console.WriteLine(message);

				if(verbosity > Verbosity.Verbose){
					System.IO.File.AppendAllText(path, message + "\n");
				}
			}
		}
	}
}
