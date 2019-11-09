using System.Threading.Tasks;

using Discord.Commands;

namespace Batbot{
	[Group("message")]
	class MessageModule : ModuleBase<SocketCommandContext>{
		[Command("list")]
		[Summary("Displays all custom announcement messages")]
		public Task MessageAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			string response = "Custom announcement messages:\n>>> ";
			lock(Data.AnnounceMessages){
				if(Data.AnnounceMessages.Count == 0){
					return ReplyAsync("There are no custom messages set.");
				}

				int index = 0;
				foreach(string s in Data.AnnounceMessages){
					response += index + ": `" + s + "`\n";
					index++;
				}
			}

			return ReplyAsync(response);
		}

		[Command("add")]
		[Summary("Adds a new custom announcement message")]
		public Task MessageAsync([Remainder] [Summary("The text to echo")] string newMessage){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock(Data.AnnounceMessages){
				if(Data.AnnounceMessages.Contains(newMessage)){
					return ReplyAsync("That message already exists!");
				}

				Data.AnnounceMessages.Add(newMessage);
			}

			Data.Save();
			return ReplyAsync("Success - new custom announcement message has been set.");
		}

		[Command("remove")]
		[Summary("Deletes the message at this index")]
		public Task MessageAsync([Remainder] [Summary("The text to echo")] int index){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock(Data.AnnounceMessages){
				if(index < 0 || index >= Data.AnnounceMessages.Count){
					return ReplyAsync("Invalid index! Must be between 0 and " + Data.AnnounceMessages.Count);
				}

				Data.AnnounceMessages.RemoveAt(index);
			}

			Data.Save();
			return ReplyAsync("Success - custom announcement message " + index + " has been removed.");
		}

		[Command("edit")]
		[Summary("Deletes the message at this index")]
		public Task MessageAsync([Summary("The text to echo")] int index, [Remainder] [Summary("New Message")] string newMessage){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock (Data.AnnounceMessages){
				if(index < 0 || index >= Data.AnnounceMessages.Count){
					return ReplyAsync("Invalid index! Must be between 0 and " + Data.AnnounceMessages.Count + "!");
				}

				Data.AnnounceMessages[index] = newMessage;
			}

			Data.Save();
			return ReplyAsync("Success - custom announcement message has been edited.");
		}
	}
}