![Server.pro logo](https://i.imgur.com/qEKFWKq.png)

# ServerProBot

### Introduction
The ServerProBot is a custom bot, developed for Server.pro's official Discord server. It is written in C# and can be hosted on any server with a .NET environment pre-configured, such as [Mono](https://www.mono-project.com/download/stable/#download-link).

This tool is still in development, and is available under the GPLv3 License.

### Basic commands
The bot comes with a number of pre-configured commands, all of which must be prepended with a period to be recognized and processed.

|Command|Argument|Description|
|--|--|--|
|`help`|-|Displays a list of commands with their descriptions.|
|-|`-c <command>`|Shows detailed information about a specified command, along with any available flags and an example. (Optional)|
|`ping`|-|Used to test whether or not the bot is online.|
|-|`-c <channel>`|Specify the channel in which the bot will respond. (Optional - will respond in current channel if not specified)|
|`sendraw`|-|Sends a manual POST request to the discord API from the bot. The contents of this request must be properly-formatted JSON in a code block.|
|-|`-c <channel>`|Specify the channel in which the bot will respond. (Optional - will respond in current channel if not specified)|
|`suggestion`|-|Formally sends a suggestion to #suggestions.|
|-|`-t <title>`|Add a brief explanation of the suggestion **(Required)**|
|-|`-b <body>`|The body portion of the suggestion **(Required)**|


