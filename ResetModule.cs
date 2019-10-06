using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ResetModule : ModuleBase<SocketCommandContext>{
		[Command("reset")]
		[Summary("Resets the data for a streamer")]
		public Task ResetAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			return ReplyAsync("```\n" + Commands.GenerateCommandText("reset") + "```");
		}

		[Command("reset")]
		[Summary("Resets the data for a streamer")]
		public Task ResetAsync([Remainder] [Summary("The text to echo")] string userID){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock(Data.Streamers){
				foreach(StreamerInfo si in Data.Streamers.Values){
					if(si.id == userID){
						TwitchUser user = Twitch.GetUserByID(userID);
						if(user == null){
							return ReplyAsync("I couldn't find any Twitch streamers with that ID. Sorry!");
						}

						Data.Streamers.Remove(Data.Streamers.FirstOrDefault(x => x.Value.id == userID).Key);
						Data.Streamers.Add(user.displayName, new StreamerInfo(user.id)); //TAKE OLD LAST STREAM TIME
						Data.Save();

						return ReplyAsync("Success - data for " + user.displayName + " has been reset.");
					}
				}
			}

			return ReplyAsync("No users with that ID are currently in the list.");
		}
	}
}