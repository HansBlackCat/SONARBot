using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SONARBot.Data.Lavalink {
  public sealed class LavaNode {
    private LavaNode() {}
    private static readonly Lazy<LavaNode> lavaNode = new Lazy<LavaNode>(() => new LavaNode());

    public static LavaNode GetNode {
      get { return lavaNode.Value; }
    }
  }
}
