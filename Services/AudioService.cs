using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Victoria;
using Victoria.EventArgs;
using Victoria.Enums;
using Victoria.Responses.Rest;
using SONARBot.Handlers;

namespace SONARBot.Services {
  public sealed class AudioService {
    private readonly LavaNode _lavaNode;
    public AudioService(LavaNode lavaNode) => _lavaNode = lavaNode;

    public async Task<Embed> Join(IGuild guild, IVoiceState voiceState, IVoiceChannel voiceChannel,
                                  ITextChannel textChannel) {
      if (_lavaNode.HasPlayer(guild)) {
        return await EmbedHandler.Error("I'm already in channel!");
      }

      try {
        await _lavaNode.JoinAsync(voiceChannel, textChannel);
        return await EmbedHandler.Info("DJ SONAR ON");
      } catch (Exception e) {
        return await EmbedHandler.Error($"ErrMessage:\n{e.Message}");
      }

      if (voiceChannel.Name != voiceState.VoiceChannel.Name) {
        return await EmbedHandler.Info($"I'm in channel {voiceChannel.Name}");
      }
    }
    public async Task<Embed> Leave(IGuild guild, IVoiceChannel voiceChannel) {
      try {
        var player = _lavaNode.GetPlayer(guild);

        if (player.PlayerState is PlayerState.Playing) {
          await player.StopAsync();
        }
        await _lavaNode.LeaveAsync(voiceChannel);
        return await EmbedHandler.Info("B");
      } catch (InvalidOperationException e) {
        return await EmbedHandler.Info($"{e.Message}");
      }
    }

    public async Task<Embed> Play(IGuild guild, SocketGuildUser user, string query) {
      if (user.VoiceChannel == null)
        return await EmbedHandler.Error("First Join");
      if (!_lavaNode.HasPlayer(guild))
        return await EmbedHandler.Error("No Player Available");

      try {
        var player = _lavaNode.GetPlayer(guild);

        LavaTrack track;

        var search = Uri.IsWellFormedUriString(query, UriKind.Absolute)
                         ? await _lavaNode.SearchAsync(query)
                         : await _lavaNode.SearchYouTubeAsync(query);

        if (search.LoadStatus == LoadStatus.NoMatches)
          return await EmbedHandler.Error($"No Match query!\n{query}");

        var YT_rx = new Regex(@"youtu(?:.be|be.com)/(?:.v(?:/|=)|(?:./)?)([a-zA-Z0-9-_]+)",
                              RegexOptions.IgnoreCase);

        track = search.Tracks.FirstOrDefault();
        string duration =
            $"{track.Duration.Hours}:{track.Duration.Minutes}:{track.Duration.Seconds}";
        if (player.Track != null && player.PlayerState is PlayerState.Playing ||
            player.PlayerState is PlayerState.Paused) {
          player.Queue.Enqueue(track);
          return await EmbedHandler.Info($"Music Added:\n{track.Title}\n{track.Author}");
        } else {
          if (player.Queue.Count() > 0) {
            for (int i = 0; i < player.Queue.Count; ++i) {
              if (i == 0) {
                await player.PlayAsync(track);
                await EmbedHandler.MusicBox(track.Title, track.Author, duration);
              } else {
                player.Queue.Enqueue(search.Tracks[i]);
              }
            }
          } else {
            await player.PlayAsync(track);
            await EmbedHandler.MusicBox(track.Title, track.Author, duration);
          }

          await player.PlayAsync(track);
          return await EmbedHandler.MusicBox(track.Title, track.Author, duration);
        }

      } catch (Exception e) {
        Console.WriteLine(e.Message);
        return await EmbedHandler.Error($"{e.Message}");
      }
    }

    public async Task OnTrackEnded(TrackEndedEventArgs args) {
      if (!args.Reason.ShouldPlayNext())
        return;
      if (!args.Player.Queue.TryDequeue(out var queueable))
        return;
      if (queueable is not LavaTrack track)
        return;

      string duration = $"{track.Duration.Hours}:{track.Duration.Minutes}:{track.Duration.Seconds}";
      await args.Player.PlayAsync(track);
      await args.Player.TextChannel.SendMessageAsync(
          embed: await EmbedHandler.MusicBox(track.Title, track.Author, duration));
    }
  }

}
