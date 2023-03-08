/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    /**
     * Additions to the Create Game menu. Mostly this adds more game time options.
     */
    public static class UI_OnlineCreateGamePatches
    {
        private const string GameTimePref = "OnlineCreateGame_GameTimeSecs";
        private const int DefaultGameTime = 90 * 60;
        
        private const int ScenarioStandard = 0;
        private const int ScenarioChinese = 1;
        private const int ScenarioLateWar = 2;
        private const int ScenarioTurnZero = 3;

        private const uint OptionsAdditionalCards = 1U;
        private const uint OptionsPromo1 = 2U;
        private const uint OptionsPromo2 = 4U;
        private const uint OptionsAltSpaceRace = 8U;

        private static int _gameTimePosition;

        private static readonly List<int> DefaultGameTimeList = new List<int>
        {
            20 * 60,
            30 * 60,
            45 * 60,
            60 * 60,
            90 * 60,
            2 * 60 * 60,
            3 * 60 * 60,
            4 * 60 * 60,
            6 * 60 * 60,
            24 * 60 * 60,
            3 * 24 * 60 * 60,
            7 * 24 * 60 * 60,
            21 * 24 * 60 * 60,
            45 * 24 * 60 * 60,
        };

        [HarmonyPatch(typeof(UI_OnlineCreateGame), "RetreiveSettings")]
        public static class RetrieveSettingsPatch
        {
            public static void Postfix()
            {
                var gameTimePref = PlayerPrefs.GetInt(GameTimePref, DefaultGameTime);
                _gameTimePosition = GetGameTimeList().BinarySearch(gameTimePref);
                if (_gameTimePosition < 0)
                {
                    _gameTimePosition = ~_gameTimePosition;
                }
            }
        }

        [HarmonyPatch(typeof(UI_OnlineCreateGame), "StoreSettings")]
        public static class StoreSettingsPatch
        {
            public static void Postfix()
            {
                var gameTime = GetGameTime();
                if (gameTime > 0)
                {
                    PlayerPrefs.SetInt(GameTimePref, gameTime);
                }
            }
        }

        [HarmonyPatch(typeof(UI_OnlineCreateGame), "OnEnterMenu")]
        public static class OnEnterMenuPatch
        {
            public static void Postfix(UI_OnlineCreateGame __instance)
            {
                UpdateGameTimeLabel(__instance, GetGameTime());
            }
        }

        [HarmonyPatch(typeof(UI_OnlineCreateGame), "OnTimerButtonPressed")]
        public static class OnTimerButtonPressedPatch
        {
            public static bool Prefix(UI_OnlineCreateGame __instance)
            {
                IncrementDameTimePosition(__instance);

                return false;
            }
        }

        [HarmonyPatch(typeof(UI_OnlineCreateGame), "OnCreateGameButtonPressed")]
        public static class OnCreateGameButtonPressedPatch
        {
            public static bool Prefix(UI_OnlineCreateGame __instance, ref Coroutine ___m_delayCoroutine)
            {
                __instance.m_messagePopupTitle.text = "Please Wait";
                __instance.m_messagePopupText.text = "Creating game...";
                __instance.m_messagePopup.SetActive(true);
                __instance.m_messagePopupCancelButton.SetActive(false);
                ___m_delayCoroutine = __instance.StartCoroutine((IEnumerator)CallInstanceMethod(
                    __instance,
                    "ProcessDelayTime",
                    new object[]
                    {
                        __instance.m_minDialogDisplayTime
                    }
                ));
                CreateGame(__instance);

                return false;
            }
        }

        private static void UpdateGameTimeLabel(UI_OnlineCreateGame menu, int secs)
        {
            menu.m_timerValueText.text = TimeSpan.FromSeconds(secs).ToString("g");
        }

        private static void IncrementDameTimePosition(UI_OnlineCreateGame menu)
        {
            ++_gameTimePosition;
            if (_gameTimePosition >= GetGameTimeList().Count)
            {
                _gameTimePosition = 0;
            }

            UpdateGameTimeLabel(menu, GetGameTime());
        }

        private static void DecrementDameTimePosition(UI_OnlineCreateGame menu)
        {
            --_gameTimePosition;
            if (_gameTimePosition < 0)
            {
                _gameTimePosition = GetGameTimeList().Count - 1;
            }

            UpdateGameTimeLabel(menu, GetGameTime());
        }

        private static List<int> GetGameTimeList()
        {
            return DefaultGameTimeList;
        }

        private static GameParameters CreateGameParameters(UI_OnlineCreateGame menu)
        {
            var gameParams = new GameParameters
            {
                additionalCardFlags = 0U
            };
            if (menu.m_optionalCards != null && menu.m_optionalCards.isOn)
            {
                gameParams.additionalCardFlags |= OptionsAdditionalCards;
            }

            if (menu.m_promoPackOne != null && menu.m_promoPackOne.isOn)
            {
                gameParams.additionalCardFlags |= OptionsPromo1;
            }

            if (menu.m_promoPackTwo != null && menu.m_promoPackTwo.isOn)
            {
                gameParams.additionalCardFlags |= OptionsPromo2;
            }

            if (menu.m_alternateSpaceRace != null && menu.m_alternateSpaceRace.isOn)
            {
                gameParams.additionalCardFlags |= OptionsAltSpaceRace;
            }

            if (menu.m_chineseCivilWar != null && menu.m_chineseCivilWar.isOn)
            {
                gameParams.scenario = ScenarioChinese;
            }
            else if (menu.m_debugLateWar != null && menu.m_debugLateWar.isOn)
            {
                gameParams.scenario = ScenarioLateWar;
            }
            else if (menu.m_turnZero != null && menu.m_turnZero.isOn)
            {
                gameParams.scenario = ScenarioTurnZero;
            }
            else
            {
                gameParams.scenario = ScenarioStandard;
            }

            var chooseSidesMethod = (int)GetInstanceField(menu, "m_ChooseSidesMethod") + 1;
            if (chooseSidesMethod == 1 && (bool)GetInstanceField(menu, "m_bPlayerSlotsSwapped"))
            {
                chooseSidesMethod = 0;
            }

            gameParams.chooseSidesMethod = chooseSidesMethod;
            gameParams.additionalInfluence =
                gameParams.chooseSidesMethod == 3 ? 0 : (int)menu.m_AdditionalInfluence.value;

            return gameParams;
        }

        private static void CreateGame(UI_OnlineCreateGame menu)
        {
            var gameParams = CreateGameParameters(menu);
            Network.CreateGame(
                2U,
                new uint[]
                {
                    (uint)GetInstanceField(menu, "m_inviteID"),
                    0U
                },
                (uint)GetGameTime(),
                gameParams
            );
        }

        private static object GetInstanceField<T>(T instance, string fieldName)
        {
            var field = AccessTools.Field(typeof(T), fieldName);
            return field.GetValue(instance);
        }

        private static object CallInstanceMethod<T>(T instance, string methodName, object[] args)
        {
            var method = AccessTools.Method(typeof(T), methodName);
            return method.Invoke(instance, args);
        }

        private static int GetGameTime()
        {
            return GetGameTimeList()[_gameTimePosition];
        }
    }
}