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