using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace BottleEmptierVar
{
    public class BottleEmptierVar : IBuildingConfig
    {
        public const string Id = "BottleEmptierVar";
        public static string DisplayName = Languages.BOTTLEEMPTIERVAR_NAME;
        public static string Description = STRINGS.BUILDINGS.PREFABS.BOTTLEEMPTIER.DESC;
        public static string Effect = STRINGS.BUILDINGS.PREFABS.BOTTLEEMPTIER.EFFECT;

        /*		private static readonly List<Storage.StoredItemModifier> PollutedWaterStorageModifiers = new List<Storage.StoredItemModifier>
				{
					Storage.StoredItemModifier.Hide,
					Storage.StoredItemModifier.Seal
				};
		*/
        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 1,
                height: 3,
                anim: "liquidator_kanim",
                hitpoints: BUILDINGS.HITPOINTS.TIER1,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER1,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
                construction_materials: MATERIALS.RAW_MINERALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.PENALTY.TIER1,
                noise: NOISE_POLLUTION.NONE);

            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;

        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);

            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = 20.0f;
            //			storage.storageFullMargin = 200f;

            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<BottleUserEmptier>();

        }
        public override void DoPostConfigureComplete(GameObject go)
        {

        }
    }
}
