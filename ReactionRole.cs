namespace Batbot{
	class ReactionRole{
		public ulong guildID = 0;
		public ulong messageID = 0;
		public string emote = null;
		public ulong role = 0;

		public ReactionRole(ulong guildID_, ulong message_, string emote_, ulong role_){
			guildID = guildID_;
			messageID = message_;
			emote = emote_;
			role = role_;
		}
	}
}