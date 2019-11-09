using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Batbot{
	[Group("reaction")]
	class ReactionModule : ModuleBase<SocketCommandContext>{
		[Command("add")]
		[Summary("Allow roles to be set based on message reactions")]
		public Task AddReactionAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👎"));

			return ReplyAsync("```\n" + Commands.GenerateCommandText("reaction") + "```");
		}

		[Command("add")]
		[Summary("Allow roles to be set based on message reactions")]
		public Task AddReactionAsync(ulong messageID, SocketGuildChannel channel, Emoji emote, SocketRole role){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			if(channel.Guild.Id != Context.Guild.Id || role.Guild.Id != Context.Guild.Id){
				return ReplyAsync("Error! Trying to add ReactionRole to a different server!");
			}else if(channel.Guild.Id != role.Guild.Id){
				return ReplyAsync("Error! Channel and role are not from the same server!");
			}

			lock(Data.ReactionRoles) Data.ReactionRoles.Add(new ReactionRole((channel as SocketGuildChannel).Guild.Id, messageID, emote.Name, role.Id));
			Data.Save();

			//TODO - Add emote reaction to specified message

			return ReplyAsync("Success - " + role.ToString() + " will be added based on " + emote.ToString() + " reactions in the specified channel");
		}

		[Command("remove")]
		[Summary("Remove a set ReactionRole")]
		public Task RemoveReactionAsync([Remainder] int index){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));

			lock(Data.ReactionRoles){
				if(index < 0 || index >= Data.ReactionRoles.Count){
					return ReplyAsync("Error! Value must be between 0 and " + Data.ReactionRoles.Count + "!");
				}

				if(Context.Guild.Id != Data.ReactionRoles[index].guildID){
					return ReplyAsync("You do not have permission to remove this Reaction Role");
				}

				//TODO - Remove emote reaction from specified message

				Data.ReactionRoles.RemoveAt(index);
			}

			Data.Save();
			return ReplyAsync("Success - removed the specified ReactionRole");
		}

		[Command("list")]
		[Summary("List all ReactionRoles")]
		public Task ListReactionsAsync(){
			Context.Message.AddReactionAsync(new Discord.Emoji("👍"));
			string responseText = "Reaction Roles:\n>>> ";

			lock(Data.ReactionRoles){
				int index = 0;
				int relevantRRs = 0;
				foreach(var rr in Data.ReactionRoles){
					if(Context.Guild.Id != rr.guildID){
						index++;
						continue;
					}

					responseText += index.ToString() + ": MessageID(" + rr.messageID + ") - Emote(" + rr.emote + ") - Role(" + Context.Guild.GetRole(rr.role).ToString() + ")\n";
					index++;
					relevantRRs++;
				}

				if(Data.ReactionRoles.Count == 0 || relevantRRs == 0){
					return ReplyAsync("There are no Reaction Roles set.");
				}
			}

			return ReplyAsync(responseText);
		}
	}
}