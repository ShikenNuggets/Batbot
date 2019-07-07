# Batbot

This bot has two key purposes:  
1. Announce Twitch streams for specified Batman Arkham speedrunners only when they're streaming an Arkham game
2. Modify user roles based on reactions to specified messages (e.g. "react with a 👍 to get the 'Racing' role")

The bot isn't really doing anything new or innovative (it's really just using some basic aspects of the Twitch and Discord API's, and was created solely as a replacement for other bots), but I've found creating and hosting this myself to be more reliable than the public offerings with similar functionality. Perhaps someone may find this code useful, although this was setup specifically for use with the Batman speedrunning Discord server (and the bot itself is hosted by me exclusively for that server), but should be relatively easily to repurpose for use by other servers and for other games.

# Known Issues/Planned Improvements

-Currently the code isn't exactly versatile. As mentioned before, it was created for one specific purpose. I'd like to make it a bit more versatile and user-friendly for anyone who wants to use this as a starting point for their own bot (or just take it wholesale if it already does everything you need it to do).

-When you first build and attempt to run the bot, it will immediately crash with no warning. This is because it is expecting to read from several data files that do not exist unless they are manually created. Ideally, if the files do not exist, you should be able to input the relevant data directly into the bot console and then have it create the data files automatically.

-Data is stored in plain-text. This is mostly fine, but things like client IDs should be encrypted in some way.

-Non-speedrunning streams are frequently announced. This is not ideal as the focus of the bot is to announce speedrunning streams specifically. [Currently testing an improvement for this](https://github.com/ShikenNuggets/Batbot/pull/1).

-When a streamer briefly disconnects from Twitch and restarts their stream, a new announcement is sent. This can result in the bot posting many announcements for one channel in quick succession (worst case I've seen is 3 within a minute of each other). [Currently testing a fix for this](https://github.com/ShikenNuggets/Batbot/pull/1).
