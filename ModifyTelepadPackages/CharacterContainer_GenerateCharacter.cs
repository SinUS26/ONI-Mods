using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ModifyTelepadPackages
{
    //    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter", new Type[] { typeof(bool), typeof(string) })]
    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    class CharacterContainer_GenerateCharacter
    {
        private static void Postfix(
            CharacterContainer __instance,
            CharacterSelectionController ___controller,
            bool is_starter,
            string guaranteedAptitudeID = null)
        {
            Logger.Print("CharacterContainer.GenerateCharacter: Prefix()");
            EditableTitleBar editableTitleBar = (EditableTitleBar)AccessTools.Field(__instance.GetType(), "characterNameTitle").GetValue(__instance);
            if (editableTitleBar==null)
                return;
//            Logger.Print("editableTitleBar");
            Transform parent = editableTitleBar.transform.parent;
            GameObject childWithName1 = parent.gameObject.FindChildWithName("ArchetypeSelect");
            if (childWithName1!=null)
                childWithName1.SetAnchoredPosition(new Vector2(-264f, 24f), Vector2.one, Vector2.one, new Vector2(1f, 0.5f));
            parent.FindOrAddUnityComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            CharacterHelper.UpdateVisuals(__instance);
            GameObject parentWithName = parent.gameObject.FindParentWithName("Content");
            if (parentWithName!=null)
            {
                GameObject childWithName2 = parentWithName.gameObject.FindChildWithName("BottomContent");
                if (childWithName2!=null)
                {
                    //                    M0 m0 = childWithName2.AddComponent<LayoutElement>();
                    LayoutElement m0 = childWithName2.AddComponent<LayoutElement>();
                    m0.preferredHeight = (float)CharacterHelper.GetBottomHeight(__instance);
                    m0.minHeight = (float)CharacterHelper.GetBottomHeight(__instance);
                }
            }
            foreach (ITelepadDeliverableContainer characterContainer in (List<ITelepadDeliverableContainer>)AccessTools.Field(((object)___controller).GetType(), "containers").GetValue((object)___controller))
                CharacterHelper.AdjustContainerHeight(characterContainer);
        }

    }
}
