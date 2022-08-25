using System;
using System.IO;
using Tomlyn;
using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    public static class InitEspionage
    {
        public static void Init()
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

            // Initialize the patch classes
            GameLogPatches.Init(espionageConfig, Debug.unityLogger);
            AnimateObjectPatches.Init();

            // Patch the TS assembly
            var harmony = new Harmony("com.example.patch");
            harmony.PatchAll();
        }
    }
}