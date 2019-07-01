using Newtonsoft.Json.Linq;
using RestSharp;

namespace Batbot{
	class TwitchAPI{
		private static readonly RestClient client = new RestClient("https://api.twitch.tv/helix/");

		public static JObject Get(string requestText){
			bool hasRetried = false;

			while(true){
				var request = new RestRequest(requestText, DataFormat.Json);
				lock(Data.TwitchClientID) request.AddHeader("Client-ID", Data.TwitchClientID);

				IRestResponse response = null;
				lock(client){
					//Twitch API is rate limited to 30 requests per minute (one every 2 seconds)
					//Manually throttling ourselves like this is inefficient and error-prone
					System.Threading.Thread.Sleep(2000);
					response = client.Get(request);
				}

				if(response == null){
					Debug.Log("Error: Unknown REST API Error!", Debug.Verbosity.Error);
					return null;
				}else if(response.StatusCode.ToString() == "429"){
					if(hasRetried){
						Debug.Log("Error: Rate-limiting issue has been encountered!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Application is being rate-limited, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(30000);
					hasRetried = true;
					continue;
				}else if(response.StatusCode.ToString() == "503"){
					if(hasRetried){
						Debug.Log("Error: Twitch API is currently unavailable!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Twitch API error, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(5000);
					hasRetried = true;
					continue;
				}

				if(response.StatusCode != System.Net.HttpStatusCode.OK){
					Debug.Log("Unhandled HTTP Error: " + response.StatusCode.ToString(), Debug.Verbosity.Error);
					return null;
				}

				if(!response.IsSuccessful){
					Debug.Log("Error: Unspecified error in Twitch API call!", Debug.Verbosity.Error);
					return null;
				}

				return JObject.Parse(response.Content);
			}

			return null;
		}
	}
}