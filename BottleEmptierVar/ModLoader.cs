//using HarmonyLib;
using Harmony;
using TUNING;
using STRINGS;
using System.Collections.Generic;

namespace BottleEmptierVar
{
    public class BottleEmptierVarPatches
    {
        public static class Mod_OnLoad
        {
            public static void OnLoad()
            {
                //				LogInit();
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                //				AddBuildingStrings(BottleEmptier30g.Id, BottleEmptier30g.DisplayName, BottleEmptier30g.Description, BottleEmptier30g.Effect);
                //				AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Plumbing, BottleEmptier30g.Id, BottleEmptierConfig.ID);
                //				AddBuildingStrings(BottleEmptier1kg.Id, BottleEmptier1kg.DisplayName, BottleEmptier1kg.Description, BottleEmptier1kg.Effect);
                //				AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Plumbing, BottleEmptier1kg.Id, BottleEmptierConfig.ID );
                AddBuildingStrings(BottleEmptierVar.Id, BottleEmptierVar.DisplayName, BottleEmptierVar.Description, BottleEmptierVar.Effect);
                AddBuildingToPlanScreen("Plumbing", BottleEmptierVar.Id, BottleEmptierConfig.ID);
            }
        }
        /*
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, BottleEmpiterMeterConfig.Id);
			}
		}
		*/
        public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
        {
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
        }

        public static void AddBuildingToPlanScreen(HashedString category, string buildingId, string addAfterBuildingId = null)
        {
            var index = TUNING.BUILDINGS.PLANORDER.FindIndex(x => x.category == category);

            if (index == -1)
                return;

            if (!(TUNING.BUILDINGS.PLANORDER[index].data is IList<string> planOrderList))
            {
                Debug.Log($"Could not add {buildingId} to the building menu.");
                return;
            }

            var neighborIdx = planOrderList.IndexOf(addAfterBuildingId);

            if (neighborIdx != -1)
                planOrderList.Insert(neighborIdx + 1, buildingId);
            else
                planOrderList.Add(buildingId);
        }




    }
}
