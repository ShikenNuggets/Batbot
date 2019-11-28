using System.Linq;
using System.Collections.Generic;
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
					response += "**" + Utility.SanitizeForMarkdown(s.Key) + "**";
					if(s.Value.lastStream.HasValue){
						response += " (" + s.Value.lastStream.Value.ToShortDateString() + ")";
					}
					response += "\n";
				}
			}

			return ReplyAsync(response);
		}

		[Command("list")]
		[Summary("Lists all streamers")]
		public Task ListAsync([Remainder] string text){
			if(text != "sorted"){
				Context.Message.AddReactionAsync(new Discord.Emoji("👎")); //Thumbs down on incorrect command usage
				return ReplyAsync(">>> " + Commands.GenerateCommandText("list"));
			}

			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			List<KeyValuePair<string, StreamerInfo>> sortedList;
			lock(Data.Streamers){
				sortedList = Data.Streamers.ToList();
			}

			if(sortedList.Count == 0){
				return ReplyAsync("There are no streamers currently being announced.");
			}

			sortedList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

			string response = "Currently announcing these " + sortedList.Count + " streamers:\n>>> ";
			foreach(var s in sortedList){
				response += "**" + Utility.SanitizeForMarkdown(s.Key) + "**";
				if(s.Value.lastStream.HasValue){
					response += " (" + s.Value.lastStream.Value.ToShortDateString() + ")";
				}
				response += "\n";
			}

			return ReplyAsync(response);
		}
	}
}