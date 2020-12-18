using System.Collections.Generic;
using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class LastStreamModule : ModuleBase<SocketCommandContext>{
		[Command("help")]
		[Summary("Sends a Help message")]
		public Task HelpAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
			return ReplyAsync(">>> " + Commands.GenerateCommandText());
		}

		[Command("help")]
		[Summary("Sends a Help message")]
		public Task HelpAsync([Remainder] [Summary("Command subgroup that you need/want help with")] string command){
			if(Commands.CommandExists(command)){
				Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
				return ReplyAsync(">>> " + Commands.GenerateCommandText(command));
			}else{
				Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
				return ReplyAsync("There is no [**" + command + "**] command!");
			}
		}
	}
}