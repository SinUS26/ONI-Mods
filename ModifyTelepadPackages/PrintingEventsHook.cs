using Database;
using HarmonyLib;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using Attribute = Klei.AI.Attribute;

namespace ModifyTelepadPackages
{
    public static class PrintingEventsHook
    {
        private static readonly List<CollapsibleDetailContentPanel> _itemsToDestroy = new List<CollapsibleDetailContentPanel>();
        internal static Dictionary<CharacterContainer, PrintingEventsHook.TraitsSet> Sets = new Dictionary<CharacterContainer, PrintingEventsHook.TraitsSet>();
        private static Color? _oldColor;
        private static TMP_FontAsset _oldFont;
        private static TMP_FontAsset _headerFont;
        private static Color? _headerColor;
        private static TextStyleSetting _style;
        private static Color? _defaultcolor;
        private static int _fontSize;

        public static void ClearContainers()
        {
            foreach (CollapsibleDetailContentPanel detailContentPanel in PrintingEventsHook._itemsToDestroy)
            {
                try
                {
                    if (detailContentPanel != null)
                    {
                        detailContentPanel.gameObject?.SetActive(false);
                        UnityEngine.Object.Destroy(detailContentPanel);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print(ex);
                }
            }
        }

        private static void FillSet(
          List<DUPLICANTSTATS.TraitVal> set,
          List<Trait> dupTraitsSet,
          List<DUPLICANTSTATS.TraitVal> original)
        {
            for (int index = 0; index < dupTraitsSet.Count; ++index)
            {
                Trait trait = dupTraitsSet[index];
                set.Add(((IEnumerable<DUPLICANTSTATS.TraitVal>)original).FirstOrDefault<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(t => (string)t.id == (string)((Resource)trait).Id)));
            }
        }

