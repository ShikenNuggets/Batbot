using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class PingModule : ModuleBase<SocketCommandContext>{
		[Command("ping")]
		[Summary("Pong!")]
		public Task PingAsync(){
			return ReplyAsync("Pong! :)");
		}
	}
}