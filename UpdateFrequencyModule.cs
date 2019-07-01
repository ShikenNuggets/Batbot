using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class UpdateFrequencyModule : ModuleBase<SocketCommandContext>{
		[Command("setupdatefrequency")]
		[Summary("Sets how often new streams should be checked for")]
		public Task SetChannelAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync("```\n" + Commands.GenerateCommandText("setupdatefrequency") + "```");
		}

		[Command("setupdatefrequency")]
		[Summary("Sets how often new streams should be checked for")]
		public Task SetChannelAsync(string _){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync("```\n" + Commands.GenerateCommandText("setupdatefrequency") + "```");
		}

		[Command("setupdatefrequency")]
		[Summary("Sets how often new streams should be checked for")]
		public Task SetChannelAsync([Remainder] [Summary("The text to echo")] float updateFrequency){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			if(updateFrequency < 0.5f){
				return ReplyAsync("Update frequency must be at least 0.5!");
			}

			Data.UpdateFrequency = updateFrequency;
			Data.Save();
			return ReplyAsync("Success - streams will now be checked every " + updateFrequency + " minute(s)");
		}
	}
}