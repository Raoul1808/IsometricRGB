using System.Threading;
using BepInEx;
using Corale.Colore.Core;
using HarmonyLib;

namespace IsometricRGB
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string MOD_GUID = "io.raoul1808.isometricrgb";
        public const string MOD_NAME = "Isometric RGB";
        public const string MOD_VERSION = "1.0.0";
        
        void Awake()
        {
            Chroma.Instance.Initialize();
            Thread.Sleep(1000);
            Chroma.Instance.SetAll(new Color(0, 0, 0));
            Harmony.CreateAndPatchAll(typeof(RgbPatches));
        }

        class RgbPatches
        {
            [HarmonyPatch(typeof(PaletteBehaviour), nameof(PaletteBehaviour.OnBeat))]
            [HarmonyPostfix]
            private static void UpdateKeyboardRgb()
            {
                var c = PaletteBehaviour.Instance.CurrentBackgroundColor;
                Chroma.Instance.SetAll(new Color(c.r, c.g, c.b));
            }
        }
    }
}
