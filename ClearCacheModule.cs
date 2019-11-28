using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ClearCacheModule : ModuleBase<SocketCommandContext>{
		[Command("clearcache")]
		[Summary("Clears Cached Data")]
		public Task ClearCacheAsync(){
			Data.ClearCache();
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
			return ReplyAsync("Cached data cleared!");
		}
	}
}