using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    public static class GameLogPatches
    {
        private static ILogger _log;
        private static GameLogWriter _gameLogWriter;

        private static readonly List<LogEntry> Entries = new List<LogEntry>();
        private static readonly AccessTools.FieldRef<GameLog, List<GameLogItem>> LogItemListRef =
            AccessTools.FieldRefAccess<GameLog, List<GameLogItem>>("m_logItemList");

        public static void Init(EspionageConfig config, ILogger log)
        {
            _gameLogWriter = new GameLogWriter(config);
            _log = log;
        }

        [HarmonyPatch(typeof(GameLog), nameof(GameLog.UpdateGameLog))]
        public static class UpdateGameLogPatch
        {
            static void Prefix(GameLog instance)
            {
                Entries.Clear();
                foreach (var item in LogItemListRef(instance))
                {
                    var entry = new LogEntry
                    {
                        name = item.m_LogItemName.text,
                        desc = item.m_LogItemDesc.text,
                        detail = item.m_LogItemDetail.text
                    };

                    Entries.Add(entry);
                }
            }

            static void Postfix()
            {
                var gameId = TwilightLib.GetCurrentGameID();
                try
                {
                    _gameLogWriter.Write(gameId, Entries);
                }
                catch (Exception e)
                {
                    _log.Log(LogType.Error, $"Failed writing game log for game {gameId}", e);
                }
            }
        }

        private class LogEntry
        {
            public string name;
            public string desc;
            public string detail;
        }

        private class GameLogWriter
        {
            private readonly string _gameLogDir;

            public GameLogWriter(EspionageConfig config)
            {
                if (config.gameLogDir == "")
                {
                    _gameLogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Twilight Struggle");
                }
                else
                {
                    _gameLogDir = Path.GetFullPath(config.gameLogDir);
                }
            }

            public void Write(uint gameId, List<LogEntry> entries)
            {
                using var outputFile = new StreamWriter(Path.Combine(_gameLogDir, $"{gameId}.txt"));
                foreach (var entry in entries)
                {
                    outputFile.WriteLine("{0}: {1}: {2}", entry.name, entry.desc, entry.detail);
                }

                outputFile.Flush();
            }
        }
    }
}