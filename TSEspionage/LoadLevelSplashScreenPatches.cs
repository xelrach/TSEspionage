/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using HarmonyLib;

namespace TSEspionage
{
    public static class LoadLevelSplashScreenPatches
    {
        public static void Init()
        {
        }

        [HarmonyPatch(typeof(LoadLevelSplashScreen), nameof(LoadLevelSplashScreen.BeginLoadingSequence))]
        public static class BeginLoadingSequencePatch
        {
        }
    }
}