using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Tomlyn;
using HarmonyLib;

namespace TSEspionage
{
    public static class InitEspionage
    {
        public static void init()
        {
            // Read the config file
            var configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".TSEspionage");
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            var configPath = Path.Combine(configDir, "config.toml");
            if (!File.Exists(configPath))
            {
                File.Create(configPath);
            }

            var espionageConfig = Toml.ToModel<EspionageConfig>(File.ReadAllText(configPath));

            // Create the patch classes
            var logWriter = new GameLogWriter(espionageConfig);

            GameLogPatch.gameLogWriter = logWriter;
            GameLogPatch.log = new NullLogger<GameLogPatch>();

            // Patch the TS assembly
            var harmony = new Harmony("com.example.patch");
            harmony.PatchAll();
        }
    }
}