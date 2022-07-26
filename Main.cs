using System.Threading;
using BepInEx;
using BepInEx.Logging;
using Corale.Colore.Core;
using HarmonyLib;
using ColoreColor = Corale.Colore.Core.Color;

namespace IsometricRGB
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string MOD_GUID = "io.raoul1808.isometricrgb";
        public const string MOD_NAME = "Isometric RGB";
        public const string MOD_VERSION = "1.0.0";

        public static ManualLogSource Log;

        public static LightingMode Mode = LightingMode.SingleColor;

        private void Awake()
        {
            Log = Logger;
            Chroma.Instance.Initialize();
            Thread.Sleep(1000);
            Chroma.Instance.SetAll(new ColoreColor(0, 0, 0));
            Harmony.CreateAndPatchAll(typeof(RgbPatches));
        }

        private class RgbPatches
        {
            [HarmonyPatch(typeof(PaletteBehaviour), nameof(PaletteBehaviour.OnFirstBeat))]
            [HarmonyPostfix]
            private static void GrabDarkPalette()
            {
                // TODO: get background color on witchcraft
            }
            
            [HarmonyPatch(typeof(PaletteBehaviour), nameof(PaletteBehaviour.OnBeat))]
            [HarmonyPostfix]
            private static void UpdatePassiveKeyboardRgb()
            {
                if (Mode != LightingMode.SingleColor)
                    return;
                var c = PaletteBehaviour.Instance.CurrentBackgroundColor;
                Chroma.Instance.SetAll(new ColoreColor(c.r, c.g, c.b));
            }

            [HarmonyPatch(typeof(PaletteBehaviour), nameof(PaletteBehaviour.LateUpdate))]
            [HarmonyPostfix]
            private static void UpdateActiveKeyboardRgb()
            {
                if (Mode == LightingMode.SingleColor || AudioManager.Instance.TimePerBeat == 0)
                    return;
                
                var dt = AudioManager.Instance.CurrentBeatTime / AudioManager.Instance.TimePerBeat;
                var c = PaletteBehaviour.Instance.CurrentBackgroundColor;

                switch (Mode)
                {
                    case LightingMode.ColorFlash:
                        Chroma.Instance.SetAll(new ColoreColor(c.r - c.r * dt, c.g - c.g * dt, c.b - c.b * dt));
                        break;
                    case LightingMode.WhiteFlash:
                        Chroma.Instance.SetAll(new ColoreColor(1f * dt, 1f * dt, 1f * dt));
                        break;
                    case LightingMode.WhiteColorFlash:
                        Chroma.Instance.SetAll(new ColoreColor(1f - (1f - c.r) * dt, 1f - (1f - c.g) * dt, 1f - (1f - c.b) * dt));
                        break;
                    default:
                        Chroma.Instance.SetAll(ColoreColor.White);
                        break;
                }
            }
        }
    }
}
