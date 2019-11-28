using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace Batbot{
	class SetChannelModule : ModuleBase<SocketCommandContext>{
		[Command("setchannel")]
		[Summary("Sets channel for streams to be announced in")]
		public Task SetChannelAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync(">>> " + Commands.GenerateCommandText("setchannel"));
		}

		[Command("setchannel")]
		[Summary("Sets channel for streams to be announced in")]
		public Task SetChannelAsync(string _){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync("Error: !setchannel must include a Discord channel! >>> " + Commands.GenerateCommandText("setchannel"));
		}

		[Command("setchannel")]
		[Summary("Sets channel for streams to be announced in")]
		public Task SetChannelAsync([Remainder] [Summary("The text to echo")] IChannel channel){
			if(channel == null){
				Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
				return ReplyAsync("Invalid Discord channel!");
			}

			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			bool contains;
			lock(Data.Channels){
				contains = Data.Channels.Contains(channel.Id);
				if(contains){
					Data.Channels.Remove(channel.Id);
				}else{
					Data.Channels.Add(channel.Id);
				}
			}

			Data.Save();
			if(contains){
				return ReplyAsync("Success - Announcements will no longer be posted in <#" + channel.Id.ToString() + ">");
			}else{
				return ReplyAsync("Success - Announcements will now be posted in <#" + channel.Id.ToString() + ">");
			}
		}
	}
}