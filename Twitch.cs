using System;
using System.Collections.Generic;
using System.Linq;

namespace Batbot{
	class Twitch{
		private static TwitchAPI _twitchAPI = null;
		private static TwitchAPI twitchAPI{
			get{ 
				if(_twitchAPI == null){
					_twitchAPI = new TwitchAPI();
				}

				return _twitchAPI;
			}
		}

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
			var o = twitchAPI.Get("games?name=" + gameName);
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return o["data"][0]["id"].ToString();
		}

		public static string GetGameName(string gameID){
			var o = twitchAPI.Get("games?id=" + gameID);
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return o["data"][0]["name"].ToString();
		}

		public static string GetUserName(string userID){
			var o = twitchAPI.Get("users?id=" + userID);
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return o["data"][0]["display_name"].ToString();
		}

		public static string GetUserID(string userName){
			var o = twitchAPI.Get("users?login=" + userName.ToLower());
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return o["data"][0]["id"].ToString();
		}

		public static TwitchUser GetUserByName(string name){
			var o = twitchAPI.Get("users?login=" + name.ToLower());
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return new TwitchUser(o);
		}

		public static TwitchUser GetUserByID(string id){
			var o = twitchAPI.Get("users?id=" + id);
			if(o == null || !o["data"].HasValues){
				return null;
			}

			return new TwitchUser(o);
		}

		public static List<TwitchStream> GetCurrentStreams(){
			int streamCount = 0;
			lock(Data.Streamers){
				streamCount = Data.Streamers.Count;
			}

			if(streamCount == 0){
				return new List<TwitchStream>();
			}

			List<List<StreamerInfo>> lists = new List<List<StreamerInfo>>();
			lock(Data.Streamers){
				lists = Utility.SplitList(Data.Streamers.Values.ToList(), TwitchAPI.MaxData);
			}

			List<TwitchStream> streams = new List<TwitchStream>();
			foreach(var streamers in lists){
				string requestString = "streams?first=" + TwitchAPI.MaxData + "&";
				foreach(var s in streamers){
					requestString += "user_id=" + s.id + "&";
				}

				var o = twitchAPI.Get(requestString);
				if(o == null){
					Debug.Log("Twitch API error!", Debug.Verbosity.Error);
					return null;
				}

				for(int i = 0; i < TwitchAPI.MaxData; i++){
					try{
						streams.Add(new TwitchStream(o, i));
					}catch(ArgumentOutOfRangeException){
						break; //No more streams from this API call - TODO relying on exceptions here sucks
					}catch(Exception e){
						Debug.Log("Error creating TwitchStream from data! Exception: " + e.Message, Debug.Verbosity.Error);
					}
				}
			}

			return streams;
		}
	}
}