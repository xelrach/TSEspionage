/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TSEspionage
{
    /**
     * Additions to the Settings menu.
     */
    public static class UI_SettingsMenuPatches
    {
        private static readonly int DEFAULT_FRAMERATE = 30;
        private static readonly string FRAMERATE_KEY = "Option_Framerate";

        private static Toggle _animationFastestToggle;
        private static GameObject _fpsPopup;

        [HarmonyPatch(typeof(UI_SettingsMenu), nameof(UI_SettingsMenu.OnAnimSpeedChanged))]
        public static class OnAnimSpeedChangedPatch
        {
            /**
             * Update the dropdown with the correct text for the toggle that has been selected.
             */
            public static bool Prefix(UI_SettingsMenu __instance)
            {
                var toggleCount = 0;
                if (__instance.m_animSpeedSlow.isOn)
                {
                    __instance.m_AnimationSpeedText.text = "Animation Speed - Slow";
                    __instance.m_animationSpeedPopupTitleText.text = "Animation Speed - Slow";
                    ++toggleCount;
                }

                if (__instance.m_animSpeedMed.isOn)
                {
                    __instance.m_AnimationSpeedText.text = "Animation Speed - Medium";
                    __instance.m_animationSpeedPopupTitleText.text = "Animation Speed - Medium";
                    ++toggleCount;
                }

                if (__instance.m_animSpeedFast.isOn)
                {
                    __instance.m_AnimationSpeedText.text = "Animation Speed - Fast";
                    __instance.m_animationSpeedPopupTitleText.text = "Animation Speed - Fast";
                    ++toggleCount;
                }

                if (_animationFastestToggle.isOn)
                {
                    __instance.m_AnimationSpeedText.text = "Animation Speed - Fastest";
                    __instance.m_animationSpeedPopupTitleText.text = "Animation Speed - Fastest";
                    ++toggleCount;
                }

                if (toggleCount == 1)
                {
                    __instance.m_AnimationSpeedPopup.SetActive(false);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(UI_SettingsMenu), nameof(UI_SettingsMenu.OnEnterMenu))]
        public static class OnEnterMenuPatch
        {
            /**
             * Update the settings UI to show our extra settings.
             */
            public static void Prefix(UI_SettingsMenu __instance)
            {
                var animationSpeed = PlayerPrefs.GetInt("Option_AnimSpeeds", 0);
                _animationFastestToggle.isOn = (animationSpeed == AnimateObjectPatches.AnimationFastest);
            }
        }

        [HarmonyPatch(typeof(UI_SettingsMenu), nameof(UI_SettingsMenu.OnExitMenu))]
        public static class OnExitMenuPatch
        {
            public static void Postfix(UI_SettingsMenu __instance)
            {
                var animationSpeed = 0;
                if (__instance.m_animSpeedMed.isOn)
                {
                    animationSpeed = 1;
                }
                else if (__instance.m_animSpeedFast.isOn)
                {
                    animationSpeed = 2;
                }
                else if (_animationFastestToggle.isOn)
                {
                    animationSpeed = 3;
                }

                PlayerPrefs.SetInt("Option_AnimSpeeds", animationSpeed);
            }
        }

        public static void Init()
        {
            SceneManager.sceneLoaded += (scene2, mode) => HandleSceneLoaded(scene2);
        }

        /**
         * When the settings scene is loaded, add our extra options.
         */
        private static void HandleSceneLoaded(Scene scene)
        {
            if (_animationFastestToggle == null && scene.path == "Assets/Screens/StartScreenAlt.unity")
            {
                foreach (var gameObj in scene.GetRootGameObjects())
                {
                    Debug.Log(gameObj);
                    if (gameObj.name == "Canvas")
                    {
                        var settingsMenu = gameObj.transform.Find(
                            "FrontEndRoot/Canvas_FrontEnd/Settings_Menu/SettingsMenu_Script");
                        var script = settingsMenu.GetComponent<UI_SettingsMenu>();

                        _animationFastestToggle = CreateFastestToggle(script);
                        // CreateFpsPopup(script);
                    }
                }
            }

            Application.targetFrameRate = GetSanitizedFramerate();
        }

        private static int GetSanitizedFramerate()
        {
            var fps = PlayerPrefs.GetInt(FRAMERATE_KEY, DEFAULT_FRAMERATE);
            if (fps < -1 || (fps >= 0 && fps <= 10))
            {
                fps = DEFAULT_FRAMERATE;
                PlayerPrefs.SetInt(FRAMERATE_KEY, fps);
            }

            return fps;
        }

        /**
         * Create a new option in the animation speed dropdown. This new button is for a new "Fastest" animation speed.
         */
        private static Toggle CreateFastestToggle(UI_SettingsMenu instance)
        {
            var scrollView = instance.m_AnimationSpeedPopup.transform.Find("ScrollView");
            var contentPanel = scrollView.Find("Content Panel");
            var fastButton = contentPanel.Find("Fast").gameObject;

            var fastestButton = Object.Instantiate(fastButton, contentPanel);
            var shadow = fastestButton.transform.GetChild(0).gameObject;
            var toggleBase = fastestButton.transform.GetChild(1).gameObject;
            var label = fastestButton.transform.GetChild(2).gameObject;

            // Update children
            fastestButton.name = "Fastest";
            shadow.name = "ShadowFastest";
            toggleBase.name = "BaseFastest";
            var tmpText = label.GetComponent<TMP_Text>();
            var uiLocalizedText = label.GetComponent<UILocalizedText>();
            tmpText.text = "Fastest";
            uiLocalizedText.KeyText = "Fastest";
            // uiLocalizedText.KeyText = "${Key_Fastest}";

            // Turn the new toggle off
            var fastestToggle = fastestButton.GetComponent<Toggle>();
            fastestToggle.isOn = false;

            // Update sizes and positions
            var scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(0, 0);
            scrollViewRect.anchoredPosition = new Vector2(0, -64);

            return fastestToggle;
        }

        private static void CreateFpsPopup(UI_SettingsMenu instance)
        {
            _fpsPopup = Object.Instantiate(instance.m_AnimationSpeedPopup, instance.m_AnimationSpeedPopup.transform.parent);
            _fpsPopup.name = "FPS_Popup";
            _fpsPopup.GetComponent<RegisterPopup>().popupName = "FPS Popup";
            _fpsPopup.transform.Find("Label (TMP)").GetComponent<TextMeshProUGUI>().text = "x FPS Popup";
            var contentPanel = _fpsPopup.transform.Find("Content Panel");
            foreach (Transform child in contentPanel.transform)
            {
                child.parent = null;
            }

            var buttons = _fpsPopup.gameObject.transform.parent.parent.parent.parent.Find("Buttons");
            var resolutionsButton = buttons.Find("Resolutions");
            var fpsButton = Object.Instantiate(resolutionsButton, resolutionsButton.transform.parent);
            fpsButton.name = "FPS";
            fpsButton.transform.SetSiblingIndex(1);
            fpsButton.Find("Label (TMP)").GetComponent<TextMeshProUGUI>().text = "x FPS Button";

            var clickEvent = new Button.ButtonClickedEvent();
            clickEvent.AddListener(ShowFpsPopup);
            fpsButton.GetComponent<Button>().onClick = clickEvent;
        }

        private static void ShowFpsPopup()
        {
            _fpsPopup.SetActive(true);
        }
        
        private static  string[] GetPlayersToHide()
        {
            var players = "".Split(new char[]{',',';'}, StringSplitOptions.RemoveEmptyEntries);

            return players.Select(player => player.Trim())
                .Where(trimmedPlayer => trimmedPlayer != "")
                .ToArray();
        }
    }
}