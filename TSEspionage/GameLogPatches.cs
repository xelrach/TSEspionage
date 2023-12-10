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

        public static void Init(ILogger log)
        {
            _gameLogWriter = new GameLogWriter("");
            _log = log;
        }

        [HarmonyPatch(typeof(GameLog), nameof(GameLog.UpdateGameLog))]
        public static class UpdateGameLogPatch
        {
            public static void Prefix(GameLog __instance, out GameLogEntry[] __state)
            {
                __state = LogItemListRef(__instance).Select(item => new GameLogEntry
                {
                    name = item.m_LogItemName.text,
                    desc = item.m_LogItemDesc.text,
                    detail = item.m_LogItemDetail.text
                }).ToArray();
            }

            public static void Postfix(GameLogEntry[] __state)
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
    }
}
