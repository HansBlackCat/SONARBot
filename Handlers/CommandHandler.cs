using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SONARBot.Handlers {
  class CommandHandler {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;

    public CommandHandler(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;
      _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
      _commandService = serviceProvider.GetRequiredService<CommandService>();

      HookEvents();
    }

    public void HookEvents() {
      _commandService.CommandExecuted += CommandCheck;
      _client.MessageReceived += HandleCommandAsync;
    }

    public async Task InstallCommandsAsync() {
      await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: _serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage message) {
      // Ignore System Message
      var msg = message as SocketUserMessage;
      int argPos = 0;
      if (msg == null)
        return;

      if (!(msg.HasCharPrefix('!', ref argPos) || msg.Author.IsBot ||
            msg.HasMentionPrefix(_client.CurrentUser, ref argPos)))
        return;

      var ctx = new SocketCommandContext(_client, msg);

      await _commandService.ExecuteAsync(context: ctx, argPos: argPos, services: _serviceProvider);
    }
    public async Task CommandCheck(Optional<CommandInfo> cmd, ICommandContext ctx, IResult res) {
      if (!cmd.IsSpecified)
        return;
      if (res.IsSuccess)
        return;
      await ctx.Channel.SendMessageAsync($"CommandCheckErr: {res}");
    }
  }

}
