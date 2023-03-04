# PLUS Emulator Discord bot
Developed by [IzzyDotexe](https://github.com/IzzyDotExe)

## How it works
This discord bot is integrated directly into the source for the Emulator, and thus can't be separated from it. It's built specifically to work with it. 

The first thing you need to do is create a bot account for it to run on. To get started head to the [Discord Developer Portal](https://discord.com/developers/applications) and create an application.

![image](https://i.imgur.com/a8rFrbj.png)

After You name it go to the bot section and create your bot. **Make sure to copy the Token**

Next go into the config.ini of the emulator and at the bottom enter your token and the IDs of your server, and the offline and online roles. Also set your prefered prefix

![image](https://i.imgur.com/0raspJJ.png)

**NOTE: Make sure to set up your bot intents, just check everyhing in the Privileged Gateway Intents section**

That's it you're set up! 

Once you run the emulator it will automatically load the discord bot and do it's thing.


## Code documentation

The bot is split into **services** and **modules** they are where all it's functionality lies. 

Services are the building blocks for the discord bot. They can do any number of things, in this case we have the HRP User service that manages the linking of users on discord and users on HRP, and we also have the CommandHandler service which enables the easy to use command handler *DsharpPlus.CommandsNext* 

Modules are groups of commands which are added to CommandsNext to create basic discord bot command functionality. Such as `!help` or `!ping` for example. 

### Adding a service
 
 To add a service you need to go into the DiscordManager.cs file and go down to the ConfigureClient Method. Over there you can add a new service like so:
 
 ```c#
 var services = new ServiceCollection()
    .AddSingleton<DiscordClient>(discordClient)
    .AddSingleton<NewServiceClass1>() // << New services
    .AddSingleton<NewServiceClass2>()
    // ... You can add as many as you like. 
    .AddSingleton<CommandHandler>()
    .AddSingleton<HRPUserService>()
    .BuildServiceProvider();
```

After adding a service you need to enable it in the BotClient() class like so...

```c#
// In order to enable a service we have to use the GetRequiredService method
_services.GetRequiredService<CommandHandler>();
_services.GetRequiredService<HRPUserService>();
_services.GetRequiredService<NewServiceClass1>(); // << New Service
_services.GetRequiredService<NewServiceClass2>();
// .. And so on
```

### Adding a module

To add a module to the bot for more commands, simply go into the CommandHandler service and add it in the constructor

```c#
// Register our modules
cmdsNext.RegisterCommands<Admin>();
cmdsNext.RegisterCommands<NewModuleClass1>(); // New Modules
cmdsNext.RegisterCommands<NewModuleClass2>();
// ... And so on
```

### Debugging

If you are adding things to the bot and are running into errors, you need to enable debug logging or you will have a tough time. Head over to the DiscordManager.cs file and set the LogLevel to Debug in the ConfigureClient() method

```c#
// Setting up the discord config
var discordConfig = new DiscordConfiguration()
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.All,
    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug

};

```
