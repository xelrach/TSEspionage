/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Runtime.InteropServices;
using GameData;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TSEspionage
{
    public enum RegionId
    {
        Europe = 0,
        Asia = 1,
        MiddleEast = 2,
        Africa = 3,
        CentralAmerica = 4,
        SouthAmerica = 5,
        SouthEastAsia = 6
    }

    public enum RegionControl
    {
        None = 0,
        Presence = 1,
        Domination = 2,
        Control = 3,
    }

    /**
     * Changes to the gameplay UI.
     */
    public static class TwilightStrugglePatches
    {
        private static readonly int RegionScoreStateSize = Marshal.SizeOf(typeof(GameFinalScoreState));

        private static RegionControlBar _middleEastRegionControlBar;
        private static RegionControlBar _asiaRegionControlBar;

        private static readonly GameEventHandler _gameEventHandler = new GameEventHandler();

        public static void Init()
        {
        }

        [HarmonyPatch(typeof(TwilightStruggle), nameof(TwilightStruggle.Initialize))]
        public static class InitializePatch
        {
            private const float PresetsTextureWidth = 96;
            private const float PresetsTextureHeight = 372;
            private const float PresetsTextureCropAmount = 82;

            private const float InfluenceBar = 52;
            private const float InfluenceBarPadding = 2;

            private const float TabOffset = 37;

            private const int ChatInputScale = 48;
            private const int ChatInputTranslation = ChatInputScale / 2;

            /**
             * Create additional region scoring bars for Middle East and Africa
             */
            public static void Postfix()
            {
                var lowerHUD = GameObject.Find("/Canvas/GameRoot/uGui_HUD/HUD_Lower");
                // Trim the camera presets
                var cameraPresets = lowerHUD.transform.Find("CameraPresets");
                TrimCameraPresets(cameraPresets);

                // Change the GUI tabs
                var cardTray = lowerHUD.transform.Find("Local Player Card Tray").transform;
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

                // Fix the chat input box
                var chatInput = cardTray.transform.Find("Panel_Chat/Root_Chat/Panel_Input");
                chatInput.GetComponent<RectTransform>().sizeDelta += new Vector2(ChatInputScale, 0);
                chatInput.GetComponent<RectTransform>().anchoredPosition += new Vector2(ChatInputTranslation, 0);

                // Add additional region control bars to Asia and the Middle East (if needed)
                if (_middleEastRegionControlBar == null)
                {
                    var middleEastRegionSummary = GameObject.Find(
                        "/MapCanvas/Map/GameMap/RegionScoring/Scoring_MiddleEast");
                    var animationSpeed = middleEastRegionSummary.GetComponent<ScorePanel>().m_animateTime;

                    var finalInfluenceBar = CreateFinalScoringControlBar(middleEastRegionSummary);

                    _middleEastRegionControlBar = finalInfluenceBar.GetComponent<RegionControlBar>();
                    _middleEastRegionControlBar.Init(finalInfluenceBar, animationSpeed);
                }

                if (_asiaRegionControlBar == null)
                {
                    var asiaRegionSummary = GameObject.Find("/MapCanvas/Map/GameMap/RegionScoring/Scoring_Asia");
                    var animationSpeed = asiaRegionSummary.GetComponent<ScorePanel>().m_animateTime;

                    var finalInfluenceBar = CreateFinalScoringControlBar(asiaRegionSummary);

                    _asiaRegionControlBar = finalInfluenceBar.GetComponent<RegionControlBar>();
                    _asiaRegionControlBar.Init(finalInfluenceBar, animationSpeed);

                    // Move the Asia region summary up so it tis when its expanded
                    var transform = asiaRegionSummary.GetComponent<RectTransform>();
                    var regionPosition = transform.anchoredPosition;
                    transform.anchoredPosition =
                        new Vector2(regionPosition.x, regionPosition.y + InfluenceBar + InfluenceBarPadding);
                }
            }

            private static GameObject CreateFinalScoringControlBar(GameObject parent)
            {
                var influenceBar = parent.transform.Find("Influence").gameObject;
                var finalControlBar = Object.Instantiate(influenceBar, parent.transform);
                finalControlBar.name = "FinalInfluence";
                finalControlBar.AddComponent(typeof(RegionControlBar));
                finalControlBar.transform.SetSiblingIndex(influenceBar.transform.GetSiblingIndex() + 1);

                // Set position and size
                var barRectTransform = finalControlBar.GetComponent<RectTransform>();
                var barPosition = barRectTransform.anchoredPosition;
                barRectTransform.anchoredPosition = new Vector2(
                    barPosition.x,
                    barPosition.y - (InfluenceBar - InfluenceBarPadding) / 2.0f);
                var barDelta = barRectTransform.sizeDelta;
                barRectTransform.sizeDelta = new Vector2(barDelta.x, barDelta.y - InfluenceBar);

                return finalControlBar;
            }

            /**
             * Trim off the top of the camera preset columns so they don't cover as much of the map
             */
            private static void TrimCameraPresets(Transform cameraPresets)
            {
                var cropRect = new Rect(0, 0, PresetsTextureWidth, PresetsTextureHeight - PresetsTextureCropAmount);
                var sizeDelta = new Vector2(48f, 186f - PresetsTextureCropAmount / 2.0f);
                const float positionY = 92f - PresetsTextureCropAmount / 4.0f;

                // Left Side
                var leftPresets = cameraPresets.Find("Left");
                var leftImage = leftPresets.GetComponent<Image>();
                var leftShadow = cameraPresets.Find("ShadowLeft");

                leftImage.sprite = Sprite.Create(leftImage.sprite.texture, cropRect, new Vector2(0.5f, 0.5f));

                var leftTransform = leftPresets.GetComponent<RectTransform>();
                leftTransform.sizeDelta = sizeDelta;
                leftTransform.anchoredPosition = new Vector2(24, positionY);

                var leftShadowTransform = leftShadow.GetComponent<RectTransform>();
                leftShadowTransform.sizeDelta = sizeDelta;
                leftShadowTransform.anchoredPosition = new Vector2(24, positionY);

                // Right side
                var rightPresets = cameraPresets.Find("Right");
                var rightImage = rightPresets.GetComponent<Image>();
                var rightShadow = cameraPresets.Find("ShadowRight");

                rightImage.sprite = Sprite.Create(rightImage.sprite.texture, cropRect, new Vector2(0.5f, 0.5f));

                var rightTransform = rightPresets.GetComponent<RectTransform>();
                rightTransform.sizeDelta = sizeDelta;
                rightTransform.anchoredPosition = new Vector2(-24, positionY);

                var rightShadowTransform = rightShadow.GetComponent<RectTransform>();
                rightShadowTransform.sizeDelta = sizeDelta;
                rightShadowTransform.anchoredPosition = new Vector2(-24, positionY);
            }
        }

        /**
         * Manage the additional region scoring bars
         */
        [HarmonyPatch(typeof(TwilightStruggle), "Update")]
        public static class UpdatePatch
        {
            public static void Postfix()
            {
                if (_middleEastRegionControlBar == null || _asiaRegionControlBar == null)
                {
                    return;
                }

                var gcHandle = GCHandle.Alloc(new byte[RegionScoreStateSize], GCHandleType.Pinned);
                var ptr = gcHandle.AddrOfPinnedObject();
                if (TwilightLib.GetGameFinalScoreState(true, ptr, RegionScoreStateSize) != 0)
                {
                    var scoreState = (GameFinalScoreState)Marshal.PtrToStructure(ptr, typeof(GameFinalScoreState));

                    var middleEastScoreState = scoreState.region[(int)RegionId.MiddleEast];
                    _middleEastRegionControlBar.HandleRegionScore(middleEastScoreState);

                    var asiaScoreState = scoreState.region[(int)RegionId.Asia];
                    _asiaRegionControlBar.HandleRegionScore(asiaScoreState);
                }

                gcHandle.Free();
            }
        }

        /**
         * Listen to game events
         */
        [HarmonyPatch(typeof(TwilightStruggle), "HandleEvent")]
        public static class HandleEventPatch
        {
            public static void Prefix(ref IntPtr eventBuffer)
            {
                _gameEventHandler.HandleEvent(ref eventBuffer);
            }
        }
    }
}
