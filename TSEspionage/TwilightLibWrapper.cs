/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameData;

namespace TSEspionage
{
    /**
     * Wrapper around the messy awkwardness of TwilightLib.
     */
    public static class TwilightLibWrapper
    {
        private static readonly int DeckCountsSize = Marshal.SizeOf<GameDeckCounts>();
        private static readonly int PlayerDataSize = Marshal.SizeOf<PlayerData>();
        private static readonly int PlayerHandStateSize = Marshal.SizeOf<GamePlayerHandState>();
        private static readonly int RegionScoreStateSize = Marshal.SizeOf<GameFinalScoreState>();

        public static GameFinalScoreState GetGameFinalScoreState()
        {
            var handle = GCHandle.Alloc(new byte[RegionScoreStateSize], GCHandleType.Pinned);

            try
            {
                var ptr = handle.AddrOfPinnedObject();
                TwilightLib.GetGameFinalScoreState(true, ptr, RegionScoreStateSize);
                return (GameFinalScoreState)Marshal.PtrToStructure(ptr, typeof(GameFinalScoreState));
            }
            finally
            {
                handle.Free();
            }
        }

        public static uint GetCurrentGameId()
        {
            return TwilightLib.GetCurrentGameID();
        }

        public static int GetPlayerId()
        {
            return TwilightLib.GetLocalPlayerIndex();
        }

        public static int GetOpponentId()
        {
            return TwilightLib.GetLocalOpponentPlayerIndex(1);
        }

        public static GamePlayerHandState GetHand(int playerId)
        {
            var handle = GCHandle.Alloc(new byte[PlayerHandStateSize], GCHandleType.Pinned);

            try
            {
                var handBufferPtr = handle.AddrOfPinnedObject();
                TwilightLib.GetGamePlayerHandState(playerId, handBufferPtr, PlayerHandStateSize);
                return (GamePlayerHandState)Marshal.PtrToStructure(handBufferPtr, typeof(GamePlayerHandState));
            }
            finally
            {
                handle.Free();
            }
        }

        public static GameDeckCounts GetDeckCounts()
        {
            var handle = GCHandle.Alloc(new byte[DeckCountsSize], GCHandleType.Pinned);

            try
            {
                var pileBufferPtr = handle.AddrOfPinnedObject();
                TwilightLib.GetGameDeckCounts(pileBufferPtr, DeckCountsSize);
                return (GameDeckCounts)Marshal.PtrToStructure(pileBufferPtr, typeof(GameDeckCounts));
            }
            finally
            {
                handle.Free();
            }
        }

        public static Dictionary<int, EPlayer> GetSuperpowers(int localPlayerId)
        {
            var handle = GCHandle.Alloc(new byte[PlayerDataSize], GCHandleType.Pinned);

            try
            {
                var ptr = handle.AddrOfPinnedObject();
                var result = new Dictionary<int, EPlayer>(2);
                foreach (var instanceId in GetInstanceList(localPlayerId, 1, 2))
                {
                    TwilightLib.GetInstanceData(1, instanceId, ptr, PlayerDataSize);
                    var playerData = (PlayerData)Marshal.PtrToStructure(ptr, typeof(PlayerData));
                    result.Add(playerData.playerID, (EPlayer)playerData.superpower);
                }

                return result;
            }
            finally
            {
                handle.Free();
            }
        }

        private static IEnumerable<int> GetInstanceList(int playerId, int instanceType, int maxEvents)
        {
            var instanceIds = new int[2];
            var handle = GCHandle.Alloc(instanceIds, GCHandleType.Pinned);

            try
            {
                var instanceCount =
                    TwilightLib.GetInstanceList(playerId, instanceType, handle.AddrOfPinnedObject(), maxEvents);

                var result = new List<int>(instanceCount);
                for (var i = 0; i < instanceCount; i++)
                {
                    result.Add(instanceIds[i]);
                }

                return result;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}