using HarmonyLib;
using TUNING;
using STRINGS;
using System.Collections.Generic;

namespace ModifyTelepadPackages
{
    public class ModifyTelepadPackages_Patches
    {
        public class Mod_OnLoad : KMod.UserMod2
        {
            //            public static void OnLoad()
            public override void OnLoad(Harmony harmony)
            {
                harmony.PatchAll();
//                ModUtil.RegisterForTranslation(typeof(Languages));
                Localization.RegisterForTranslation(typeof(Languages));
            }
        }

    }
}
