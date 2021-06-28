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
//            Debug.Log("CharacterSelectionController.InitializeContainers: Prefix()");
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
            if (!__instance.IsStarterMinion && ((Components.Cmps<MinionIdentity>)Components.LiveMinionIdentities).Count >= 5 /* StartUp<Engine, Settings, TranslationMod>.Settings.MaxDuplicants*/)
            {
                for (int index = 0; index < ___containers.Count; ++index)
                {
                    ITelepadDeliverableContainer deliverableContainer = ___containers[index];
                    deliverableContainer.GetGameObject().SetActive(false);
                    deliverableContainer.GetGameObject().transform.SetParent((Transform)null);
                    Object.Destroy((Object)deliverableContainer.GetGameObject());
                }
                ___containers.Clear();
                ___numberOfDuplicantOptions = 0;
                ___numberOfCarePackageOptions = 5 /* 4 */;
            }
            else
            {
                ___numberOfCarePackageOptions = 5 - ___numberOfDuplicantOptions - ___numberOfCarePackageOptions;
            }
            for (int index = 0; index < ___numberOfCarePackageOptions; ++index)
            {
                CarePackageContainer packageContainer = Util.KInstantiateUI<CarePackageContainer>(___carePackageContainerPrefab.gameObject, ___containerParent);
                packageContainer.SetController(__instance);
                ___containers.Add((ITelepadDeliverableContainer)packageContainer);
                packageContainer.gameObject.transform.SetSiblingIndex(UnityEngine.Random.Range(0, packageContainer.transform.parent.childCount));
            }
            ___numberOfCarePackageOptions = 5 - ___numberOfDuplicantOptions;
            ___selectedDeliverables = new List<ITelepadDeliverable>();
        }
    }
}
