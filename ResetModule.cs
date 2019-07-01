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

			TwitchUser user = null;
			lock(Data.Streamers){
				if(!Data.Streamers.ContainsValue(userID)){
					return ReplyAsync("No users with that ID are currently in the list.");
				}

				user = Twitch.GetUserByID(userID);
				if(user == null){
					return ReplyAsync("I couldn't find any Twitch streamers with that ID. Sorry!");
				}

				Data.Streamers.Remove(Data.Streamers.FirstOrDefault(x => x.Value == userID).Key);
				Data.Streamers.Add(user.displayName, user.id);
			}

			Data.Save();
			return ReplyAsync("Success - data for " + user.displayName + " has been reset.");
		}
	}
}
