﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSEspionage
{
    /**
     * Patches the Twilight Struggle code.
     */
    public static class InitEspionage
    {
        private static bool _patched = false;

        public static void Init()
        {
            SceneManager.activeSceneChanged += Patch;
        }

        private static void Patch(Scene oldScene, Scene newScene)
        {
            if (!_patched)
            {
                var gameLogWriter = new GameLogWriter("");
                var gameEventHandler = new GameEventHandler(gameLogWriter);

                // Initialize the patch classes
                GameLogPatches.Init(gameLogWriter, Debug.unityLogger);
                LoadLevelSplashScreenPatches.Init();
                TwilightStrugglePatches.Init(gameEventHandler);
                UI_SettingsMenuPatches.Init();

                // Patch the TS assembly
                new Harmony("com.twilight-struggle.TSEspionage").PatchAll();

                _patched = true;
            }
        }
    }
}