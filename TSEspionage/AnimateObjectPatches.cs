using HarmonyLib;
using UnityEngine;

namespace TSEspionage
{
    public static class AnimateObjectPatches
    {
        // Option_AnimSpeeds has three possible options (originally): 0 for slow, 1 for medium, 2 for fast.
        // 3 has been added for fastest
        private const int AnimationSlow = 0;
        private const int AnimationMedium = 1;
        private const int AnimationFast = 2;
        private const int AnimationFastest = 3;

        // The existing implementation uses a resolution of 1024 for its movement calculations
        private const double InternalWidth = 1024d;
        
        private static readonly AccessTools.FieldRef<AnimateObject, float> AnimationDivisorRef =
            AccessTools.FieldRefAccess<AnimateObject, float>("m_optionScalar");

        public static void Init()
        {
            
        }
        
        [HarmonyPatch(typeof(AnimateObject), "SetOptionScalar")]
        public static class SetOptionScalarPatch
        {
            static bool Prefix(AnimateObject instance)
            {
                var slowdown = (PlayerPrefs.GetInt("Option_AnimSpeeds", 0)) switch
                {
                    AnimationFastest => 0.1d,
                    AnimationFast => 1d,
                    AnimationMedium => 1.5d,
                    AnimationSlow => 2d,
                    _ => 2d
                };

                AnimationDivisorRef(instance) = (float) (slowdown * InternalWidth / Screen.height);

                return false;
            }
        }

        [HarmonyPatch(typeof(AnimateObject), nameof(AnimateObject.StartAnimationToPlaceholder))]
        public static class StartAnimationToPlaceholderPatch
        {
            static void Prefix(AnimateObject instance, GameObject placeholder, ref float pauseAtDestination)
            {
                var animationSpeed = PlayerPrefs.GetInt("Option_AnimSpeeds", 0);
                pauseAtDestination = AdjustPause(animationSpeed, pauseAtDestination);
            }
        }

        [HarmonyPatch(typeof(AnimateObject), nameof(AnimateObject.StartAnimationToLocator))]
        public static class StartAnimationToLocatorPatch
        {
            static void Prefix(AnimateObject instance, AnimationLocator destinationLocator,
                AnimationLocator sourceLocator, ref float pauseAtDestination)
            {
                var animationSpeed = PlayerPrefs.GetInt("Option_AnimSpeeds", 0);
                pauseAtDestination = AdjustPause(animationSpeed, pauseAtDestination);
            }
        }

        private static float AdjustPause(int animationSpeed, float currentPause)
        {
            return animationSpeed switch
            {
                AnimationFast when currentPause > 0.1f => 0.1f,
                AnimationFastest when currentPause > 0.01667f => 0.01667f,
                _ => currentPause
            };
        }
    }
}