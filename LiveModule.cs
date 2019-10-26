using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class LiveModule : ModuleBase<SocketCommandContext>{
		[Command("live")]
		[Summary("Lists all streamers who are currently live")]
		public Task LiveAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response = "";
			lock(Data.CurrentlyLive){
				if(Data.CurrentlyLive.Count == 0){
					return ReplyAsync("No streamers are currently live.");
				}

				response += "The following " + Data.CurrentlyLive.Count + " streamers are currently live:\n>>> ";

				foreach(var s in Data.CurrentlyLive){
					string gameName = s.Value;
					if(gameName == null){
						gameName = "Unknown Game";
					}

					response += "**" + Utility.SanitizeForMarkdown(s.Key) + "** - " + Utility.SanitizeForMarkdown(gameName) + "\n";
				}
			}

			return ReplyAsync(response);
		}
	}
}