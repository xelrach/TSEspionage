/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
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

        public static void Init(GameLogWriter gameLogWriter, ILogger log)
        {
            _gameLogWriter = gameLogWriter;
            _log = log;
        }

        [HarmonyPatch(typeof(GameLog), nameof(GameLog.UpdateGameLog))]
        public static class UpdateGameLogPatch
        {
            public static void Postfix(GameLog __instance)
            {
                var entries = LogItemListRef(__instance).Select(item => new GameLogEntry
                {
                    name = item.m_LogItemName.text,
                    desc = item.m_LogItemDesc.text,
                    detail = item.m_LogItemDetail.text
                }).ToArray();

                if (entries.Length == 0)
                {
                    return;
                }

                var gameId = TwilightLibWrapper.GetCurrentGameId();
                try
                {
                    _gameLogWriter.Write(gameId, entries);
                }
                catch (Exception e)
                {
                    _log.Log(LogType.Error, $"Failed writing game log for game {gameId}", e);
                }
            }
        }
    }
}