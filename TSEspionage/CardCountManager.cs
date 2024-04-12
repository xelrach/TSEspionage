/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using UnityEngine;
using UnityEngine.Events;

namespace TSEspionage
{
    /**
     * Counts of cards in each hand/pile.
     */
    public class CardCounts
    {
        public readonly Players Players;
        public readonly EPlayer ChinaCardHolder;
        public readonly bool ChinaCardFaceUp;
        public readonly ushort UsaHandCount;
        public readonly ushort UssrHandCount;
        public readonly ushort DrawPileCount;
        public readonly ushort DiscardPileCount;
        public readonly ushort RemovedPileCount;

        private CardCounts(Players players, EPlayer chinaCardHolder, bool chinaCardFaceUp, ushort usaHandCount,
            ushort ussrHandCount, ushort drawPileCount, ushort discardPileCount, ushort removedPileCount)
        {
            Players = players;
            ChinaCardHolder = chinaCardHolder;
            ChinaCardFaceUp = chinaCardFaceUp;
            UsaHandCount = usaHandCount;
            UssrHandCount = ussrHandCount;
            DrawPileCount = drawPileCount;
            DiscardPileCount = discardPileCount;
            RemovedPileCount = removedPileCount;
        }

        public class Builder
        {
            public Players Players;
            public EPlayer ChinaCardHolder = EPlayer.NONE;
            public bool ChinaCardFaceUp;
            public ushort UsaHandCount;
            public ushort UssrHandCount;
            public ushort DrawPileCount;
            public ushort DiscardPileCount;
            public ushort RemovedPileCount;

            public CardCounts Build()
            {
                return new CardCounts(Players, ChinaCardHolder, ChinaCardFaceUp, UsaHandCount, UssrHandCount,
                    DrawPileCount, DiscardPileCount, RemovedPileCount);
            }
        }
    }

    /**
     * Listens for events that might have changed the counts of cards in hands or piles. Recalculates the counts and
     * sends them to listeners.
     */
    public class CardCountManager : MonoBehaviour
    {
        private readonly UnityEvent<CardCounts> _eventTrigger = new UnityEvent<CardCounts>();

        private static Players GetPlayers()
        {
            var localPlayerId = TwilightLibWrapper.GetPlayerId();
            var opposingPlayerId = TwilightLibWrapper.GetOpponentId();
            var superpowers = TwilightLibWrapper.GetSuperpowers(localPlayerId);

            return new Players(localPlayerId, opposingPlayerId, superpowers[localPlayerId],
                superpowers[opposingPlayerId]);
        }

        public void UpdateCardCounts()
        {
            var localPlayerId = TwilightLibWrapper.GetPlayerId();
            var opposingPlayerId = TwilightLibWrapper.GetOpponentId();
            var superpowers = TwilightLibWrapper.GetSuperpowers(localPlayerId);
            
            var players = new Players(localPlayerId, opposingPlayerId, superpowers[localPlayerId],
                superpowers[opposingPlayerId]);
            if (players.LocalSuperpower == EPlayer.NONE || players.OpposingSuperpower == EPlayer.NONE)
            {
                // Players have not yet been assigned sides
                return;
            }

            var usaHandState = TwilightLibWrapper.GetHand(players.GetUsaPlayerId());
            var ussrHandState = TwilightLibWrapper.GetHand(players.GetUssrPlayerId());

            var cardCounts = new CardCounts.Builder
            {
                Players = players,
                UsaHandCount = usaHandState.handCardCount,
                UssrHandCount = ussrHandState.handCardCount,
            };

            if (usaHandState.chinaCardFaceUp > 0)
            {
                cardCounts.ChinaCardHolder = EPlayer.US;
                cardCounts.ChinaCardFaceUp = true;
            }
            else if (usaHandState.chinaCardFaceDown > 0)
            {
                cardCounts.ChinaCardHolder = EPlayer.US;
                cardCounts.ChinaCardFaceUp = false;
            }
            else if (ussrHandState.chinaCardFaceUp > 0)
            {
                cardCounts.ChinaCardHolder = EPlayer.USSR;
                cardCounts.ChinaCardFaceUp = true;
            }
            else if (ussrHandState.chinaCardFaceDown > 0)
            {
                cardCounts.ChinaCardHolder = EPlayer.USSR;
                cardCounts.ChinaCardFaceUp = false;
            }

            var gameDeckCounts = TwilightLibWrapper.GetDeckCounts();

            cardCounts.DrawPileCount = (ushort)gameDeckCounts.draw_pile_count;
            cardCounts.DiscardPileCount = (ushort)gameDeckCounts.discard_pile_count;
            cardCounts.RemovedPileCount = (ushort)gameDeckCounts.removed_pile_count;

            _eventTrigger.Invoke(cardCounts.Build());
        }

        public void AddListener(UnityAction<CardCounts> callback)
        {
            _eventTrigger.AddListener(callback);
        }

        public void RemoveListener(UnityAction<CardCounts> callback)
        {
            _eventTrigger.RemoveListener(callback);
        }
    }

    public class Players
    {
        public readonly int LocalPlayerId;
        public readonly int OpposingPlayerId;
        public readonly EPlayer LocalSuperpower;
        public readonly EPlayer OpposingSuperpower;

        public Players(int localPlayerId, int opposingPlayerId, EPlayer localSuperpower, EPlayer opposingSuperpower)
        {
            LocalPlayerId = localPlayerId;
            OpposingPlayerId = opposingPlayerId;
            LocalSuperpower = localSuperpower;
            OpposingSuperpower = opposingSuperpower;
        }

        public int GetUsaPlayerId()
        {
            if (LocalSuperpower == EPlayer.US)
            {
                return LocalPlayerId;
            }

            if (OpposingSuperpower == EPlayer.US)
            {
                return OpposingPlayerId;
            }

            throw new InvalidOperationException();
        }

        public int GetUssrPlayerId()
        {
            if (LocalSuperpower == EPlayer.USSR)
            {
                return LocalPlayerId;
            }

            if (OpposingSuperpower == EPlayer.USSR)
            {
                return OpposingPlayerId;
            }

            throw new InvalidOperationException();
        }
    }
}