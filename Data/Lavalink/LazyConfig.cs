using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SONARBot.Data.Lavalink {
  public sealed class LazyConfig {
    private LazyConfig() {}
    private static readonly Lazy<LazyConfig> lavaConfig =
        new Lazy<LazyConfig>(() => new LazyConfig());

    public static LazyConfig GetConfig {
      get { return lavaConfig.Value; }
    }
  }
}
