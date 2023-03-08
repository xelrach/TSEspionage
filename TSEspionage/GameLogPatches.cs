/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    /**
     * Ways to export the Game Log from the UI.
     */
    public static class GameLogPatches
    {
        private static ILogger _log;
        private static GameLogWriter _gameLogWriter;

        private static readonly AccessTools.FieldRef<GameLog, List<GameLogItem>> LogItemListRef =
            AccessTools.FieldRefAccess<GameLog, List<GameLogItem>>("m_logItemList");

        public static void Init(ILogger log)
        {
            _gameLogWriter = new GameLogWriter("");
            _log = log;
        }

        [HarmonyPatch(typeof(GameLog), nameof(GameLog.UpdateGameLog))]
        public static class UpdateGameLogPatch
        {
            public static void Prefix(GameLog __instance, out LogEntry[] __state)
            {
                __state = LogItemListRef(__instance).Select(item => new LogEntry
                {
                    name = item.m_LogItemName.text,
                    desc = item.m_LogItemDesc.text,
                    detail = item.m_LogItemDetail.text
                }).ToArray();
            }

            public static void Postfix(LogEntry[] __state)
            {
                if (__state.Length == 0)
                {
                    return;
                }
                
                var gameId = TwilightLib.GetCurrentGameID();
                try
                {
                    _gameLogWriter.Write(gameId, __state);
                }
                catch (Exception e)
                {
                    _log.Log(LogType.Error, $"Failed writing game log for game {gameId}", e);
                }
            }
        }

        public class LogEntry
        {
            public string name;
            public string desc;
            public string detail;
        }

        private class GameLogWriter
        {
            private readonly string _gameLogDir;

            public GameLogWriter(string gameLogDir)
            {
                if (gameLogDir == "")
                {
                    _gameLogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Twilight Struggle");
                }
                else
                {
                    _gameLogDir = Path.GetFullPath(gameLogDir);
                }
            }

            public void Write(uint gameId, IEnumerable<LogEntry> entries)
            {
                CreateOutputDir();
                
                var outputFile = new StreamWriter(Path.Combine(_gameLogDir, $"{gameId}.txt"));
                foreach (var entry in entries)
                {
                    outputFile.WriteLine("{0}: {1}: {2}", entry.name, entry.desc, entry.detail);
                }

                outputFile.Close();
            }

            private void CreateOutputDir()
            {
                if (!Directory.Exists(_gameLogDir))
                {
                    Directory.CreateDirectory(_gameLogDir);
                }
            }
        }
    }
}