using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the starup configuration
    /// </summary>
    public class StartupConfig
    {
        private static string StartupConfigFile => Path.Combine(Config.ApplicationDirectory, "startup.json");

        /// <summary>
        /// Gets or sets the database location
        /// </summary>
        [JsonPropertyName("database_location")]
        public string DatabaseLocation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupConfig"/> class
        /// </summary>
        /// <remarks>
        /// Dev note: Use <see cref="GetStartupConfig"/> instead of using this constructor.
        /// This is public so Json can deserialize it.
        /// </remarks>
        public StartupConfig()
        {
            DatabaseLocation = Config.ApplicationDirectory;
        }

        /// <summary>
        /// Gets the startup configuraion.
        /// </summary>
        /// <returns>A <see cref="StartupConfig"/> object</returns>
        public static StartupConfig GetStartupConfig()
        {
            try
            {
                if (File.Exists(StartupConfigFile))
                {
                    string json = File.ReadAllText(StartupConfigFile);
                    return JsonSerializer.Deserialize<StartupConfig>(json);
                }

                StartupConfig newConfig = new();
                Save(newConfig);
                return newConfig;
            }
            catch
            {
                return new StartupConfig();
            }
        }

        /// <summary>
        /// Save the configuration
        /// </summary>
        public void Save()
        {
            Save(this);
        }

        private static void Save(StartupConfig config)
        {
            try
            {
                string json = JsonSerializer.Serialize(config);
                Directory.CreateDirectory(Config.ApplicationDirectory);
                File.WriteAllText(StartupConfigFile, json);
            }
            catch { }
        }
    }
}