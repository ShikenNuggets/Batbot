using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Batbot{
	class TwitchAPI{
		private readonly HttpClient client;
		private readonly Dictionary<string, string> accessTokenParams;
		public static readonly int MaxData = 100;

		public TwitchAPI(){
			client = new HttpClient();
			lock(Data.TwitchClientID) client.DefaultRequestHeaders.Add("Client-ID", Data.TwitchClientID);

			accessTokenParams = new Dictionary<string, string>{
				{ "client_id", Data.TwitchClientID },
				{ "client_secret", Data.TwitchClientSecret },
				{ "grant_type", "client_credentials" }
			};

			RefreshAccessToken();
		}

		public JObject Get(string requestText){
			bool hasRetried = false;

			while(true){
				Task<HttpResponseMessage> response = null;
				lock(client){
					//Twitch API is rate limited to 30 requests per minute (one every 2 seconds)
					//Manually throttling ourselves like this is inefficient and error-prone
					System.Threading.Thread.Sleep(2000);
					response = client.GetAsync("https://api.twitch.tv/helix/" + requestText);
					response.Wait();
				}

				if(response == null || response.IsFaulted || response.IsCanceled){
					Debug.Log("Error: Unknown REST API Error!", Debug.Verbosity.Error);
					return null;
				}else if(response.Result.StatusCode.ToString() == "429"){
					if(hasRetried){
						Debug.Log("Error: Rate-limiting issue has been encountered!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Application is being rate-limited, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(30000);
					hasRetried = true;
					continue;
				}else if(response.Result.StatusCode.ToString() == "503"){
					if(hasRetried){
						Debug.Log("Error: Twitch API is currently unavailable!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Twitch API error, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(5000);
					hasRetried = true;
					continue;
				}

				if(response.Result.StatusCode != System.Net.HttpStatusCode.OK){
					Debug.Log("Unhandled HTTP Error: " + response.Result.StatusCode.ToString(), Debug.Verbosity.Error);
					return null;
				}

				if(!response.Result.IsSuccessStatusCode){
					Debug.Log("Error: Unspecified error in Twitch API call!", Debug.Verbosity.Error);
					return null;
				}

				var data = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);
				if(data == null || !data.HasValues || !data.ContainsKey("data") || data["data"] == null){
					return null;
				}

				return data;
			}

			return null;
		}

		public JObject Post(string command, FormUrlEncodedContent content){
			bool hasRetried = false;

			while(true){
				Task<HttpResponseMessage> response = null;
				lock(client){
					//Twitch API is rate limited to 30 requests per minute (one every 2 seconds)
					//Manually throttling ourselves like this is inefficient and error-prone
					System.Threading.Thread.Sleep(2000);
					response = client.PostAsync("https://id.twitch.tv/oauth2/" + command, content);
					response.Wait();
				}

				if(response == null || response.IsFaulted || response.IsCanceled){
					Debug.Log("Error: Unknown REST API Error!", Debug.Verbosity.Error);
					return null;
				}else if(response.Result.StatusCode.ToString() == "429"){
					if(hasRetried){
						Debug.Log("Error: Rate-limiting issue has been encountered!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Application is being rate-limited, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(30000);
					hasRetried = true;
					continue;
				}else if(response.Result.StatusCode.ToString() == "503"){
					if(hasRetried){
						Debug.Log("Error: Twitch API is currently unavailable!", Debug.Verbosity.Error);
						break;
					}

					Debug.Log("Twitch API error, standby...", Debug.Verbosity.Verbose);
					System.Threading.Thread.Sleep(5000);
					hasRetried = true;
					continue;
				}

				if(response.Result.StatusCode != System.Net.HttpStatusCode.OK){
					Debug.Log("Unhandled HTTP Error: " + response.Result.StatusCode.ToString(), Debug.Verbosity.Error);
					return null;
				}

				if(!response.Result.IsSuccessStatusCode){
					Debug.Log("Error: Unspecified error in Twitch API call!", Debug.Verbosity.Error);
					return null;
				}

				var data = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);
				if(data == null || !data.HasValues){
					return null;
				}

				return data;
			}

			return null;
		}

		private void RefreshAccessToken(){
			lock(Data.AccessToken){
				if(Data.AccessToken.IsValid() && Data.AccessToken.SecondsRemaining() > 60.0 * 5.0){
					//No need to refresh token (yet)
					return;
				}

				var data = Post("token", new FormUrlEncodedContent(accessTokenParams));
				if(data == null){
					Debug.Log("Critical error! Could not get OAuth access token!", Debug.Verbosity.Error);
				}else{
					Data.AccessToken = new OAuthToken(data);
					if(client.DefaultRequestHeaders.Contains("Authorization")){
						client.DefaultRequestHeaders.Remove("Authorization");
					}

					lock(Data.TwitchClientID) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Data.AccessToken.ToString());
				}
			}
		}
	}
}