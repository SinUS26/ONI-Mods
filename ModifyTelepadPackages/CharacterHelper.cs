using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using TUNING;
using UnityEngine.UI;
using Database;

namespace ModifyTelepadPackages
{
    class CharacterHelper
    {
        private static Random _rnd = new Random();

        public static Trait VitalToTrait(DUPLICANTSTATS.TraitVal vital) => CharacterHelper.VitalToTrait((string)vital.id);

        public static Trait VitalToTrait(string vitalId) => ((ResourceSet<Trait>)((ModifierSet)Db.Get()).traits).Get(vitalId);

        public static List<Trait> GetPositiveTraits(CharacterContainer dup) => dup.Stats.Traits.Where<Trait>((Func<Trait, bool>)(u => u.PositiveTrait && u.Id != MinionConfig.MINION_BASE_TRAIT_ID)).ToList<Trait>();

        public static List<DUPLICANTSTATS.TraitVal> GetPositiveVitals(
          CharacterContainer dup)
        {
            List<Trait> goodTraits = CharacterHelper.GetPositiveTraits(dup);
            return ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.GOODTRAITS).Where<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => ((IEnumerable<Trait>)goodTraits).Any<Trait>((Func<Trait, bool>)(t => (string)((Resource)t).Id == (string)u.id)))).ToList<DUPLICANTSTATS.TraitVal>();
        }

        public static List<Trait> GetNegativeTraits(CharacterContainer dup) => dup.Stats.Traits.Where<Trait>((Func<Trait, bool>)(u => !u.PositiveTrait && u.Id != MinionConfig.MINION_BASE_TRAIT_ID)).ToList<Trait>();

        public static List<DUPLICANTSTATS.TraitVal> GetNegativeVitals(
          CharacterContainer dup)
        {
            List<Trait> goodTraits = CharacterHelper.GetNegativeTraits(dup);
            return ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.BADTRAITS).Where<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => ((IEnumerable<Trait>)goodTraits).Any<Trait>((Func<Trait, bool>)(t => (string)((Resource)t).Id == (string)u.id)))).ToList<DUPLICANTSTATS.TraitVal>();
        }

        public static Trait RollNewPositiveTrait(CharacterContainer dup)
        {
            List<Trait> positiveTraits = CharacterHelper.GetPositiveTraits(dup);
            List<Trait> negativeTraits = CharacterHelper.GetNegativeTraits(dup);
            List<DUPLICANTSTATS.TraitVal> list = ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.GOODTRAITS).Where<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => ((IEnumerable<Trait>)positiveTraits).All<Trait>((Func<Trait, bool>)(t => (string)((Resource)t).Id != (string)u.id)) && !CharacterHelper.IsMutuallyExclusive(negativeTraits, u))).ToList<DUPLICANTSTATS.TraitVal>();
            int index = CharacterHelper._rnd.Next(0, list.Count);
            return CharacterHelper.VitalToTrait(list[index]);
        }

        public static Trait RollNewNegativeTrait(CharacterContainer dup)
        {
            List<Trait> positiveTraits = CharacterHelper.GetPositiveTraits(dup);
            List<Trait> negativeTraits = CharacterHelper.GetNegativeTraits(dup);
            List<DUPLICANTSTATS.TraitVal> list = ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.BADTRAITS).Where<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => ((IEnumerable<Trait>)negativeTraits).All<Trait>((Func<Trait, bool>)(t => (string)((Resource)t).Id != (string)u.id)) && !CharacterHelper.IsMutuallyExclusive(positiveTraits, u))).ToList<DUPLICANTSTATS.TraitVal>();
            int index = CharacterHelper._rnd.Next(0, list.Count);
            return CharacterHelper.VitalToTrait(list[index]);
        }

        public static bool IsMutuallyExclusive(
          List<DUPLICANTSTATS.TraitVal> excludeList,
          DUPLICANTSTATS.TraitVal trait)
        {
            return excludeList != null && ((IEnumerable<DUPLICANTSTATS.TraitVal>)excludeList).Any<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(e =>
            {
                List<string> mutuallyExclusiveTraits = e.mutuallyExclusiveTraits;
                return mutuallyExclusiveTraits != null && mutuallyExclusiveTraits.Contains((string)trait.id);
            }));
        }

        public static bool IsMutuallyExclusive(List<Trait> excludeList, DUPLICANTSTATS.TraitVal trait) => excludeList != null && ((IEnumerable<Trait>)excludeList).Any<Trait>((Func<Trait, bool>)(e =>
        {
            // ISSUE: variable of the null type
            List<string> mutuallyExclusiveTraits = CharacterHelper.TraitToVital(e).mutuallyExclusiveTraits;
            // ISSUE: explicit non-virtual call
            return mutuallyExclusiveTraits != null && mutuallyExclusiveTraits.Contains((string)trait.id);
        }));

        private static DUPLICANTSTATS.TraitVal TraitToVital(Trait trait)
        {
            DUPLICANTSTATS.TraitVal traitVal = ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.GOODTRAITS).FirstOrDefault<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => (string)u.id == (string)((Resource)trait).Id));
            if (string.IsNullOrEmpty((string)traitVal.id))
                traitVal = ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.BADTRAITS).FirstOrDefault<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => (string)u.id == (string)((Resource)trait).Id));
            if (string.IsNullOrEmpty((string)traitVal.id))
                traitVal = ((IEnumerable<DUPLICANTSTATS.TraitVal>)DUPLICANTSTATS.STRESSTRAITS).FirstOrDefault<DUPLICANTSTATS.TraitVal>((Func<DUPLICANTSTATS.TraitVal, bool>)(u => (string)u.id == (string)((Resource)trait).Id));
            return traitVal;
        }

        public static void SetInfoText(CharacterContainer dup) => AccessTools.Method(((object)dup).GetType(), nameof(SetInfoText), (Type[])null, (Type[])null).Invoke((object)dup, new object[0]);

        public static void SetAnimator(CharacterContainer dup) => AccessTools.Method(((object)dup).GetType(), nameof(SetAnimator), (Type[])null, (Type[])null).Invoke((object)dup, new object[0]);

        public static void UpdateVisuals(CharacterContainer dup)
        {
            CharacterHelper.SetAnimator(dup);
            CharacterHelper.SetInfoText(dup);
        }

        public static int TotalTraitsCount(CharacterContainer characterContainer) => CharacterHelper.GetPositiveTraits(characterContainer).Count + CharacterHelper.GetNegativeTraits(characterContainer).Count;

        public static int GetContainerHeight(CharacterContainer characterContainer)
        {
            switch (CharacterHelper.TotalTraitsCount(characterContainer))
            {
                case 4:
                    return 685;
                case 5:
                    return 725;
                case 6:
                    return 775;
                default:
                    return 640;
            }
        }

        public static int GetBottomHeight(CharacterContainer characterContainer)
        {
            switch (CharacterHelper.TotalTraitsCount(characterContainer))
            {
                case 4:
                    return 240;
                case 5:
                    return 330;
                case 6:
                    return 410;
                default:
                    return 150;
            }
        }

        public static int MaxContainerHeight(List<ITelepadDeliverableContainer> containers)
        {
            int val1 = 640;
            foreach (ITelepadDeliverableContainer container in containers)
            {
                if (container is CharacterContainer characterContainer1 && characterContainer1.Stats != null)
                {
                    int containerHeight = CharacterHelper.GetContainerHeight(characterContainer1);
                    val1 = Math.Max(val1, containerHeight);
                }
            }
            return val1;
        }

        public static void AdjustContainerHeight(ITelepadDeliverableContainer characterContainer)
        {
            CharacterSelectionController selectionController = (CharacterSelectionController)AccessTools.Field(((object)characterContainer).GetType(), "controller").GetValue((object)characterContainer);
            List<ITelepadDeliverableContainer> containers = (List<ITelepadDeliverableContainer>)AccessTools.Field(((object)selectionController).GetType(), "containers").GetValue((object)selectionController);
            if (containers == null)
            {
                Debug.Log("No containers detected!");
            }
            else
            {
                int num = CharacterHelper.MaxContainerHeight(containers);
                LayoutElement orAddComponent = Util.FindOrAddComponent<LayoutElement>(characterContainer.GetGameObject());
                orAddComponent.preferredHeight = (float)num;
                orAddComponent.minHeight = (float)num;
            }
        }

        public static SkillGroup RollNewAptitude(CharacterContainer characterContainer)
        {
            List<SkillGroup> existing = ((IEnumerable<KeyValuePair<SkillGroup, float>>)characterContainer.Stats.skillAptitudes).Select<KeyValuePair<SkillGroup, float>, SkillGroup>((Func<KeyValuePair<SkillGroup, float>, SkillGroup>)(s => s.Key)).ToList<SkillGroup>();
            List<SkillGroup> list = ((IEnumerable<SkillGroup>)new List<SkillGroup>((IEnumerable<SkillGroup>)((ResourceSet<SkillGroup>)Db.Get().SkillGroups).resources)).Where<SkillGroup>((Func<SkillGroup, bool>)(u => !((IEnumerable<SkillGroup>)existing).Any<SkillGroup>((Func<SkillGroup, bool>)(a => (string)((Resource)a).Id == (string)((Resource)u).Id)))).ToList<SkillGroup>();
            Util.Shuffle<SkillGroup>(list);
            return ((IEnumerable<SkillGroup>)list).First<SkillGroup>();
        }
    }
}
