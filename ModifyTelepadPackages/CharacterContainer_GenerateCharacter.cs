using HarmonyLib;

namespace ModifyTelepadPackages
{
    //    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter", new Type[] { typeof(bool), typeof(string) })]
    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    class CharacterContainer_GenerateCharacter
    {
    }
}
