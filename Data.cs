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
		private static readonly string cooldownFile = "Data/Cooldown.txt";
		private static readonly string announceMessagesFile = "Data/AnnounceMessages.txt";
		private static readonly string reactionRoleFile = "Data/ReactionRoles.json";

		private static string _discordClientID = "";
		private static string _twitchClientID = "";
		private static Dictionary<string, StreamerInfo> _streamers = new Dictionary<string, StreamerInfo>();
		private static List<ulong> _channels = new List<ulong>();
		private static List<string> _announcedStreams = new List<string>();
		private static volatile float _updateFrequency = 0.0f;
		private static volatile float _cooldown = 0.0f;
		private static List<string> _announceMessages = new List<string>();
		private static List<ReactionRole> _reactionRoles = new List<ReactionRole>();
		private static Dictionary<string, string> _currentlyLive = new Dictionary<string, string>();

		public static string DiscordClientID{
			get{ return _discordClientID; }
			set{ _discordClientID = value; Save(); }
		}

		public static string TwitchClientID{
			get{ return _twitchClientID; }
			set{ _twitchClientID = value; Save(); }
		}

		public static Dictionary<string, StreamerInfo> Streamers{
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

		public static float Cooldown{
			get{ return _cooldown; }
			set{ _cooldown = value; Save(); }
		}

		public static List<string> AnnounceMessages{
			get{ return _announceMessages; }
			set{ _announceMessages = value; Save(); }
		}

		public static List<ReactionRole> ReactionRoles{
			get{ return _reactionRoles; }
			set{ _reactionRoles = value; Save(); }
		}

		public static Dictionary<string, string> CurrentlyLive{
			get{ return _currentlyLive; }
			set{ _currentlyLive = value; }
		}

		public static void Initialize(){
			CreateFiles();

			lock (_discordClientID) _discordClientID = System.IO.File.ReadAllText(discordClientIDFile);
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

			if(float.TryParse(System.IO.File.ReadAllText(cooldownFile), out float c)){
				_cooldown = c;
			}else{
				_cooldown = 1.0f;
			}

			//lock(_streamers){
			//	_streamers = JsonConvert.DeserializeObject<Dictionary<string, StreamerInfo>>(System.IO.File.ReadAllText(streamersFile));
			//	if(_streamers == null){
			//		_streamers = new Dictionary<string, StreamerInfo>();
			//	}
			//}

			DeserializeStreamers();
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
			System.IO.File.WriteAllText(cooldownFile, _cooldown.ToString());

			SerializeStreamers();
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

		private static void SerializeStreamers(){
			Dictionary<string, string> streamerText = new Dictionary<string, string>();
			lock(_streamers){
				foreach(var pair in _streamers){
					streamerText.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
				}
			}

			System.IO.File.WriteAllText(streamersFile, JsonConvert.SerializeObject(streamerText));
		}

		private static void DeserializeStreamers(){
			Dictionary<string, string> srs = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(streamersFile));
			if(srs == null){
				return;
			}

			foreach(var pair in srs){
				StreamerInfo info = null;
				try{
					info = JsonConvert.DeserializeObject<StreamerInfo>(pair.Value);
				}catch(Exception){
					info = new StreamerInfo(pair.Value);
				}

				if(info != null){
					lock(_streamers) _streamers.Add(pair.Key, info);
				}
			}
		}

		private static void CreateFiles(){
			if(!System.IO.Directory.Exists("Data")){
				System.IO.Directory.CreateDirectory("Data");
			}

			if(!System.IO.File.Exists(discordClientIDFile)){
				Console.WriteLine("Enter your Discord Client ID:");
				string id = Console.ReadLine();

				System.IO.FileStream stream = System.IO.File.Create(discordClientIDFile);
				stream.Close();

				System.IO.File.WriteAllText(discordClientIDFile, id);
			}

			if(!System.IO.File.Exists(twitchClientIDFile)){
				Console.WriteLine("Enter your Twitch Client ID:");
				string id = Console.ReadLine();

				System.IO.FileStream stream = System.IO.File.Create(twitchClientIDFile);
				stream.Close();

				System.IO.File.WriteAllText(twitchClientIDFile, id);
			}

			if(!System.IO.File.Exists(streamersFile)){
				System.IO.FileStream stream = System.IO.File.Create(streamersFile);
				stream.Close();
				System.IO.File.WriteAllText(streamersFile, JsonConvert.SerializeObject(new Dictionary<string, string>()));
			}

			if(!System.IO.File.Exists(channelsFile)){
				System.IO.FileStream stream = System.IO.File.Create(channelsFile);
				stream.Close();
			}

			if(!System.IO.File.Exists(announcedStreamsFile)){
				System.IO.FileStream stream = System.IO.File.Create(announcedStreamsFile);
				stream.Close();
			}

			if(!System.IO.File.Exists(updateFrequencyFile)){
				System.IO.FileStream stream = System.IO.File.Create(updateFrequencyFile);
				stream.Close();
			}

			if(!System.IO.File.Exists(cooldownFile)){
				System.IO.FileStream stream = System.IO.File.Create(cooldownFile);
				stream.Close();
				System.IO.File.WriteAllText(cooldownFile, (1.0f).ToString());
			}

			if(!System.IO.File.Exists(announceMessagesFile)){
				System.IO.FileStream stream = System.IO.File.Create(announceMessagesFile);
				stream.Close();
			}

			if(!System.IO.File.Exists(reactionRoleFile)){
				System.IO.FileStream stream = System.IO.File.Create(reactionRoleFile);
				stream.Close();
				//System.IO.File.WriteAllText(reactionRoleFile, JsonConvert.SerializeObject(new List<ReactionRole>()));
			}
		}
	}
}