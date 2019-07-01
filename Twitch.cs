using System;
using System.Collections.Generic;

namespace Batbot{
	class Twitch{
		public static readonly Dictionary<string, string> gameIDs = new Dictionary<string, string>{
			{ "Batman: Arkham Asylum", "21101" },
			{ "Batman: Arkham City", "26559" },
			{ "Batman: Arkham Origins", "368936" },
			{ "Batman: Arkham Origins Blackgate", "368937" },
			{ "Batman: Arkham Knight", "459676" },
			{ "Batman: Arkham VR", "493771" },
			{ "Batman: Return to Arkham", "492819" }
		};

		public static string GetGameID(string gameName){
			var o = TwitchAPI.Get("games?name=" + gameName);
			if(o == null){
				return null;
			}

			return o["data"][0]["id"].ToString();
		}

		public static string GetGameName(string gameID){
			var o = TwitchAPI.Get("games?id=" + gameID);
			if(o == null){
				return null;
			}

			return o["data"][0]["name"].ToString();
		}

		public static string GetUserName(string userID){
			var o = TwitchAPI.Get("users?id=" + userID);
			if(o == null){
				return null;
			}

			return o["data"][0]["display_name"].ToString();
		}

		public static string GetUserID(string userName){
			var o = TwitchAPI.Get("users?login=" + userName.ToLower());
			if(o == null){
				return null;
			}

			return o["data"][0]["id"].ToString();
		}

		public static TwitchUser GetUserByName(string name){
			var o = TwitchAPI.Get("users?login=" + name.ToLower());
			if(o == null){
				return null;
			}

			return new TwitchUser(o);
		}

		public static TwitchUser GetUserByID(string id){
			var o = TwitchAPI.Get("users?id=" + id);
			if(o == null){
				return null;
			}

			return new TwitchUser(o);
		}

		public static List<TwitchStream> GetCurrentStreams(){
			string requestString = "streams?first=100&";
			lock(Data.Streamers){
				foreach(string s in Data.Streamers.Values){
					requestString += "user_id=" + s + "&";
				}
			}

			var o = TwitchAPI.Get(requestString);
			if(o == null){
				return null;
			}

			List<TwitchStream> streams = new List<TwitchStream>();
			for(int i = 0; i < 100; i++){
				try{
					TwitchStream ts = new TwitchStream(o, i);
					streams.Add(ts);
				}catch(ArgumentOutOfRangeException){
					break;
				}catch(Exception e){
					Debug.Log("Error creating TwitchStream from data! Exception: " + e.Message, Debug.Verbosity.Error);
				}
			}

			return streams;
		}
	}
}