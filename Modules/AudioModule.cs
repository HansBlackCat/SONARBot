using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;
using SONARBot.Services;
using SONARBot.Data;
using SONARBot.Handlers;

namespace SONARBot.Modules {
  public sealed class AudioModule : ModuleBase<SocketCommandContext> {
    private readonly LavaNode _lavaNode;
    private readonly AudioService _audioService;

    public AudioModule(LavaNode lavaNode, AudioService audioService) {
      _lavaNode = lavaNode;
      _audioService = audioService;
    }

    [Command("join"), Alias("Join", "J", "j")]
    public async Task JoinAsync() => await ReplyAsync(embed: await _audioService.Join(
        Context.Guild, Context.User as IVoiceState,
        Context.Guild.GetVoiceChannel(UInt64.Parse(ConfigData.Config.voicechannel_id)),
        Context.Channel as ITextChannel));

    [Command("quit"), Alias("Quit", "q", "Q")]
    public async Task LeaveAsync() => await ReplyAsync(embed: await _audioService.Leave(
        Context.Guild,
        Context.Guild.GetVoiceChannel(UInt64.Parse(ConfigData.Config.voicechannel_id))));

    [Command("add"), Alias("Add", "a", "A")]
    public async Task AddAndRunAsync([Remainder] string query) => await ReplyAsync(
        embed: await _audioService.Play(Context.Guild, Context.User as SocketGuildUser, query));

    [Command("DEBUG")]
    public async Task Debug() => await ReplyAsync(embed: await EmbedHandler.Error("I'm buggy"));
  }
}
