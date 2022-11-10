# This project is no longer being maintained. I do not recommend using this.

# Batbot

This bot has two key purposes:  
1. Announce Twitch streams for specified Batman Arkham speedrunners only when they're streaming an Arkham game
2. Modify user roles based on reactions to specified messages (e.g. "react with a 👍 to get the 'Racing' role") (this functionality was disabled some time ago but the code is still present)

The bot isn't really doing anything new or innovative (it's just using some basic aspects of the Twitch and Discord API's, and was created solely as a replacement for other bots), but I've found creating and hosting this myself to be more reliable than the public offerings with similar functionality. Perhaps someone may find this code useful. This was setup specifically for use with the Batman speedrunning Discord server, but should be relatively easily to repurpose for use by other servers and for other games.

# Known Issues/Planned Improvements

-The bot is remarkably unstable (at least, it is on a low-end cloud VM running Windows Server 2012). I've done many things to attempt to improve its stability, but I'm still not quite happy with it.

-The bot is not versatile. As mentioned before, it was created for one specific purpose. I'd like to make it a bit more versatile and user-friendly for anyone who wants to use this as a starting point for their own bot (or just take it wholesale if it already does everything you need it to do).

-Data is stored in plain-text. This is mostly fine, but things like client IDs should perhaps not be.

-Non-speedrunning streams are frequently announced. This is not ideal as the focus of the bot is to announce speedrunning streams specifically. I have implemented an improvement for this, but it unfortunately depends on people properly marking their streams with [nosrl].

-The bot can only announce Twitch streamers. I'd like if it could announce streams from other platforms as well (YouTube, Mixer, etc), but unfortunately those do not provide a way for me to filter streams by game, making it impossible to implement them correctly. Even if this was possible it would be fairly low priority. As of writing there are no active Batman speedrunners on other platforms (and in general, the speedrunning community almost exclusively uses Twitch).

-Having the bot program be a GUI instead of a command line program would be nice. But it would also be a lot of work that would benefit one person (and may even make it run worse on the low-end cloud VM I have it up on). Maybe someday.
