using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class CheckReactionsModule : ModuleBase<SocketCommandContext>{
		[Command("checkreactions")]
		[Summary("Checks Reaction Roles")]
		public Task ClearCacheAsync(){
			Data.ClearCache();
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			Program.HourlyReactionCheckup();

			return ReplyAsync("Checking reactions...");
		}
	}
}