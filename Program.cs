using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Batbot{
	public class Program{
		private static DiscordSocketClient _client;
		private CommandService _commands;

		private static readonly List<string> streamersOnCooldown = new List<string>();

		bool IsOnCooldown(string twitchID){
			bool value = false;
			lock(streamersOnCooldown) value = streamersOnCooldown.Contains(twitchID);
			return value;
		}

		void ApplyCooldown(string twitchID, string name){
			Debug.Log("Placing " + name + " [" + twitchID + "] on cooldown...", Debug.Verbosity.Verbose);
			lock(streamersOnCooldown) streamersOnCooldown.Add(twitchID);
			System.Threading.Thread.Sleep((int)(1000.0f * 60.0f * 60.0f * Data.Cooldown)); //Wait [cooldown] hours
			lock(streamersOnCooldown) streamersOnCooldown.Remove(twitchID);
			Debug.Log(name + " [" + twitchID + "] removed from cooldown", Debug.Verbosity.Verbose);
		}

		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
		private delegate bool HandlerRoutine(CtrlTypes CtrlType);

		private enum CtrlTypes{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}

		public static void Main(){
			HandlerRoutine hr = new HandlerRoutine(ConsoleCtrlCheck);
			GC.KeepAlive(hr);
			SetConsoleCtrlHandler(hr, true);

			Program program = null;

			//This is a really garbage way to prevent full crashes - TODO
			bool successfulExit = false;
			while(!successfulExit){
				try{
					successfulExit = true;
					program = new Program();
					program.MainAsync().GetAwaiter().GetResult();
				}catch(Exception e){
					successfulExit = false;
					Debug.Log("Unhandled Exception! Error: " + e.Message);

				#if DEBUG
					Console.ReadKey();
				#endif

					Debug.Log("Restarting process, standby...", Debug.Verbosity.Error);
					System.Threading.Thread.Sleep(1000 * 60); //Wait one minute before trying again
				}finally{
					program = null;
				}
			}

			Environment.Exit(0);
		}

		private static bool ConsoleCtrlCheck(CtrlTypes ctrlType){
			switch(ctrlType){
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
					if(_client != null){
						_client.LogoutAsync();
						_client = null;
					}

					System.GC.Collect();
                    break;
            }

			return true;
		}

		public async Task MainAsync(){
			Debug.Initialize();
			Data.Initialize();

			_client = new DiscordSocketClient();
			_commands = new CommandService();

			_client.Log += Log;
			_client.Ready += OnReady;
			_client.MessageReceived += HandleCommandAsync;
			_client.ReactionAdded += OnReactionChange;
			_client.ReactionRemoved += OnReactionChange;
			_client.ReactionsCleared += OnReactionChange;

			await _client.LoginAsync(TokenType.Bot, Data.DiscordClientID);
			await _client.StartAsync();

			_commands.AddTypeReader<Emoji>(new EmojiTypeReader());

			await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
			await _commands.AddModuleAsync<AddModule>(null);
			await _commands.AddModuleAsync<CooldownModule>(null);
			await _commands.AddModuleAsync<HelpModule>(null);
			await _commands.AddModuleAsync<ListModule>(null);
			await _commands.AddModuleAsync<LiveModule>(null);
			await _commands.AddModuleAsync<MessageModule>(null);
			await _commands.AddModuleAsync<ReactionModule>(null);
			await _commands.AddModuleAsync<ResetModule>(null);
			await _commands.AddModuleAsync<SetChannelModule>(null);
			await _commands.AddModuleAsync<UpdateFrequencyModule>(null);

			var aTimer = new Timer(1000 * 60 * 60); //1 hour in milliseconds
			aTimer.Elapsed += new ElapsedEventHandler(HourlyReactionCheckup);
			aTimer.Start();

			await Task.Delay(-1);
		}

		private async Task Log(LogMessage message){
			Debug.Log(message.ToString());
			await Task.CompletedTask;
		}

		private async Task OnReady(){
			_ = Task.Run(() => CheckStreams());
			await Task.CompletedTask;
		}

		private async Task HandleCommandAsync(SocketMessage messageParam){
			//Don't process the command if it was a system message
			if(!(messageParam is SocketUserMessage message)){
				return;
			}

			//Create a number to track where the prefix ends and the command begins
			int argPos = 0;

			//Determine if the message is a command based on the prefix and make sure no bots or webhooks trigger commands
			if(!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot || message.Author.IsWebhook){
				return;
			}

			if(!(message.Author is SocketGuildUser a) || !a.GuildPermissions.Administrator){
				Debug.Log("Non-admin attempted to trigger a command. Ignoring...", Debug.Verbosity.Verbose);
				return;
			}

			Debug.Log("Command Received: " + message.Content);

			var context = new SocketCommandContext(_client, message);
			await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
		}

		private async Task OnReactionChange(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction){
			var messageValue = await message.GetOrDownloadAsync();
			if(messageValue == null){
				Debug.Log("Could not get message!", Debug.Verbosity.Error);
				await Task.CompletedTask;
			}

			await CheckReactions(messageValue);
		}

		private async Task OnReactionChange(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel){
			var messageValue = await message.GetOrDownloadAsync();
			if(messageValue == null){
				Debug.Log("Could not get message!", Debug.Verbosity.Error);
				await Task.CompletedTask;
			}

			await CheckReactions(messageValue);
		}

		private async void CheckStreams(){
			List<string> loggedStreams = new List<string>();
			Dictionary<string, string> currentLiveStreams;

			int iterations = 0;
			while(true){
				currentLiveStreams = new Dictionary<string, string>();

				if(iterations > 0){
					System.Threading.Thread.Sleep((int)(1000.0f * 60.0f * Data.UpdateFrequency)); //Every [updateFrequency] minutes
				}

				Debug.Log("Checking for new streams to announce...", Debug.Verbosity.Verbose);
				List<TwitchStream> streams = Twitch.GetCurrentStreams();
				if(streams == null){
					Debug.Log("An error has occured while attempting to get streams, restarting process...", Debug.Verbosity.Error);
					System.Threading.Thread.Sleep(1000 * 30); //Wait 30 seconds before starting over
					continue;
				}

				int streamsAnnounced = 0;
				foreach(TwitchStream ts in streams){
					string gameName;
					if(Data.IsCached(ts.id)){
						gameName = Data.GetCachedGame(ts.id);
					}else{
						gameName = Data.CacheGame(ts.id, Twitch.GetGameName(ts.gameID));
					}

					if(ts.title.Contains("[nosrl]")){
						currentLiveStreams.Add(ts.user, gameName + " [nosrl]");
					}else{
						currentLiveStreams.Add(ts.user, gameName);
					}

					lock(Data.AnnouncedStreams){
						if(Data.AnnouncedStreams.Contains(ts.id)){
							if(Twitch.gameIDs.ContainsValue(ts.gameID) && !ts.title.Contains("[nosrl]")){
								streamsAnnounced++; //Only count towards the total if it's still a Batman speedrunning stream
							}
							
							continue; //We've already announced this stream
						}
					}

					if(Twitch.gameIDs.ContainsValue(ts.gameID) && !ts.title.Contains("[nosrl]")){
						streamsAnnounced++;
						await AnnounceStream(ts);
					}else{
						if(!loggedStreams.Contains(ts.id)){
							if(ts.title.Contains("[nosrl]")){
								Debug.Log(ts.user + " is streaming non-speedrunning content, ignoring...");
							}

							Debug.Log(ts.user + " is streaming a non-Batman game (" + gameName + "), ignoring...");
							loggedStreams.Add(ts.id);
						}
					}
				}

				if(streamsAnnounced == 0){
					await _client.SetGameAsync("for new Batman streams", null, ActivityType.Watching);
				}else if(streamsAnnounced == 1){
					await _client.SetGameAsync("a Batman stream", null, ActivityType.Watching);
				}else if(streamsAnnounced > 1){
					await _client.SetGameAsync(streamsAnnounced + " Batman streams", null, ActivityType.Watching);
				}

				iterations++;
				Debug.Log(streamsAnnounced + " previously-announced stream(s) are still live with Batman content", Debug.Verbosity.Verbose);
				Debug.Log("Check " + iterations + " complete. Program is now idle.", Debug.Verbosity.Verbose);

				lock(Data.CurrentlyLive){ Data.CurrentlyLive = currentLiveStreams; }
				Data.Save();
			}
		}

		private async void HourlyReactionCheckup(object _, ElapsedEventArgs __){
			List<ReactionRole> rrs;
			lock(Data.ReactionRoles){
				rrs = new List<ReactionRole>(Data.ReactionRoles); //Create a copy of the ReactionRole list and use that copy
			}

			foreach(ReactionRole rr in rrs){
				var guild = _client.GetGuild(rr.guildID);
				foreach(var c in guild.TextChannels){
					if(await c.GetMessageAsync(rr.messageID) is IUserMessage message){
						await CheckReactions(message);
					}
				}
			}
		}

		private async Task CheckReactions(IUserMessage message){
			List<ReactionRole> rrs;
			lock(Data.ReactionRoles){
				rrs = new List<ReactionRole>(Data.ReactionRoles); //Create a copy of the ReactionRole list and use that copy
			}

			foreach(ReactionRole rr in rrs){
				if(message.Id != rr.messageID){
					continue;
				}

				Debug.Log("Checking reactions to message(" + rr.messageID + ")...", Debug.Verbosity.Verbose);

				var guild = _client.GetGuild(rr.guildID);
				if(guild == null){
					Debug.Log("Could not get guild by ID!", Debug.Verbosity.Error);
					continue;
				}

				Emoji emoji = new Emoji(rr.emote);
				if(emoji == null){
					Debug.Log("Could not create emoji [" + rr.emote + "]!", Debug.Verbosity.Error);
					continue;
				}

				var usersWithReactions = await message.GetReactionUsersAsync(emoji, 100).FlattenAsync();
				List<ulong> userIDs = new List<ulong>();
				foreach(var u in usersWithReactions){
					userIDs.Add(u.Id);
				}

				foreach(var u in guild.Users){
					if(u.IsBot || u.IsWebhook){
						continue; //Don't mess with bot roles
					}

					SocketRole role = guild.GetRole(rr.role);
					if(role == null){
						Debug.Log("Could not find role with ID " + rr.role + "!", Debug.Verbosity.Error);
						break;
					}

					if(userIDs.Contains(u.Id) && !u.Roles.Contains(role)){
						string debugText = "Adding " + role.Name + " role to user " + u.Username;
						if(u.Nickname != null && u.Nickname.Length > 0){
							debugText += " (" + u.Nickname + ")";
						}

						Debug.Log(debugText);
						await u.AddRoleAsync(role);
					}else if(!userIDs.Contains(u.Id) && u.Roles.Contains(guild.GetRole(rr.role))){
						string debugText = "Removing " + role.Name + " role from user " + u.Username;
						if(u.Nickname != null && u.Nickname.Length > 0){
							debugText += " (" + u.Nickname + ")";
						}

						Debug.Log(debugText);
						await u.RemoveRoleAsync(role);
					}
				}
			}
		}

		private async Task AnnounceStream(TwitchStream ts){
			lock(Data.AnnouncedStreams) Data.AnnouncedStreams.Add(ts.id);

			lock(Data.Streamers){
				//If the name we have on file doesn't match the streamer's name, reset the entry
				if(!Data.Streamers.ContainsKey(ts.user)){
					var item = Data.Streamers.First(kvp => kvp.Value.id == ts.userID);
					Data.Streamers.Remove(item.Key);
					Data.Streamers.Add(ts.user, new StreamerInfo(ts.userID, DateTime.Now));
				}

				Data.Streamers[ts.user] = new StreamerInfo(ts.userID, DateTime.Now);
			}

			Data.Save();

			if(IsOnCooldown(ts.userID)){
				Debug.Log(ts.user + " is live, not announcing since they are still on cooldown");
				return;
			}

			var gameName = Twitch.gameIDs.FirstOrDefault(x => x.Value == ts.gameID).Key;
			Debug.Log(ts.user + " is live with " + gameName);
			Task.Run(() => ApplyCooldown(ts.userID, ts.user));
			foreach(ulong c in Data.Channels){
				var announceChannel = _client.GetChannel(c) as SocketTextChannel;
				await announceChannel.SendMessageAsync(FormatAnnouncementMessage(ts.user, gameName, ts.title));
			}
		}

		private string FormatAnnouncementMessage(string userName, string gameName, string title){
			string message;
			lock(Data.AnnounceMessages){
				if(Data.AnnounceMessages.Count > 0){
					message = Data.AnnounceMessages[new Random().Next(0, Data.AnnounceMessages.Count)];
				}else{
					message = "{user} is live with {game} at {link}";
				}
			}

			message = message.Replace("{user}", Utility.SanitizeForMarkdown(userName));
			message = message.Replace("{game}", Utility.SanitizeForMarkdown(gameName));
			message = message.Replace("{title}", Utility.SanitizeForMarkdown(title));
			message = message.Replace("{link}", "https://www.twitch.tv/" + userName.ToLower());

			return message;
		}
	}
}