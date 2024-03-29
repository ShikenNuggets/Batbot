﻿using System.Collections.Generic;

namespace Batbot{
	class Commands{
		private static readonly Dictionary<string, List<string>> commands = new Dictionary<string, List<string>>{
			{ "add", new List<string>{
				"**!add [twitch]** - Adds or removes a Twitch streamer from the list"
			}},

			{ "help", new List<string>{
				"**!help** - Displays all commands and how to use them",
				"**!help [sub-command]** - Displays relevant sub-commands and how to use them"
			}},

			{ "list", new List<string>{
				"**!list** - Lists user IDs for all Twitch streamers",
				"**!list sorted** - Lists user IDs for all Twitch streamers, sorted by last stream date"
			}},

			{ "live", new List<string>{
				"**!live** - Lists all Twitch streamers who are currently live"
			}},

			{ "message", new List<string>{
				"**!message list** - Lists all custom messages",
				"**!message add [message]** - Creates a new custom message",
				"**!message remove [index]** - Removes a custom message at the specified index",
				"**!message edit [index] [new-message]** - Edits a custom message at the specified index"
			}},

			{ "ping", new List<string>{
				"**!ping** - Bot replys with \"pong\" (useful for checking if it's alive)"
			}},

			{ "reaction", new List<string>{
				"**!reaction add [message-id] [channel] [emote] [role]** - Creates a Reaction Role",
				"**!reaction remove [index]** - Removes a Reaction Role at the specified index",
				"**!reaction list** - Lists all Reaction Roles for this server"
			}},

			{ "reset", new List<string>{
				"**!reset [twitch-user-name]** - Resets the name for a particular streamer in the list"
			}},

			{ "setcooldown", new List<string>{
				"**!setcooldown [hours]** - Sets how long I should wait before being able to announce the same streamer again"
			}},

			{ "setchannel", new List<string>{
				"**!setchannel [#discord-chanel]** - Adds or removes Discord channels for streams to be announced in"
			}},

			{ "setupdatefrequency", new List<string>{
				"**!setupdatefrequency [minutes]** - Sets how often new streams will be checked for"
			}}
		};

		public static bool CommandExists(string command){
			return commands.ContainsKey(command);
		 }

		public static string GenerateCommandText(){
			string finalString = "";
			foreach(List<string> ls in commands.Values){
				foreach(string s in ls){
					finalString += s + "\n";
				}
			}

			return finalString;
		}

		public static string GenerateCommandText(string command){
			string finalString = "";

			if(CommandExists(command)){
				foreach(string s in commands[command]){
					finalString += s + "\n";
				}
			}

			return finalString;
		}
	}
}