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
  }
  
  static class WrongClass {
    public static WrongAdd() { return int.MaxValue + 2; }
  }
}
