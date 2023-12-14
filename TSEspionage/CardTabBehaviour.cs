/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using TMPro;
using UnityEngine;

namespace TSEspionage
{
    /**
     * Controls the card tabs in the lower HUD. Moves them to a better place on-screen and adds a card count to
     * each tab.
     */
    public class CardTabBehaviour : MonoBehaviour
    {
        private const float TabOffset = 37;

        private CardCountManager _cardCountManager;

        private TextMeshProUGUI _playerHandTmp;
        private TextMeshProUGUI _discardedPileTmp;
        private TextMeshProUGUI _removedPileTmp;
        private TextMeshProUGUI _playerHandActiveTmp;
        private TextMeshProUGUI _discardedPileActiveTmp;
        private TextMeshProUGUI _removedPileActiveTmp;

        private string _origPlayerHandText;
        private string _origDiscardPileText;
        private string _origRemovedPileText;

        public void Initialize(CardCountManager cardCountManager, Transform cardTray)
        {
            _cardCountManager = cardCountManager;

            // Move the card tabs
            var handTab = cardTray.Find("HandTab");
            handTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);
            var discardTab = cardTray.Find("DiscardTab");
            discardTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);
            var removedTab = cardTray.Find("RemovedTab");
            removedTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);

            var activeHandTab = cardTray.Find("Hand Display").Find("HandTab");
            activeHandTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);
            var activeDiscardTab = cardTray.Find("Discard Display").Find("DiscardTab");
            activeDiscardTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);
            var activeRemovedTab = cardTray.Find("Removed Display").Find("RemovedTab");
            activeRemovedTab.GetComponent<RectTransform>().anchoredPosition -= new Vector2(TabOffset, 0);

            // Add handler for card counts
            _playerHandTmp = handTab.Find("HandTab (TMP)").GetComponent<TextMeshProUGUI>();
            _discardedPileTmp = discardTab.Find("DiscardTab (TMP)").GetComponent<TextMeshProUGUI>();
            _removedPileTmp = removedTab.Find("RemovedTab (TMP)").GetComponent<TextMeshProUGUI>();

            _playerHandActiveTmp = activeHandTab.Find("Hand (TMP)").GetComponent<TextMeshProUGUI>();
            _discardedPileActiveTmp = activeDiscardTab.Find("Discard (TMP)").GetComponent<TextMeshProUGUI>();
            _removedPileActiveTmp = activeRemovedTab.Find("Removed (TMP)").GetComponent<TextMeshProUGUI>();

            _origPlayerHandText = _playerHandTmp.text;
            _origDiscardPileText = _discardedPileTmp.text;
            _origRemovedPileText = _removedPileTmp.text;
        }

        public void Start()
        {
            _cardCountManager.AddListener(UpdateText);
        }

        public void OnDestroy()
        {
            _cardCountManager.RemoveListener(UpdateText);
        }

        /**
         * Updates the text of the card tabs with the count of each hand or pile.
         */
        public void UpdateText(CardCounts cardCounts)
        {
            ushort playerCount;
            ushort opponentCount;
            if (cardCounts.Players.LocalSuperpower == EPlayer.US)
            {
                playerCount = cardCounts.UsaHandCount;
                opponentCount = cardCounts.UssrHandCount;
            }
            else
            {
                playerCount = cardCounts.UssrHandCount;
                opponentCount = cardCounts.UsaHandCount;
            }

            var playerChina = "";
            var opponentChina = "";
            if (cardCounts.Players.LocalSuperpower == cardCounts.ChinaCardHolder)
            {
                playerChina = cardCounts.ChinaCardFaceUp ? " + C" : " - C";
            }
            else if (cardCounts.Players.OpposingSuperpower == cardCounts.ChinaCardHolder)
            {
                opponentChina = cardCounts.ChinaCardFaceUp ? " + C" : " - C";
            }

            var playerText = $"{_origPlayerHandText} ({playerCount}{playerChina})";
            _playerHandTmp.text = playerText;
            _playerHandActiveTmp.text = playerText;

            var discardText = $"{_origDiscardPileText} ({cardCounts.DiscardPileCount})";
            _discardedPileTmp.text = discardText;
            _discardedPileActiveTmp.text = discardText;

            var removedText = $"{_origRemovedPileText} ({cardCounts.RemovedPileCount})";
            _removedPileTmp.text = removedText;
            _removedPileActiveTmp.text = removedText;
        }
    }
}