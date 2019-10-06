# Batbot

This bot has two key purposes:  
1. Announce Twitch streams for specified Batman Arkham speedrunners only when they're streaming an Arkham game
2. Modify user roles based on reactions to specified messages (e.g. "react with a 👍 to get the 'Racing' role")

The bot isn't really doing anything new or innovative (it's really just using some basic aspects of the Twitch and Discord API's, and was created solely as a replacement for other bots), but I've found creating and hosting this myself to be more reliable than the public offerings with similar functionality. Perhaps someone may find this code useful, although this was setup specifically for use with the Batman speedrunning Discord server (and the bot itself is hosted by me exclusively for that server), but should be relatively easily to repurpose for use by other servers and for other games.

# Known Issues/Planned Improvements

-Currently the code isn't exactly versatile. As mentioned before, it was created for one specific purpose. I'd like to make it a bit more versatile and user-friendly for anyone who wants to use this as a starting point for their own bot (or just take it wholesale if it already does everything you need it to do).

-Data is stored in plain-text. This is mostly fine, but things like client IDs should be encrypted in some way.

-Non-speedrunning streams are frequently announced. This is not ideal as the focus of the bot is to announce speedrunning streams specifically. I have implemented an improvement for this, but it unfortunately depends on people marking their streams with [nosrl].

-If there are more than 100 streamers in the announcement list, the Twitch API call may fail. I haven't tested this so I'm not sure what exactly happens, but you can only pass 100 user IDs in a single call. In the event that more than 100 streamers are required, I would need to rework it so that it spreads them out over multiple Twitch API calls.

-It would be useful to be able to see how long it's been since a particular user last streamed, as that data may be relevant if you wish to purge your streamer list.
