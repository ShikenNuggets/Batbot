﻿using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class AddModule : ModuleBase<SocketCommandContext>{
		[Command("add")]
		[Summary("Adds a streamer to the list")]
		public Task AddAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎")); //Thumbs down on incorrect command usage

			return ReplyAsync("```\n" + Commands.GenerateCommandText("add") + "```");
		}

		[Command("add")]
		[Summary("Adds a streamer to the list")]
		public Task AddAsync([Remainder] [Summary("Name of the Twitch streamer you want to add or remove from the list")] string streamerName){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍")); //Thumbs up to acknowledge that the command was received

			bool containsKey;
			lock(Data.Streamers){
				containsKey = Data.Streamers.ContainsKey(streamerName);
				if(containsKey){
					Data.Streamers.Remove(streamerName);
				}
			}

			if(containsKey){
				Data.Save();
				return ReplyAsync("Success - " + streamerName + " will no longer be announced.");
			}

			TwitchUser user = Twitch.GetUserByName(streamerName);
			if(user == null){
				return ReplyAsync("I couldn't find any Twitch streamers with that name. Sorry!");
			}

			lock(Data.Streamers) Data.Streamers.Add(user.displayName, user.id);
			Data.Save();
			return ReplyAsync("Success - " + user.displayName + " will now be announced.");
		}
	}
}