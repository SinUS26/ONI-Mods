using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ModifyTelepadPackages
{
    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    class CharacterSelectionController_Patch  
    {
        private static void Prefix() 
        {
            Debug.Log("CharacterSelectionController.InitializeContainers: Prefix()");
        }
        private static void Postfix(ref CharacterSelectionController __instance,
            CarePackageContainer ___carePackageContainerPrefab,
            GameObject ___containerParent,
            ref List<ITelepadDeliverableContainer> ___containers,
            ref List<ITelepadDeliverable> ___selectedDeliverables,
            ref int ___numberOfDuplicantOptions,
            ref int ___numberOfCarePackageOptions ) 
        {
            Debug.Log("CharacterSelectionController.InitializeContainers: PostFix()");
        }
    }
}
