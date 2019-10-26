using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	class LiveModule : ModuleBase<SocketCommandContext>{
		[Command("live")]
		[Summary("Lists all streamers who are currently live")]
		public Task LiveAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response = "The following streamers are currently live:\n>>> ";
			lock(Data.CurrentlyLive){
				if(Data.CurrentlyLive.Count == 0){
					return ReplyAsync("No streamers are currently live.");
				}

				foreach(var s in Data.CurrentlyLive){
					response += "**" + Utility.SanitizeForMarkdown(s.Key) + "** - " + Utility.SanitizeForMarkdown(s.Value) + "\n";
				}
			}

			return ReplyAsync(response);
		}
	}
}