using Newtonsoft.Json.Linq;

namespace Batbot{
	class TwitchStream{
		public string id;
		public string user;
		public string title;
		public string userID;
		public string gameID;
		public bool isLive;

		public TwitchStream(JObject jObject, int index){
			id = jObject["data"][index]["id"].ToString();
			user = jObject["data"][index]["user_name"].ToString();
			title = jObject["data"][index]["title"].ToString();
			userID = jObject["data"][index]["user_id"].ToString();
			gameID = jObject["data"][index]["game_id"].ToString();

			if(jObject["data"][index]["game_id"].ToString() == "live"){
				isLive = true;
			}else{
				isLive = false;
			}
		}
	}
}