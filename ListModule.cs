using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ListModule : ModuleBase<SocketCommandContext>{
		[Command("list")]
		[Summary("Lists all streamers")]
		public Task ListAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response = "```Currently announcing these streamers:\n";
			lock(Data.Streamers){
				if(Data.Streamers.Count == 0){
					return ReplyAsync("There are no streamers currently being announced.");
				}

				foreach(var s in Data.Streamers){
					response += s.Key + " (" + s.Value + ")\n";
				}
			}
			response += "```";

			return ReplyAsync(response);
		}
	}
}