        private static void PickNextTrait(
          Trait lastTrait,
          List<DUPLICANTSTATS.TraitVal> currentTraits,
          List<DUPLICANTSTATS.TraitVal> original,
          List<DUPLICANTSTATS.TraitVal> excludeList = null)
        {
            int index1 = 0;
            for (int index2 = 0; index2 < currentTraits.Count; ++index2)
            {
                DUPLICANTSTATS.TraitVal currentTrait = currentTraits[index2];
                if ((string)((Resource)lastTrait).Id == (string)currentTrait.id)
                {
                    index1 = index2;
                    break;
                }
            }
            int num = 0;
            for (int i = 0; i < original.Count; i++)
            {
                if ((string)original[i].id == (string)((Resource)lastTrait).Id)
                    ++num;
                else if (num > 0 && !CharacterHelper.IsMutuallyExclusive(excludeList, original[i]) && ((IEnumerable<DUPLICANTSTATS.TraitVal>)currentTraits).All<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(c => (string)c.id != (string)original[i].id)))
                {
                    currentTraits[index1] = original[i];
                    return;
                }
            }
            DUPLICANTSTATS.TraitVal traitVal = ((IEnumerable<DUPLICANTSTATS.TraitVal>)original).FirstOrDefault<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => ((IEnumerable<DUPLICANTSTATS.TraitVal>)currentTraits).All<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(c => (string)c.id != (string)u.id)) && !CharacterHelper.IsMutuallyExclusive(excludeList, u)));
            currentTraits[index1] = traitVal;
        }

        public static void OnClick(
          CharacterContainer dup,
          GameObject go,
          CollapsibleDetailContentPanel panel = null,
          Trait prevTrait = null,
          PrintingEventsHook.ChangeType changeType = PrintingEventsHook.ChangeType.PositiveTrait,
          int attributeIndex = 0)
        {
            bool flag = false;
            if (!PrintingEventsHook.Sets.ContainsKey(dup))
            {
                if (!PrintingEventsHook.Sets.ContainsKey(dup))
                    PrintingEventsHook.Sets.Add(dup, new PrintingEventsHook.TraitsSet());
                PrintingEventsHook.FillSet(PrintingEventsHook.Sets[dup].Positive, CharacterHelper.GetPositiveTraits(dup), (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.GOODTRAITS);
                PrintingEventsHook.FillSet(PrintingEventsHook.Sets[dup].Negative, CharacterHelper.GetNegativeTraits(dup), (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.BADTRAITS);
                foreach (DUPLICANTSTATS.TraitVal traitVal in (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS)
                {
                    if ((string)((Resource)dup.Stats.stressTrait).Id == (string)traitVal.id)
                    {
                        PrintingEventsHook.Sets[dup].Stress = traitVal;
                        break;
                    }
                }
                foreach (DUPLICANTSTATS.TraitVal traitVal in (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS)
                {
                    if ((string)((Resource)dup.Stats.joyTrait).Id == (string)traitVal.id)
                    {
                        PrintingEventsHook.Sets[dup].Joy = traitVal;
                        break;
                    }
                }
            }
            else if (panel != null)
            {
                switch (changeType)
                {
                    case PrintingEventsHook.ChangeType.PositiveTrait:
                        PrintingEventsHook.PickNextTrait(prevTrait, PrintingEventsHook.Sets[dup].Positive, (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.GOODTRAITS, CharacterHelper.GetNegativeVitals(dup));
                        break;
                    case PrintingEventsHook.ChangeType.NegativeTrait:
                        PrintingEventsHook.PickNextTrait(prevTrait, PrintingEventsHook.Sets[dup].Negative, (List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.BADTRAITS, CharacterHelper.GetPositiveVitals(dup));
                        break;
                    case PrintingEventsHook.ChangeType.StressReaction:
                        int num1 = ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS).Count - 1;
                        for (int index = 0; index < ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS).Count; ++index)
                        {
                            if ((string)((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS)[index].id == (string)((Resource)prevTrait).Id)
                            {
                                num1 = index;
                                break;
                            }
                        }
                        int index1 = (num1 + 1) % ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS).Count;
                        PrintingEventsHook.Sets[dup].Stress = ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS)[index1];
                        break;
                    case PrintingEventsHook.ChangeType.Attribute:
                        SkillGroup oldAptitude = (SkillGroup)null;
                        float num2 = 0.0f;
                        int num3 = 0;
                        List<HashedString> existing = new List<HashedString>();
                        foreach (KeyValuePair<SkillGroup, float> keyValuePair in new Dictionary<SkillGroup, float>((IDictionary<SkillGroup, float>)dup.Stats.skillAptitudes))
                        {
                            if (num3 == attributeIndex)
                            {
                                ((Dictionary<SkillGroup, float>)dup.Stats.skillAptitudes).Remove(keyValuePair.Key);
                                oldAptitude = keyValuePair.Key;
                                num2 = keyValuePair.Value;
                            }
                            else
                                existing.Add((HashedString)((Resource)keyValuePair.Key).IdHash);
                            ++num3;
                        }
                        List<SkillGroup> list = ((IEnumerable<SkillGroup>)((IEnumerable<SkillGroup>)new List<SkillGroup>((IEnumerable<SkillGroup>)((ResourceSet<SkillGroup>)Db.Get().SkillGroups).resources)).ToList<SkillGroup>()).Where<SkillGroup>((Func<SkillGroup, bool>)(u => !existing.Contains((HashedString)((Resource)u).IdHash))).ToList<SkillGroup>();
                        for (int index2 = 0; index2 < list.Count; ++index2)
                        {
                            if (list[index2].IdHash == oldAptitude.IdHash)
                            {
                                int index3 = (index2 + 1) % list.Count;
                                ((Dictionary<SkillGroup, float>)dup.Stats.skillAptitudes).Add(list[index3], num2);
                                int num4 = 0;
                                foreach (Attribute relevantAttribute1 in (List<Attribute>)list[index2].relevantAttributes)
                                {
                                    int num5 = 1;
                                    foreach (KeyValuePair<SkillGroup, float> keyValuePair in ((IEnumerable<KeyValuePair<SkillGroup, float>>)dup.Stats.skillAptitudes).Where<KeyValuePair<SkillGroup, float>>((Func<KeyValuePair<SkillGroup, float>, bool>)(u => u.Key.IdHash!=oldAptitude.IdHash)))
                                    {
                                        foreach (Attribute relevantAttribute2 in (List<Attribute>)keyValuePair.Key.relevantAttributes)
                                        {
                                            if (relevantAttribute1.IdHash == relevantAttribute2.IdHash)
                                                ++num5;
                                        }
                                    }
                                    num4 = ((Dictionary<string, int>)dup.Stats.StartingLevels)[(string)((Resource)relevantAttribute1).Id];
                                    ((Dictionary<string, int>)dup.Stats.StartingLevels)[(string)((Resource)relevantAttribute1).Id] = num5 > 1 ? num4 : 0;
                                }
                                foreach (Attribute relevantAttribute in (List<Attribute>)list[index3].relevantAttributes)
                                    ((Dictionary<string, int>)dup.Stats.StartingLevels)[(string)((Resource)relevantAttribute).Id] = num4;
                            }
                        }
                        break;
                    case PrintingEventsHook.ChangeType.JoyTrait:
                        int num6 = ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS).Count - 1;
                        for (int index2 = 0; index2 < ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS).Count; ++index2)
                        {
                            if ((string)((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS)[index2].id == (string)((Resource)prevTrait).Id)
                            {
                                num6 = index2;
                                break;
                            }
                        }
                        int index4 = (num6 + 1) % ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS).Count;
                        PrintingEventsHook.Sets[dup].Joy = ((List<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.JOYTRAITS)[index4];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(changeType), (object)changeType, (string)null);
                }
            }
            try
            {
                if (panel == null)
                {
                    flag = true;
                    panel = (CollapsibleDetailContentPanel)Util.KInstantiateUI<CollapsibleDetailContentPanel>((GameObject)ScreenPrefabs.Instance.CollapsableContentPanel, ((Component)((Component)dup).gameObject.transform.parent).gameObject, false);
                    PrintingEventsHook._itemsToDestroy.Add(panel);
                    panel.HeaderLabel.text = Languages.TITLE; //Languages.TITLE.NAME;
                    (((KMonoBehaviour)panel).transform as RectTransform).sizeDelta = Util.rectTransform((Component)((KMonoBehaviour)dup).transform).sizeDelta;
                    Transform transform = ((Component)((Component)panel).GetComponent<VerticalLayoutGroup>()).gameObject.transform;
                    LayoutElement component = ((Component)panel).GetComponent<LayoutElement>();
                    Rect rect = Util.rectTransform((Component)((KMonoBehaviour)panel).transform).rect;
                    double width = (double)rect.width;
                    ((LayoutElement)component).minWidth = (float)width;
                    ((LayoutGroup)((Component)transform.GetChild(1)).GetComponent<HorizontalLayoutGroup>()).padding = new RectOffset(10, 0, 8, 0);
                    panel.scalerMask.hoverLock = true;
                    if (PrintingEventsHook._fontSize == 0)
                        PrintingEventsHook._fontSize = (int)((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize;
                    panel.labelTemplate.label.textStyleSetting.fontSize = 16;
                    panel.SetCollapsible(false);
                    Color color1;
                    ColorUtility.TryParseHtmlString("#252B3D", out color1);
                    Color color2;
                    ColorUtility.TryParseHtmlString("#A9517D", out color2);
                    if (PrintingEventsHook._style == null)
                        PrintingEventsHook._style = (TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting;
                    if (!PrintingEventsHook._defaultcolor.HasValue)
                        PrintingEventsHook._defaultcolor = new Color?((Color)((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor);
                    // ISSUE: cast to a reference type
                    // ISSUE: explicit reference operation
                    panel.SetColors(new CollapsibleDetailContentPanel.PanelColors()
                    {
                        ArrowColor = panel.colors.ArrowColor,
                        FrameColor = color1,
                        FrameColor_Hover = color1,
                        FrameColor_Press = color1,
                        TextColor = color1
                    });
                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = color1;
                    ((Component)dup).gameObject.SetActive(false);
                    ((Component)panel).gameObject.transform.SetSiblingIndex(((Component)dup).gameObject.transform.GetSiblingIndex());
                    ((Component)panel).gameObject.SetActive(true);
                    ((HorizontalOrVerticalLayoutGroup)((Component)panel).GetComponent<VerticalLayoutGroup>()).spacing = 13f;
                    if (!PrintingEventsHook._oldColor.HasValue)
                        PrintingEventsHook._oldColor = new Color?((Color)((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor);
                    if (!PrintingEventsHook._headerColor.HasValue)
                        PrintingEventsHook._headerColor = new Color?(color2);
                    if (PrintingEventsHook._oldFont == null)
                        PrintingEventsHook._oldFont = (TMP_FontAsset)((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont;
                    if (PrintingEventsHook._headerFont == null)
                        PrintingEventsHook._headerFont = (TMP_FontAsset)((TextStyleSetting)((LocText)panel.HeaderLabel).textStyleSetting).sdfFont;
                    ((LayoutGroup)Util.FindOrAddComponent<VerticalLayoutGroup>((Component)panel.Content)).childAlignment = (TextAnchor)0;
                    ((LayoutElement)Util.FindOrAddUnityComponent<LayoutElement>((Component)panel)).minHeight = (float)CharacterHelper.GetContainerHeight(dup);
                    ((LayoutElement)Util.FindOrAddComponent<LayoutElement>((Component)panel.Content)).preferredHeight = (float)(CharacterHelper.GetContainerHeight(dup) - 100);
                }

                ((List<Trait>)dup.Stats.Traits).Clear();
                ((List<Trait>)dup.Stats.Traits).Add(CharacterHelper.VitalToTrait((string)MinionConfig.MINION_BASE_TRAIT_ID));
                foreach (DUPLICANTSTATS.TraitVal vital in PrintingEventsHook.Sets[dup].Positive)
                    ((List<Trait>)dup.Stats.Traits).Add(CharacterHelper.VitalToTrait(vital));
                foreach (DUPLICANTSTATS.TraitVal vital in PrintingEventsHook.Sets[dup].Negative)
                    ((List<Trait>)dup.Stats.Traits).Add(CharacterHelper.VitalToTrait(vital));
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._headerFont;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._headerColor.Value;
                panel.SetLabel("attributesTrait", Languages.ATTRIBUTES_NAME /*Languages.ATTRIBUTES.NAME*/, Languages.ATTRIBUTES_DESCRIPTION /* Languages.ATTRIBUTES.DESCRIPTION */);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._oldFont;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._oldColor.Value;
                int num4 = 0;
                foreach (KeyValuePair<SkillGroup, float> skillAptitude in (Dictionary<SkillGroup, float>)dup.Stats.skillAptitudes)
                {
                    KeyValuePair<SkillGroup, float> skill = skillAptitude;
                    int index = num4;
                    panel.SetLabelWithButton(string.Format("attributeButton{0}", (object)num4), "  <b>" + (string)((Resource)skill.Key).Name + "</b>", "", Languages.CHANGE_BUTTON_NAME, Languages.CHANGE_BUTTON_DESCRIPTION, (System.Action)(() =>
                    {
                        Logger.Print(string.Format("Click {0} attributeButton{1}", (object)((Resource)skill.Key).Name, (object)index));
                        PrintingEventsHook.OnClick(dup, (GameObject)null, panel, changeType: PrintingEventsHook.ChangeType.Attribute, attributeIndex: index);
                    }));
                    ++num4;
                }
                int fontSize = (int)((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize;
//                            if (!StartUp<Engine, Settings, TranslationMod>.Settings.NoPositiveTraits)
//                            {
                                for (int index2 = 0; index2 < PrintingEventsHook.Sets[dup].Positive.Count; ++index2)
                                {
                                    if (index2 == 0)
                                    {
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._headerFont;
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._headerColor.Value;
                                        if (PrintingEventsHook.Sets[dup].Positive.Count > 1)
                                            panel.SetLabel("positiveTrait", Languages.POSITIVE_TRAITS_NAME, Languages.POSITIVE_TRAITS_DESCRIPTION);
                                        else
                                            panel.SetLabel("positiveTrait", Languages.POSITIVE_TRAIT_NAME, Languages.POSITIVE_TRAIT_DESCRIPTION);
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._oldFont;
                                    }
                                    Trait positive = CharacterHelper.VitalToTrait(PrintingEventsHook.Sets[dup].Positive[index2]);
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = Constants.POSITIVE_COLOR;
                                    panel.SetLabelWithButton(string.Format("positiveTraitButton{0}", (object)index2), "<b>" + (string)((Resource)positive).Name + "</b>", positive.description, Languages.CHANGE_BUTTON_NAME, Languages.CHANGE_BUTTON_DESCRIPTION, (System.Action)(() => PrintingEventsHook.OnClick(dup, (GameObject)null, panel, positive)));
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._oldColor.Value;
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = 14;
                                    PrintingEventsHook.GenerateDescriptions(panel, positive, string.Format("positive{0}", (object)index2));
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = fontSize;
                                }
//                            } 
  //                          if (!StartUp<Engine, Settings, TranslationMod>.Settings.NoNegativeTraits)
//                            {
                                for (int index2 = 0; index2 < PrintingEventsHook.Sets[dup].Negative.Count; ++index2)
                                {
                                    if (index2 == 0)
                                    {
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._headerFont;
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._headerColor.Value;
                                        if (PrintingEventsHook.Sets[dup].Negative.Count > 1)
                                            panel.SetLabel("negativeTrait", Languages.NEGATIVE_TRAITS_NAME, Languages.NEGATIVE_TRAITS_DESCRIPTION);
                                        else
                                            panel.SetLabel("negativeTrait", Languages.NEGATIVE_TRAIT_NAME, Languages.NEGATIVE_TRAIT_DESCRIPTION);
                                        ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._oldFont;
                                    }
                                    Trait negative = CharacterHelper.VitalToTrait(PrintingEventsHook.Sets[dup].Negative[index2]);
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = Constants.NEGATIVE_COLOR;
                                    panel.SetLabelWithButton(string.Format("negativeTraitButton{0}", (object)index2), "<b>" + (string)((Resource)negative).Name + "</b>", negative.description, Languages.CHANGE_BUTTON_NAME, Languages.CHANGE_BUTTON_DESCRIPTION, (System.Action)(() => PrintingEventsHook.OnClick(dup, (GameObject)null, panel, negative, PrintingEventsHook.ChangeType.NegativeTrait)));
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._oldColor.Value;
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = 14;
                                    PrintingEventsHook.GenerateDescriptions(panel, negative, string.Format("negativeTrait{0}", (object)index2));
                                    ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = fontSize;
                                }
//                            } 
                Trait stress = CharacterHelper.VitalToTrait(PrintingEventsHook.Sets[dup].Stress);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._headerFont;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._headerColor.Value;
                panel.SetLabel("stressTrait", Languages.STRESS_REACTION_NAME, Languages.STRESS_REACTION_DESCRIPTION);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._oldFont;
                Color color3;
                ColorUtility.TryParseHtmlString("#7A7018", out color3);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = color3;
                panel.SetLabelWithButton("stressTraitButton", "<b>" + (string)((Resource)stress).Name + "</b>", "", Languages.CHANGE_BUTTON_NAME, Languages.CHANGE_BUTTON_DESCRIPTION, (System.Action)(() => PrintingEventsHook.OnClick(dup, (GameObject)null, panel, stress, PrintingEventsHook.ChangeType.StressReaction)));
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._oldColor.Value;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = 13;
                panel.SetLabel("stressTraitDesr", "<i>" + stress.description.Replace("<style=\"KKeyword\">", "<b>").Replace("</style>", "</b>") + "</i>\n\n", "");
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = fontSize;
                dup.Stats.stressTrait = stress;
                Trait joyTrait = CharacterHelper.VitalToTrait(PrintingEventsHook.Sets[dup].Joy);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._headerFont;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._headerColor.Value;
                panel.SetLabel("joyTrait", Languages.JOY_TRAIT_NAME, Languages.JOY_TRAIT_DESCRIPTION);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).sdfFont = PrintingEventsHook._oldFont;
                Color color4;
                ColorUtility.TryParseHtmlString("#1498E9", out color4);
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = color4;
                panel.SetLabelWithButton("joyTraitButton", "<b>" + (string)((Resource)joyTrait).Name + "</b>", "", Languages.CHANGE_BUTTON_NAME, Languages.CHANGE_BUTTON_DESCRIPTION, (System.Action)(() => PrintingEventsHook.OnClick(dup, (GameObject)null, panel, joyTrait, PrintingEventsHook.ChangeType.JoyTrait)));
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._oldColor.Value;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = 13;
                panel.SetLabel("joyTraitDesr", "<i>" + joyTrait.description.Replace("<style=\"KKeyword\">", "<b>").Replace("</style>", "</b>") + "</i>\n\n", "");
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = fontSize;
                dup.Stats.joyTrait = joyTrait;
                if (flag)
                {
                    KButton m0 = Util.KInstantiateUI<KButton>(((Component)((IEnumerable<KButton>)Resources.FindObjectsOfTypeAll<KButton>()).First<KButton>((Func<KButton, bool>)(a => a.name == "LoadButton"))).gameObject, ((Component)((Component)panel).GetComponent<VerticalLayoutGroup>()).gameObject, false);
                    m0.onClick += (System.Action)(() => PrintingEventsHook.Apply(dup, panel));
                    m0.gameObject.SetActive(true);
                    m0.gameObject.AddComponent<HorizontalLayoutGroup>();
                    ((LayoutElement)Util.FindOrAddComponent<LayoutElement>((Component)m0)).minHeight = 40f;
                    ((LayoutElement)Util.FindOrAddComponent<LayoutElement>((Component)m0)).preferredHeight = 40f;
                    m0.name = "Apply";
                    ((TMP_Text)((Component)((KMonoBehaviour)m0).transform.GetChild(0)).GetComponent<LocText>()).text = Languages.APPLY_BUTTON_NAME;
                    ((TextStyleSetting)((LocText)((Component)((KMonoBehaviour)m0).transform.GetChild(0)).GetComponent<LocText>()).textStyleSetting).fontSize = 18;
                }
                ((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting = PrintingEventsHook._style;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).textColor = PrintingEventsHook._defaultcolor.Value;
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = PrintingEventsHook._fontSize;
            }
            catch (Exception ex)
            {
                Logger.Print(ex);
            }

        }
        private static void GenerateDescriptions(
          CollapsibleDetailContentPanel panel,
          Trait trait,
          string prefix)
        {
            panel.SetLabel(prefix + "TraitDesrmodifier0", "", "");
            panel.SetLabel(prefix + "TraitDesrmodifier1", "", "");
            panel.SetLabel(prefix + "TraitDesrmodifier2", "", "");
            panel.SetLabel(prefix + "disabledChoreGroups", "", "");
            panel.SetLabel(prefix + "ignoredEffects", "", "");
            panel.SetLabel(prefix + "desrc", "", "");
            for (int index = 0; index < ((Component)panel.Content).transform.childCount; ++index)
            {
                Transform child = ((Component)panel.Content).transform.GetChild(index);
                if (child.name.StartsWith(prefix ?? ""))
                {
                    ((TMP_Text)((Component)child).GetComponent<LocText>()).text = "";
                    ((LayoutElement)Util.FindOrAddComponent<LayoutElement>((Component)((Component)child).transform)).preferredHeight = 20f;
                    ((Component)child).gameObject.SetActive(false);
                }
            }
            if (trait.SelfModifiers != null)
            {
                for (int index1 = 0; index1 < trait.SelfModifiers.Count; ++index1)
                {
                    string str1 = string.Format((string)((double)trait.SelfModifiers[index1].Value > 0.0 ? STRINGS.UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED : STRINGS.UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED), (object)Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[index1].AttributeId.ToUpper() + ".NAME"));
                    Klei.AI.Attribute attrib = Db.Get().Attributes.Get(trait.SelfModifiers[index1].AttributeId);
                    string str2 = str1;
                    string tooltip = string.Format("{0}\n\n{1}: {2}", (object)str2, (object)Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[index1].AttributeId.ToUpper() + ".NAME"), (object)trait.SelfModifiers[index1].GetFormattedString());
                    List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(attrib);
                    for (int index2 = 0; index2 < convertersForAttribute.Count; ++index2)
                    {
                        string str3 = convertersForAttribute[index2].DescriptionFromAttribute(convertersForAttribute[index2].multiplier * trait.SelfModifiers[index1].Value, (GameObject)null);
                        if (str3 != string.Empty)
                            tooltip = tooltip + "\n    • " + str3;
                    }
                    panel.SetLabel(string.Format("{0}TraitDesrmodifier{1}", (object)prefix, (object)index1), "   " + str2, tooltip);
                }
            }
            if (trait.disabledChoreGroups != null)
            {
                string disabledChoresString = trait.GetDisabledChoresString(false);
                string empty1 = string.Empty;
                string empty2 = string.Empty;
                for (int index = 0; index < trait.disabledChoreGroups.Length; ++index)
                {
                    if (index > 0)
                    {
                        empty1 += ", ";
                        empty2 += "\n";
                    }
                    empty1 += (string)((Resource)trait.disabledChoreGroups[index]).Name;
                    empty2 += (string)((ChoreGroup)trait.disabledChoreGroups[index]).description;
                }
                string str = string.Format((LocString)((LocString)DUPLICANTS.TRAITS.CANNOT_DO_TASK_TOOLTIP), (object)empty1, (object)empty2);
                panel.SetLabel(prefix + "disabledChoreGroups", "   " + disabledChoresString + "\n\n", str);
            }
            if (trait.ignoredEffects != null && trait.ignoredEffects.Length != 0)
            {
                string ignoredEffectsString = trait.GetIgnoredEffectsString(false);
                string empty1 = string.Empty;
                string empty2 = string.Empty;
                for (int index = 0; index < trait.ignoredEffects.Length; ++index)
                {
                    if (index > 0)
                    {
                        empty1 += ", ";
                        empty2 += "\n";
                    }
                    empty1 += (StringEntry)(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + ((string)trait.ignoredEffects[index]).ToUpper() + ".NAME"));
                    empty2 += (StringEntry)(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + ((string)trait.ignoredEffects[index]).ToUpper() + ".CAUSE"));
                }
                string str = string.Format((LocString)((LocString)DUPLICANTS.TRAITS.IGNORED_EFFECTS_TOOLTIP), (object)empty1, (object)empty2);
                panel.SetLabel(prefix + "ignoredEffects", "   " + ignoredEffectsString, str);
            }
            StringEntry stringEntry1;
            if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS." + ((string)((Resource)trait).Id).ToUpper() + ".SHORT_DESC", out stringEntry1))
            {
                string str = (string)stringEntry1.String;
                StringEntry stringEntry2 = Strings.Get("STRINGS.DUPLICANTS.TRAITS." + ((string)((Resource)trait).Id).ToUpper() + ".SHORT_DESC_TOOLTIP");
                panel.SetLabel(prefix + "desrc", "   " + str, (StringEntry)(stringEntry2));
            }
            for (int index = 0; index < ((Component)panel.Content).transform.childCount; ++index)
            {
                Transform child = ((Component)panel.Content).transform.GetChild(index);
                if (child.name.StartsWith(prefix ?? "") && !string.IsNullOrEmpty(((TMP_Text)((Component)child).GetComponent<LocText>()).text))
                    child.gameObject.SetActive(true);
            }
        }

        private static void Apply(CharacterContainer dup, CollapsibleDetailContentPanel panel)
        {
            try
            {
                ((TextStyleSetting)((LocText)((DetailLabel)panel.labelTemplate).label).textStyleSetting).fontSize = 12;
                ((TextStyleSetting)((LocText)panel.HeaderLabel).textStyleSetting).fontSize = 14;
                AccessTools.Method(((object)dup).GetType(), "SetInfoText", (Type[])null, (Type[])null).Invoke((object)dup, new object[0]);
            }
            catch (Exception ex)
            {
                Logger.Print(ex);
            }
          ((Component)dup).gameObject.transform.SetSiblingIndex(((Component)panel).gameObject.transform.GetSiblingIndex());
            ((Component)dup).gameObject.SetActive(true);
            ((Component)panel).gameObject.SetActive(false);
            UnityEngine.Object.Destroy(panel);
        }

        internal class TraitsSet
        {
            public List<DUPLICANTSTATS.TraitVal> Positive { get; set; } = new List<DUPLICANTSTATS.TraitVal>();

            public List<DUPLICANTSTATS.TraitVal> Negative { get; set; } = new List<DUPLICANTSTATS.TraitVal>();

            public DUPLICANTSTATS.TraitVal Stress { get; set; }

            public DUPLICANTSTATS.TraitVal Joy { get; set; }
        }

        public enum ChangeType
        {
            PositiveTrait,
            NegativeTrait,
            StressReaction,
            Attribute,
            JoyTrait,
        }

    }
}