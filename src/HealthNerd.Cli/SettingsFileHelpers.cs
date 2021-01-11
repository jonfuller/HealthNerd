using System.IO;
using System.Linq;
using HealthKitData.Core.Excel.Settings;
using Newtonsoft.Json;

namespace HealthNerd.Cli
{
    public static class SettingsFileHelpers
    {
        public static Settings FromFile(string filename)
        {
            if (!File.Exists(filename)) return Settings.Default;

            var type = new[] { new { name = "", value = new object() } };
            var deserialized = JsonConvert.DeserializeAnonymousType(File.ReadAllText(filename), type);

            var settings = Settings.Default;
            foreach (var item in deserialized)
            {
                settings.SetValue(item.name, item.value);
            }
            return settings;
        }

        public static void ToFile(string filename, Settings settings)
        {
            var toSerialize = settings
               .Select(s => new {
                    name = s.Name,
                    value = s.JsonSerialization == SerializationBehavior.Nothing
                        ? s.Value
                        : s.Value.ToString()
                })
               .ToArray();

            File.WriteAllText(filename, JsonConvert.SerializeObject(toSerialize, Formatting.Indented));
        }
    }
}