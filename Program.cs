using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SONARBot.Services;

namespace SONARBot {
  class Program {
    private readonly DiscordSocketClient _client;
    static Task Main(string[] args) => new DiscordEntry().MainAsync();

    public Program() {
      _client = new DiscordSocketClient();
      _client.Log += LogAsync;
      _client.Ready += ReadAsync;
      _client.MessageReceived += MessageReceivedAsync;
    }

    public async Task MainAsync() {
      await _client.LoginAsync(TokenType.Bot, "hi");
      await _client.StartAsync();
      await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log) {
      Console.WriteLine(log.ToString());
      return Task.CompletedTask;
    }

    private Task ReadAsync() {
      Console.WriteLine($"{_client.CurrentUser} is connected");
      return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message) {
      if (message.Author.Id == _client.CurrentUser.Id)
        return;
      if (message.Content == "!ping")
        await message.Channel.SendMessageAsync("pong!");
    }
    private async Task OnReadyAsync() {}
  }
}
