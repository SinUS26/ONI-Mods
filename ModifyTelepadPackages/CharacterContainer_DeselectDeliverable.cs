using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModifyTelepadPackages
{
    [HarmonyPatch(typeof(CharacterContainer), "DeselectDeliverable")]
    internal class CharacterContainer_DeselectDeliverable
    {
        private static bool Prefix(
          CharacterContainer __instance,
          CharacterSelectionController ___controller,
          KToggle ___selectButton,
          Image ___titleBar,
          KBatchedAnimController ___animController,
          GameObject ___selectedBorder,
          Color ___deselectedTitleColor,
          MinionStartingStats ___stats)
        {
            ___controller.RemoveDeliverable((ITelepadDeliverable)___stats);
            ((ImageToggleState)((Component)___selectButton).GetComponent<ImageToggleState>()).SetInactive();
            ___selectButton.Deselect();
            ___selectButton.ClearOnClick();
//            ___selectButton.onClick += new Action(__instance.SelectDeliverable);
            ___selectButton.onClick += (System.Action)(() => __instance.SelectDeliverable());
            ___selectedBorder.SetActive(false);
            ((Graphic)___titleBar).color = ___deselectedTitleColor;
//            ((KAnimControllerBase)___animController)?.Queue(HashedString.op_Implicit("cheer_pst"), (KAnim.PlayMode)1, 1f, 0.0f);
//            ((KAnimControllerBase)___animController)?.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode)0, 1f, 0.0f);
            ___animController.Queue((HashedString)"cheer_pst");
            ___animController.Queue((HashedString)"idle_default", KAnim.PlayMode.Loop);
                    return false;
        }
    }
}
