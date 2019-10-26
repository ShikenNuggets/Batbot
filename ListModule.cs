using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class ListModule : ModuleBase<SocketCommandContext>{
		[Command("list")]
		[Summary("Lists all streamers")]
		public Task ListAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response;
			lock(Data.Streamers){
				if(Data.Streamers.Count == 0){
					return ReplyAsync("There are no streamers currently being announced.");
				}

				response = "Currently announcing these " + Data.Streamers.Count + " streamers:\n>>> ";
				foreach(var s in Data.Streamers){
					response += "**" + SanitizeNameText(s.Key) + "** (" + s.Value.id + ")";
					if(s.Value.lastStream.HasValue){
						response += " (" + s.Value.lastStream.Value.ToShortDateString() + ")";
					}else{
						response += " (unknown)";
					}
					response += "\n";
				}
			}

			return ReplyAsync(response);
		}

		private string SanitizeNameText(string name){
			string safeName = name;
			safeName = safeName.Replace("*", "\\*");
			safeName = safeName.Replace("_", "\\_");
			safeName = safeName.Replace("~", "\\~");

			return safeName;
		}
	}
}