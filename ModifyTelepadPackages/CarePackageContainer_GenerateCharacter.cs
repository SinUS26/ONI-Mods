using HarmonyLib;

namespace ModifyTelepadPackages
{
//    [HarmonyPatch(typeof(CarePackageContainer), "GenerateCharacter", new Type[] { typeof(bool) })]
    [HarmonyPatch(typeof(CarePackageContainer), "GenerateCharacter"]
    class CarePackageContainer_GenerateCharacter
    {
    }
}
