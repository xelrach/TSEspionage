/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    /**
     * Fixes and improvements to animation speeds. 
     */
    public static class AnimateObjectPatches
    {
        // Option_AnimSpeeds has three possible options (originally): 0 for slow, 1 for medium, 2 for fast.
        // 3 has been added for fastest
        public const int AnimationSlow = 0;
        public const int AnimationMedium = 1;
        public const int AnimationFast = 2;
        public const int AnimationFastest = 3;

        // The existing implementation uses a resolution of 1024x768 (1280 diagonal) for its movement calculations
        private const double InternalDiag = 1280d;

        // Animation pauses
        private const float FastPause = 0.1f;
        private const float FastestPause = 0.066666f;

        private static readonly AccessTools.FieldRef<AnimateObject, float> AnimationDivisorRef =
            AccessTools.FieldRefAccess<AnimateObject, float>("m_optionScalar");

        [HarmonyPatch(typeof(AnimateObject), "SetOptionScalar")]
        public static class SetOptionScalarPatch
        {
            public static bool Prefix(AnimateObject __instance)
            {
                double slowdown;
                switch (PlayerPrefs.GetInt("Option_AnimSpeeds", AnimationSlow))
                {
                    case AnimationFastest:
                        slowdown = 0.25d;
                        break;
                    case AnimationFast:
                        slowdown = 1d;
                        break;
                    case AnimationMedium:
                        slowdown = 1.5d;
                        break;
                    case AnimationSlow:
                    default:
                        slowdown = 2d;
                        break;
                }

                var diag = Math.Sqrt(Math.Pow(Screen.width, 2) + Math.Pow(Screen.height, 2));
                var scale = InternalDiag / diag;
                AnimationDivisorRef(__instance) = (float)(slowdown * scale);

                return false;
            }
        }

        [HarmonyPatch(typeof(AnimateObject), nameof(AnimateObject.StartAnimationToPlaceholder))]
        public static class StartAnimationToPlaceholderPatch
        {
            public static void Prefix(ref GameObject placeholder, ref float pauseAtDestination)
            {
                var animationSpeed = PlayerPrefs.GetInt("Option_AnimSpeeds", 0);
                pauseAtDestination = AdjustPause(animationSpeed, pauseAtDestination);
            }
        }

        [HarmonyPatch(typeof(AnimateObject), nameof(AnimateObject.StartAnimationToLocator))]
        public static class StartAnimationToLocatorPatch
        {
            public static void Prefix(ref AnimationLocator destinationLocator, ref AnimationLocator sourceLocator,
                ref float pauseAtDestination)
            {
                var animationSpeed = PlayerPrefs.GetInt("Option_AnimSpeeds", 0);
                pauseAtDestination = AdjustPause(animationSpeed, pauseAtDestination);
            }
        }

        private static float AdjustPause(int animationSpeed, float currentPause)
        {
            switch (animationSpeed)
            {
                case AnimationFast when currentPause > FastPause:
                    return FastPause;
                case AnimationFastest when currentPause > FastestPause:
                    return FastestPause;
                default:
                    return currentPause;
            }
        }
    }
}
