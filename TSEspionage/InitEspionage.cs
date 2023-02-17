using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSEspionage
{
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
                // Initialize the patch classes
                GameLogPatches.Init(Debug.unityLogger);
                LoadLevelSplashScreenPatches.Init();
                TwilightStrugglePatches.Init();
                UI_SettingsMenuPatches.Init();

                // Patch the TS assembly
                new Harmony("com.example.patch").PatchAll();

                _patched = true;
            }
        }
    }
}