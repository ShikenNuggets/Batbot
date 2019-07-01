using System.Collections.Generic;
using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class HelpModule : ModuleBase<SocketCommandContext>{
		[Command("help")]
		[Summary("Sends a Help message")]
		public Task HelpAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
			return ReplyAsync("```\n" + Commands.GenerateCommandText() + "```");
		}
	}
}