using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SONARBot.Data;
using SONARBot.Handlers;
using Victoria;

namespace SONARBot.Services {
  public class DiscordEntry {
    private readonly ServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _client;
    private readonly ConfigData _configData;
    private readonly LavaNode _lavaNode;
    private readonly AudioService _audioService;
    private readonly CommandHandler _commandHandler;

    public DiscordEntry() {
      _serviceProvider = ConfigureServices();
      _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
      _configData = _serviceProvider.GetRequiredService<ConfigData>();
      _lavaNode = _serviceProvider.GetRequiredService<LavaNode>();
      _audioService = _serviceProvider.GetRequiredService<AudioService>();
      _commandHandler = _serviceProvider.GetRequiredService<CommandHandler>();

      SubscribeDiscordEvents();
    }

    public async Task MainAsync() {
      await InitializeGlobalDataAsync();
      await InitializeClientAsync();
      await InitializeCommandHandler();

      Console.WriteLine("Initialize Success");
      await Task.Delay(-1);
    }

    private void SubscribeDiscordEvents() {
      _client.Ready += OnReadyAsync;
      _client.MessageReceived += TestMessageReceivedAsync;

      _lavaNode.OnTrackEnded += _audioService.OnTrackEnded;
    }

    private async Task OnReadyAsync() {
      try {
        await _lavaNode.ConnectAsync();
      } catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }
    private async Task TestMessageReceivedAsync(SocketMessage message) {
      if (message.Author.Id == _client.CurrentUser.Id)
        return;
      if (message.Content == "!ping")
        await message.Channel.SendMessageAsync("pong!");
    }

    private static async Task InitializeGlobalDataAsync() {
      await ConfigData.InitializeAsync();
    }

    private async Task InitializeCommandHandler() {
      await _commandHandler.InstallCommandsAsync();
    }

    private async Task InitializeClientAsync() {
      await _client.LoginAsync(TokenType.Bot, ConfigData.Config.token);
      await _client.StartAsync();
    }

    private static ServiceProvider ConfigureServices() {
      return new ServiceCollection()
          .AddSingleton<DiscordSocketClient>()
          .AddSingleton<CommandService>()
          .AddSingleton<CommandHandler>()
          .AddSingleton<ConfigData>()
          .AddLavaNode(i =>
          {
            i.SelfDeaf = true;
    })
          .AddSingleton<AudioService>()
          .BuildServiceProvider();
  }
}
}
