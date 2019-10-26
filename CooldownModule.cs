using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class CooldownModule : ModuleBase<SocketCommandContext>{
		[Command("setcooldown")]
		[Summary("Sets how long to wait before being able to announce the same streamer again")]
		public Task SetChannelAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync("```\n" + Commands.GenerateCommandText("setcooldown") + "```");
		}

		[Command("setcooldown")]
		[Summary("Sets how long to wait before being able to announce the same streamer again")]
		public Task SetChannelAsync(string _){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
			return ReplyAsync("```\n" + Commands.GenerateCommandText("setcooldown") + "```");
		}

		[Command("setcooldown")]
		[Summary("Sets how long I should wait before being able to announce the same streamer again")]
		public Task SetChannelAsync([Remainder] [Summary("New cooldown length")] float cooldown){
			if(cooldown < 0.0f){
				Context.Message.AddReactionAsync(new Discord.Emoji("👎"));
				return ReplyAsync("Cooldown must be a positive value!");
			}

			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
			Data.Cooldown = cooldown; //Don't need to lock this since it's an atomic type
			Data.Save();
			return ReplyAsync("Success - I will now wait " + cooldown + " hour(s) before announcing the same streamer again");
		}
	}
}