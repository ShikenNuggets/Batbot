﻿using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class AddModule : ModuleBase<SocketCommandContext>{
		[Command("add")]
		[Summary("Adds a streamer to the list")]
		public Task AddAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎")); //Thumbs down on incorrect command usage
			return ReplyAsync(">>> " + Commands.GenerateCommandText("add"));
		}

		[Command("add")]
		[Summary("Adds a streamer to the list")]
		public Task AddAsync([Remainder] [Summary("Name of the Twitch streamer you want to add or remove from the list")] string streamerName){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍")); //Thumbs up to acknowledge that the command was received

			bool containsKey = false;
			int numStreamers = 0;
			lock(Data.Streamers){
				numStreamers = Data.Streamers.Count;
				foreach(var s in Data.Streamers){
					if(streamerName.Equals(s.Key, System.StringComparison.OrdinalIgnoreCase)){
						containsKey = true;
						streamerName = s.Key;
						break;
					}
				}

				if(containsKey){
					Data.Streamers.Remove(streamerName);
				}
			}

			if(containsKey){
				Data.Save();
				return ReplyAsync("Success - **" + Utility.SanitizeForMarkdown(streamerName) + "** will no longer be announced. There are now " + (numStreamers - 1) + " streamers in the list.");
			}

			TwitchUser user = Twitch.GetUserByName(streamerName);
			if(user == null){
				return ReplyAsync("I couldn't find any Twitch streamers with that name. Sorry!");
			}

			lock(Data.Streamers){
				foreach(var s in Data.Streamers){
					if(s.Value.id == user.id){
						return ReplyAsync("User is currently in list with an older username (**" + Utility.SanitizeForMarkdown(s.Key) + ")");
					}
				}

				Data.Streamers.Add(user.displayName, new StreamerInfo(user.id));
			}

			Data.Save();
			return ReplyAsync("Success - **" + Utility.SanitizeForMarkdown(user.displayName) + "** will now be announced. There are now " + (numStreamers + 1) + " streamers in the list.");
		}
	}
}