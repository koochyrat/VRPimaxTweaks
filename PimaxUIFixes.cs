using HarmonyLib;
using UnityEngine;

namespace VRPimaxTweaks
{
    [HarmonyPatch(typeof(MainMenuLoadButton), nameof(MainMenuLoadButton.Load))]
    public static class UIResetter
    {
        [HarmonyPostfix]
        public static void Postfix(MainMenuLoadButton __instance)
        {
            //reset when reloading game
            HUDFixer.Reset();
            ToolbarFixer.Reset();
            CompassFixer.Reset();
            SeaTruckHUDFixer.Reset();
            ExosuitHUDFixer.Reset();
            HoverbikeHUDFixer.Reset();
            SeaTruckSegmentFixer.Reset();
        }
    }

    [HarmonyPatch(typeof(uGUI_HealthBar), nameof(uGUI_HealthBar.LateUpdate))]
    public static class HUDFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(230, -300, 0);
        public static uGUI_HealthBar instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_HealthBar __instance)
        {
            if (!isFixed)
            {
                __instance.transform.parent.localPosition += offset;
                instance = __instance;
                isFixed = true;
                MiscSettings.SetUIScale(0.4f);
            }
        }
        public static void Reset()
        {
            if(instance)
                instance.transform.parent.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_QuickSlots), nameof(uGUI_QuickSlots.Update))]
    public static class ToolbarFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(0, -200, 0);
        public static uGUI_QuickSlots instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_QuickSlots __instance)
        {
            if (!isFixed)
            {
                __instance._rectTransform.localPosition += offset;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance._rectTransform.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_Compass), nameof(uGUI_Compass.UpdateAngle))]
    public static class CompassFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(0, -1000, 0);
        public static uGUI_Compass instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_Compass __instance)
        {
            if (!isFixed)
            {
                __instance.transform.parent.localPosition += offset;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance.transform.parent.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_SeaTruckHUD), nameof(uGUI_SeaTruckHUD.Update))]
    public static class SeaTruckHUDFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(-450, -300, 0);
        public static uGUI_SeaTruckHUD instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_SeaTruckHUD __instance)
        {
            if (!isFixed)
            {
                __instance.root.transform.localPosition += offset;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance.root.transform.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_ExosuitHUD), nameof(uGUI_ExosuitHUD.Update))]
    public static class ExosuitHUDFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(-450, -300, 0);
        public static uGUI_ExosuitHUD instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_ExosuitHUD __instance)
        {
            if (!isFixed)
            {
                __instance.root.transform.localPosition += offset;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance.root.transform.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_HoverbikeHUD), nameof(uGUI_HoverbikeHUD.Update))]
    public static class HoverbikeHUDFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(-450, -300, 0);
        public static uGUI_HoverbikeHUD instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_HoverbikeHUD __instance)
        {
            if (!isFixed)
            {
                __instance.root.transform.localPosition += offset;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance.root.transform.localPosition -= offset;
            isFixed = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_SeaTruckSegment), nameof(uGUI_SeaTruckSegment.SetHealth))]
    public static class SeaTruckSegmentFixer
    {
        public static bool isFixed;
        public static Vector3 offset = new Vector3(800, -580, 0);
        public static uGUI_SeaTruckSegment instance;
        [HarmonyPostfix]
        public static void Postfix(uGUI_SeaTruckSegment __instance)
        {
            if (!isFixed)
            {
                __instance.transform.parent.localPosition += offset;
                __instance.transform.parent.localScale = 0.4f * Vector3.one;
                instance = __instance;
                isFixed = true;
            }
        }
        public static void Reset()
        {
            if (instance)
                instance.transform.parent.localPosition -= offset;
            isFixed = false;
        }
    }

}
