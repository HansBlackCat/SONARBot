using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SONARBot.Data {
  public sealed class ConfigScheme {
    public string token;
    public string default_prefix;
    public string voicechannel_id;
  }

  public class ConfigData {
    public static readonly string ConfigRoute = @"config.json";
    public static ConfigScheme Config;

    public static async Task InitializeAsync() {
      if (!File.Exists(ConfigRoute)) {
        throw new Exception($"No config.json file found on {Directory.GetCurrentDirectory()}");
      }

      Config = JsonConvert.DeserializeObject<ConfigScheme>(File.ReadAllText(ConfigRoute));
      await Task.Delay(1);
    }
  }
}
