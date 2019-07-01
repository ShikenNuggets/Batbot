using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Batbot{
	class Data{
		private static readonly string discordClientIDFile = "Data/Discord.txt";
		private static readonly string twitchClientIDFile = "Data/Twitch.txt";
		private static readonly string streamersFile = "Data/Streamers.json";
		private static readonly string channelsFile = "Data/Channels.txt";
		private static readonly string announcedStreamsFile = "Data/AnnouncedStreams.txt";
		private static readonly string updateFrequencyFile = "Data/UpdateFrequency.txt";
		private static readonly string announceMessagesFile = "Data/AnnounceMessages.txt";
		private static readonly string reactionRoleFile = "Data/ReactionRoles.json";

		private static string _discordClientID = "";
		private static string _twitchClientID = "";
		private static Dictionary<string, string> _streamers = new Dictionary<string, string>();
		private static List<ulong> _channels = new List<ulong>();
		private static List<string> _announcedStreams = new List<string>();
		private static volatile float _updateFrequency = 0.0f;
		private static List<string> _announceMessages = new List<string>();
		private static List<ReactionRole> _reactionRoles = new List<ReactionRole>();

		public static string DiscordClientID{
			get{ return _discordClientID; }
			set{ _discordClientID = value; Save(); }
		}

		public static string TwitchClientID{
			get{ return _twitchClientID; }
			set{ _twitchClientID = value; Save(); }
		}

		public static Dictionary<string, string> Streamers{
			get{ return _streamers; }
			set{ _streamers = value; Save(); }
		}

		public static List<ulong> Channels{
			get{ return _channels; }
			set{ _channels = value; Save(); }
		}

		public static List<string> AnnouncedStreams{
			get{ return _announcedStreams; }
			set{ _announcedStreams = value; Save(); }
		}

		public static float UpdateFrequency{
			get{ return _updateFrequency; }
			set{ _updateFrequency = value; Save(); }
		}

		public static List<string> AnnounceMessages{
			get{ return _announceMessages; }
			set{ _announceMessages = value; Save(); }
		}

		public static List<ReactionRole> ReactionRoles{
			get{ return _reactionRoles; }
			set{ _reactionRoles = value; Save(); }
		}

		public static void Initialize(){
			lock(_discordClientID) _discordClientID = System.IO.File.ReadAllText(discordClientIDFile);
			lock(_twitchClientID) _twitchClientID = System.IO.File.ReadAllText(twitchClientIDFile);
			lock(_announcedStreams) _announcedStreams = new List<string>(System.IO.File.ReadAllLines(announcedStreamsFile));
			lock(_announceMessages) _announceMessages = new List<string>(System.IO.File.ReadAllLines(announceMessagesFile));

			lock(_channels){
				_channels = new List<ulong>();
				var channelText = new List<string>(System.IO.File.ReadAllLines(channelsFile));
				foreach(string s in channelText){
					if(ulong.TryParse(s, out ulong u)){
						_channels.Add(u);
					}
				}
			}

			if(float.TryParse(System.IO.File.ReadAllText(updateFrequencyFile), out float f)){
				_updateFrequency = f;
			}else{
				_updateFrequency = 5.0f;
			}

			lock(_streamers){
				_streamers = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(streamersFile));
				if(_streamers == null){
					_streamers = new Dictionary<string, string>();
				}
			}

			DeserializeRoles();

			Save();
			Debug.Log("Successfully loaded " + Streamers.Count + " streamer(s)");
		}

		public static void Save(){
			lock(_streamers) System.IO.File.WriteAllText(streamersFile, JsonConvert.SerializeObject(_streamers));
			lock(_announcedStreams) System.IO.File.WriteAllLines(announcedStreamsFile, _announcedStreams);
			lock(_announceMessages) System.IO.File.WriteAllLines(announceMessagesFile, _announceMessages);

			lock(_channels){
				var channelText = new List<string>();
				foreach(ulong u in _channels){
					channelText.Add(u.ToString());
				}

				System.IO.File.WriteAllLines(channelsFile, channelText);
			}

			System.IO.File.WriteAllText(updateFrequencyFile, _updateFrequency.ToString());

			SerializeRoles();
		}

		private static void SerializeRoles(){
			List<string> rrText = new List<string>();
			lock(_reactionRoles){
				foreach(ReactionRole rr in _reactionRoles){
					if(rr.emote != null){
						rrText.Add(JsonConvert.SerializeObject(rr));
					}
				}
			}

			System.IO.File.WriteAllLines(reactionRoleFile, rrText);
		}

		private static void DeserializeRoles(){
			List<ReactionRole> rrs = new List<ReactionRole>();
			List<string> fileText = new List<string>(System.IO.File.ReadAllLines(reactionRoleFile));

			foreach(string s in fileText){
				ReactionRole rr = JsonConvert.DeserializeObject<ReactionRole>(s);
				if(rr.emote != null){
					rrs.Add(JsonConvert.DeserializeObject<ReactionRole>(s));
				}
			}

			lock(_reactionRoles){
				_reactionRoles = rrs;
			}
		}
	}
}