/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Runtime.InteropServices;
using GameEvent;

namespace TSEspionage
{
    /**
     * Listens for game events and dispatches the event values to various classes.
     */
    public class GameEventHandler
    {
        private const float GameLoaded = 2.0f;

        public readonly GameLogWriter GameLogWriter;

        public CardCountManager CardCountManager;

        public GameEventHandler(GameLogWriter gameLogWriter)
        {
            GameLogWriter = gameLogWriter ?? throw new ArgumentNullException(nameof(gameLogWriter));
        }

        public void HandleEvent(ref IntPtr eventBuffer)
        {
            var eventType = (EventType)Marshal.ReadIntPtr(eventBuffer).ToInt32();
            var eventPointer = eventBuffer + Marshal.SizeOf(typeof(int));

            switch (eventType)
            {
                case EventType.ActionRound:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.CardsAdded:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.CardPlayed:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.ChinaCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.CountryInfluence:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.DiscardsReshuffled:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.EventPlayed:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.GameOver:
                    var gameOver = Marshal.PtrToStructure<GameOver>(eventPointer);
                    var gameId = TwilightLibWrapper.GetCurrentGameId();
                    var players = TwilightLibWrapper.GetPlayers();
                    GameLogWriter.WriteGameOver(
                        gameId,
                        players.GetSuperpowerForPlayerId(gameOver.winner),
                        (GameOverType)gameOver.win_type
                    );
                    break;
                case EventType.HeadlineAnnounce:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.LoadProgress:
                    var loadProgress = (LoadProgress)Marshal.PtrToStructure(eventPointer, typeof(LoadProgress));
                    if (loadProgress.progress >= GameLoaded)
                    {
                        CardCountManager.UpdateCardCounts();
                    }

                    break;
                case EventType.OutputEventAnimationCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.PhasingPlayer:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.PopResolveCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.PopRevealCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.PushResolveCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.PushRevealCard:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.Reshuffle:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.EndTurn:
                    CardCountManager.UpdateCardCounts();
                    break;
                case EventType.LogUpdated:
                    var logUpdated = Marshal.PtrToStructure<LogUpdated>(eventPointer);
                    break;
            }
        }
    }
}