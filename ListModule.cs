using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ListModule : ModuleBase<SocketCommandContext>{
		[Command("list")]
		[Summary("Lists all streamers")]
		public Task ListAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response = "Currently announcing these streamers:```";
			lock(Data.Streamers){
				if(Data.Streamers.Count == 0){
					return ReplyAsync("There are no streamers currently being announced.");
				}

				foreach(var s in Data.Streamers){
					response += s.Key + " (" + s.Value.id + ")";
					if(s.Value.lastStream.HasValue){
						response += " (" + s.Value.lastStream.Value.ToShortDateString() + ")";
					}else{
						response += " (unknown)";
					}
					response += "\n";
				}
			}
			response += "```";

			return ReplyAsync(response);
		}
	}
}