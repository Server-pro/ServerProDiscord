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

### Installation
The commands below are specific to Ubuntu 20.04 LTS. If you are using a different distribution, please modify the process accordingly.

1. If Git is not already installed on the server, install it.
```bash
sudo apt install git -y
```

2. Download this repository to your server.
```bash
git clone https://github.com/CatSandwich/ServerProDiscord.git
```

3. Navigate into the downloaded folder.
```bash
cd ServerProDiscord
```

4. Install nuget.
```bash
sudo apt-get install nuget -y
```

5. Install .NET Core.
```bash
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
```
```bash
sudo dpkg -i packages-microsoft-prod.deb
```
```bash
rm packages-microsoft-prod.deb
```
```bash
sudo apt-get update
```
```bash
sudo apt-get install apt-transport-https -y
```
```bash
sudo apt-get install dotnet-sdk-3.1 -y
```
```bash
sudo apt-get install aspnetcore-runtime-3.1 -y
```

6. Install Mono
```bash
sudo apt install gnupg ca-certificates -y
```
```bash
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
```
```bash
echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
```
```bash
sudo apt update -y && apt upgrade -y
```
```bash
sudo apt install mono-devel -y
```

7. Install Discord.NET
```bash
nuget restore
```
8. Obtain bot token and create `config.json`
Firstly navigate to the [Discord Developer Portal](https://discordapp.com/developers/applications/).
Once there, click "New Application" and fill in the details requested (including a name, icon and description).

Next, select "Bot" in the menu, and click "Add Bot", followed by "Yes, do it!". You should receive a confirmation message. Under Token, select "Click to Reveal Token".

Navigate back to the OAuth2 tab. Scroll down to the "OAuth2 URL Generator" and enable `bot` under Scopes. Next, under "Bot Permissions" enable the `Administrator` permission (or be more granular if you wish).

Copy the URL that is displayed further up the page, navigate to that URL, and add the bot to your server.

Lastly, rename `config.example.json` to be `config.json`. Then, open the file with your favourite text editor, and configure it to your liking. If you want to enable the developer environment, then leave DevEnv as `true`. Insert the Token for your server in the `Token` field, and do the same for `DevToken` if you have a separate development / testing server. Next, create a `bot-rcon` channel on your server and insert the ID for this channel in the `RconChannel` field - please be aware that this can cause problems due to Discord's Rate Limiting.

Once you bring the bot online, it should be working in Discord.

9. Build the project
```bash
msbuild
```

### Running the bot
We will run the bot inside a GNU Screen. If you don't already have this installed, run `sudo apt install screen -y` to do so.

Firstly, create a new screen:
```bash
screen
```

Then, navigate to the runpath:
```bash
cd ServerProDiscord/bin/Debug/netcoreapp3.1
```

and run it.
```bash
./ServerProDiscord
```

To detach from the screen, press `Ctrl+a+d`. You can reconnect to the screen by finding the screen's ID with `screen -list`, and then entering the command `screen -R <ID>`.

If you have any issues with installing or running the bot, please open an issue.

