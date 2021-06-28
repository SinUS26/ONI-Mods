using HarmonyLib;
using UnityEngine;
using System;

namespace ModifyTelepadPackages
{
    [HarmonyPatch(typeof(EditableTitleBar), "OnSpawn")]
    class EditableTitleBar_OnSpawn
    {
        private static void Postfix(EditableTitleBar __instance)
        {
//            StartUp<Engine, Settings, TranslationMod>.Settings.Initialize();
            Logger.Header(nameof(EditableTitleBar_OnSpawn));
            CharacterContainer component1 = (CharacterContainer)__instance.transform.parent.gameObject.GetComponent<CharacterContainer>();
            if (component1==null)
                return;
            CharacterSelectionController selectionController = (CharacterSelectionController)AccessTools.Field(component1.GetType(), "controller").GetValue(component1);
            if (__instance.editNameButton==null /*|| !StartUp<Engine, Settings, TranslationMod>.Settings.EnablePrintingPod */&& !selectionController.IsStarterMinion)
                return;
            GameObject gameObject = Util.KInstantiateUI(((Component)__instance.editNameButton).gameObject, ((Component)((KMonoBehaviour)__instance.editNameButton).transform.parent).gameObject, false);
            KButton componentInChildren = (KButton)gameObject.GetComponentInChildren<KButton>();
            ((ToolTip)((Component)componentInChildren).GetComponent<ToolTip>()).toolTip = Languages.TITLE;
            gameObject.transform.SetSiblingIndex(((Component)__instance.editNameButton).gameObject.transform.GetSiblingIndex() + 1);
            try
            {
                int num = 0;
                foreach (KImage componentsInChild in (KImage[])((Component)((KMonoBehaviour)componentInChildren).transform).gameObject.GetComponentsInChildren<KImage>())
                {
                    if (num == 1)
                    {
                        componentsInChild.sprite = (Sprite)null;
                        componentsInChild.sprite = Assets.GetSprite(/*HashedString.op_Implicit(*/(string)Db.Get().SkillGroups.Technicals.archetypeIcon/*)*/);
                        break;
                    }
                    ++num;
                }
            }
            catch (Exception ex)
            {
                Logger.Print(ex);
            }
            RectTransform transform = (RectTransform)gameObject.transform;
            Rect rect1 = transform.rect;
            float width = rect1.width;
            Rect rect2 = transform.rect;
            double height = (double)rect2.height;
            gameObject.name = "dupSettings";
            gameObject.transform.position = new Vector3((float)gameObject.transform.position.x - width - (float)(int)((double)width / 3.0), (float)gameObject.transform.position.y, (float)gameObject.transform.position.z);
            gameObject.SetActive(true);
            KButton component2 = (KButton)gameObject.GetComponent<KButton>();
            if (component2==null)
                return;
            try
            {
                CharacterContainer dup = (CharacterContainer)((Component)((KMonoBehaviour)__instance).transform.parent).GetComponent<CharacterContainer>();
//                component2.onClick += (System.Action)(() => PrintingEventsHook.OnClick(dup, gameObject));
// FIXING
            }
            catch (Exception ex)
            {
                Logger.Print(ex);
            }
            Logger.Print("Line67");
        }
    }
}
