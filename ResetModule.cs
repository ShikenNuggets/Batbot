using System;
using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ResetModule : ModuleBase<SocketCommandContext>{
		[Command("reset")]
		[Summary("Resets the data for a streamer")]
		public Task ResetAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			return ReplyAsync(">>> " + Commands.GenerateCommandText("reset"));
		}

		[Command("reset")]
		[Summary("Resets the data for a streamer")]
		public Task ResetAsync([Remainder] [Summary("The text to echo")] string userName){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock(Data.Streamers){
				foreach(var si in Data.Streamers){
					if(si.Key == userName){
						TwitchUser user = Twitch.GetUserByID(si.Value.id);
						if(user == null){
							return ReplyAsync("I couldn't find that Twitch streamer. Sorry!");
						}

						DateTime? lastStreamTime = si.Value.lastStream;

						Data.Streamers.Remove(userName);
						Data.Streamers.Add(user.displayName, new StreamerInfo(user.id, lastStreamTime));
						Data.Save();

						return ReplyAsync("Success - data for **" + Utility.SanitizeForMarkdown(user.displayName) + "** has been reset.");
					}
				}
			}

			return ReplyAsync("No users with that ID are currently in the list.");
		}
	}
}