using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BottleUserEmptier : StateMachineComponent<BottleUserEmptier.StatesInstance>, IGameObjectEffectDescriptor, IUserControlledCapacity
{
    public float emptyRate = 10f;
    [Serialize]
    private float storeLimit = 20000.0f;
    [Serialize]
    public bool allowManualPumpingStationFetching;
    public bool isGasEmptier;
    private static readonly EventSystem.IntraObjectHandler<BottleUserEmptier> OnRefreshUserMenuDelegate =
          new EventSystem.IntraObjectHandler<BottleUserEmptier>((System.Action<BottleUserEmptier, object>)((component, data) => component.OnRefreshUserMenu(data)));
    private static readonly EventSystem.IntraObjectHandler<BottleUserEmptier> OnCopySettingsDelegate =
          new EventSystem.IntraObjectHandler<BottleUserEmptier>((System.Action<BottleUserEmptier, object>)((component, data) => component.OnCopySettings(data)));

    float IUserControlledCapacity.UserMaxCapacity
    {
        get => (float)this.storeLimit;
        set
        {
            this.storeLimit = value;
            this.GetComponent<Storage>().capacityKg = value / 1000;
            this.smi.RefreshChore();
            //          Debug.Log("IUserControlledCapacity.UserMaxCapacity");
            //            this.GetComponent<TreeFilterable>().OnFilterChanged();
        }
    }

    float IUserControlledCapacity.AmountStored => (float)this.storeLimit;

    float IUserControlledCapacity.MinCapacity => 0.0f;

    float IUserControlledCapacity.MaxCapacity => 20000.0f;

    bool IUserControlledCapacity.WholeValues => false;

    LocString IUserControlledCapacity.CapacityUnits => GameUtil.GetCurrentMassUnit(useSmallUnit: true);

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.smi.StartSM();
        this.Subscribe<BottleUserEmptier>((int)GameHashes.RefreshUserMenu, BottleUserEmptier.OnRefreshUserMenuDelegate);
        this.Subscribe<BottleUserEmptier>((int)GameHashes.CopySettings, BottleUserEmptier.OnCopySettingsDelegate);

    }

    public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>)null;

    private void OnChangeAllowManualPumpingStationFetching()
    {
        this.allowManualPumpingStationFetching = !this.allowManualPumpingStationFetching;
        this.smi.RefreshChore();
    }

    private void OnRefreshUserMenu(object data)
    {
        Game.Instance.userMenu.AddButton(this.gameObject, this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP)), 0.4f);
        //        Debug.Log("OnRefreshUserMenu");
    }

    private void OnCopySettings(object data)
    {
        this.allowManualPumpingStationFetching = ((GameObject)data).GetComponent<BottleUserEmptier>().allowManualPumpingStationFetching;
        this.smi.RefreshChore();
    }



    public class StatesInstance : GameStateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.GameInstance
    {
        private FetchChore chore;

        public MeterController meter { get; private set; }

        public StatesInstance(BottleUserEmptier smi)
          : base(smi)
        {
            this.master.GetComponent<TreeFilterable>().OnFilterChanged += new System.Action<Tag[]>(this.OnFilterChanged);
            this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
            {
        "meter_target",
        "meter_arrow",
        "meter_scale"
            });
            this.Subscribe((int)GameHashes.OnStorageChange, new System.Action<object>(this.OnStorageChange));
            this.Subscribe((int)GameHashes.OnlyFetchMarkedItemsSettingChanged, new System.Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
        }

        public void CreateChore()
        {
            this.GetComponent<KBatchedAnimController>();
            Tag[] tags = this.GetComponent<TreeFilterable>().GetTags();
            Tag[] forbidden_tags;
            if (!this.master.allowManualPumpingStationFetching)
                forbidden_tags = new Tag[1] { GameTags.LiquidSource };
            else
                forbidden_tags = new Tag[0];
            Storage component = this.GetComponent<Storage>();
            if (this.GetComponent<BottleUserEmptier>().storeLimit > 0)
            {
                this.chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component, this.GetComponent<BottleUserEmptier>().storeLimit / 1000.0f, tags, forbidden_tags: forbidden_tags);
                //                Debug.Log("+" + this.GetComponent<BottleUserEmptier>().storeLimit);

            }
            else { CancelChore(); }
        }

        public void CancelChore()
        {
            if (this.chore == null)
                return;
            this.chore.Cancel("Storage Changed");
            this.chore = (FetchChore)null;
        }

        public void RefreshChore() => this.GoTo((StateMachine.BaseState)this.sm.unoperational);

        private void OnFilterChanged(Tag[] tags) => this.RefreshChore();


        private void OnStorageChange(object data)
        {
            Storage component = this.GetComponent<Storage>();
            this.meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
            //            Debug.Log("+-" + component.RemainingCapacity() + "/" + component.capacityKg);
        }

        private void OnOnlyFetchMarkedItemsSettingChanged(object data) => this.RefreshChore();

        public void StartMeter()
        {
            PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
            if ((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null)
                return;
            this.meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
            this.meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
            this.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), (Color)firstPrimaryElement.Element.substance.colour);
        }

        private PrimaryElement GetFirstPrimaryElement()
        {
            Storage component1 = this.GetComponent<Storage>();
            for (int idx = 0; idx < component1.Count; ++idx)
            {
                GameObject gameObject = component1[idx];
                if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
                {
                    PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
                    if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
                        return component2;
                }
            }
            return (PrimaryElement)null;
        }

        public void Emit(float dt)
        {
#if (SPACEDOUT)
            PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
            if ((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null)
                return;
            Storage component = this.GetComponent<Storage>();
            float amount = Mathf.Min(firstPrimaryElement.Mass, this.master.emptyRate * dt);
            if ((double)amount <= 0.0)
                return;
            Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
            float amount_consumed;
            SimUtil.DiseaseInfo disease_info;
            float aggregate_temperature;
            component.ConsumeAndGetDisease(prefabTag, amount, out amount_consumed, out disease_info, out aggregate_temperature);
            Vector3 position = this.transform.GetPosition();
            position.y += 1.8f;
            bool flag = this.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
            position.x += flag ? -0.2f : 0.2f;
            int num = Grid.PosToCell(position) + (flag ? -1 : 1);
            if (Grid.Solid[num])
                num += flag ? 1 : -1;
            Element element = firstPrimaryElement.Element;
            byte idx = element.idx;
            if (element.IsLiquid)
                FallingWater.instance.AddParticle(num, idx, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count, true);
            else
                SimMessages.ModifyCell(num, (int)idx, aggregate_temperature, amount_consumed, disease_info.idx, disease_info.count);
#endif
#if (VANILLA)
            PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
            if ((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null)
                return;
            Storage component = this.GetComponent<Storage>();
            float amount = Mathf.Min(firstPrimaryElement.Mass, this.master.emptyRate * dt);
            if ((double)amount <= 0.0)
                return;
            Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
            SimUtil.DiseaseInfo disease_info;
            float aggregate_temperature;
            component.ConsumeAndGetDisease(prefabTag, amount, out disease_info, out aggregate_temperature);
            Vector3 position = this.transform.GetPosition();
            position.y += 1.8f;
            bool flag = this.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
            position.x += flag ? -0.2f : 0.2f;
            int num2 = Grid.PosToCell(position) + (flag ? -1 : 1);
            if (Grid.Solid[num2])
                num2 += flag ? 1 : -1;
            Element element = firstPrimaryElement.Element;
            byte idx = element.idx;
            if (element.IsLiquid)
                FallingWater.instance.AddParticle(num2, idx, amount, aggregate_temperature, disease_info.idx, disease_info.count, true);
            else
                SimMessages.ModifyCell(num2, (int)idx, aggregate_temperature, amount, disease_info.idx, disease_info.count);
        
#endif
            this.GetComponent<BottleUserEmptier>().storeLimit -= amount * 1000;
            if (Mathf.Abs(this.GetComponent<BottleUserEmptier>().storeLimit) < 0.1f) { this.GetComponent<BottleUserEmptier>().storeLimit = 0.0f; }
            //          Debug.Log("-" + (amount * 1000)+"="+ this.GetComponent<BottleUserEmptier>().storeLimit);
        }
    }

    public class States : GameStateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier>
    {
        private StatusItem statusItem;
        public GameStateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State unoperational;
        public GameStateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State waitingfordelivery;
        public GameStateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State emptying;

        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = (StateMachine.BaseState)this.waitingfordelivery;
            this.statusItem = new StatusItem(nameof(BottleUserEmptier), "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            this.statusItem.resolveStringCallback = (Func<string, object, string>)((str, data) =>
           {
               BottleUserEmptier BottleUserEmptier = (BottleUserEmptier)data;
               if ((UnityEngine.Object)BottleUserEmptier == (UnityEngine.Object)null)
                   return str;
               return BottleUserEmptier.allowManualPumpingStationFetching ? (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME : (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
           });
            this.statusItem.resolveTooltipCallback = (Func<string, object, string>)((str, data) =>
           {
               BottleUserEmptier BottleUserEmptier = (BottleUserEmptier)data;
               if ((UnityEngine.Object)BottleUserEmptier == (UnityEngine.Object)null)
                   return str;
               return BottleUserEmptier.allowManualPumpingStationFetching ? (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP : (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
           });
            this.root.ToggleStatusItem(this.statusItem, (Func<BottleUserEmptier.StatesInstance, object>)(smi => (object)smi.master));
            this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery).PlayAnim("off");
            this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.emptying, (StateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.Transition.ConditionCallback)(smi => !smi.GetComponent<Storage>().IsEmpty())).Enter("CreateChore", (StateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State.Callback)(smi => smi.CreateChore())).Exit("CancelChore", (StateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State.Callback)(smi => smi.CancelChore())).PlayAnim("on");
            this.emptying.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (StateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.Transition.ConditionCallback)(smi => smi.GetComponent<Storage>().IsEmpty())).Enter("StartMeter", (StateMachine<BottleUserEmptier.States, BottleUserEmptier.StatesInstance, BottleUserEmptier, object>.State.Callback)(smi => smi.StartMeter())).Update("Emit", (System.Action<BottleUserEmptier.StatesInstance, float>)((smi, dt) => smi.Emit(dt))).PlayAnim("working_loop", KAnim.PlayMode.Loop);
        }
    }
}
