using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SONARBot.Handlers {
  public static class EmbedHandler {
    public static async Task<Embed> Error(string description, string title = "[ERROR] SONARBot") {
      return await Task.Run(() => new EmbedBuilder()
                                      .WithTitle(title)
                                      .WithDescription(description)
                                      .WithColor(Color.Red)
                                      .Build());
    }
    public static async Task<Embed> Info(string description, string title = "[INFO] SONARBot") {
      return await Task.Run(() => new EmbedBuilder()
                                      .WithTitle(title)
                                      .WithDescription(description)
                                      .WithColor(Color.Purple)
                                      .Build());
    }
    public static async Task<Embed> MusicBox(string title, string author,  // string img_url,
                                             string duration) {
      return await Task.Run(() => new EmbedBuilder()
                                      .WithTitle(title)
                                      .WithAuthor(author)
                                      .WithColor(Color.Blue)
                                      //.WithImageUrl(img_url)
                                      .WithFooter(duration)
                                      .Build());
    }
  }
}
