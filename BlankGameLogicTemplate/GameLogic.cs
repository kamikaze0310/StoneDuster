using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using System;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace StoneDuster
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Drill), false, "SmallBlockDrill", "LargeBlockDrill")]
    public class GameLogic : MyGameLogicComponent
    {
        private static bool _initControls;
        public static Guid cpmID = new Guid("B9916634-2230-41E3-8E77-25C454F5B1D6");
        public bool enableStoneDuster;
        public bool init;
        public IMyShipDrill drill;
        public bool isServer;
        public SerializableDefinitionId id;
        public IMyGunObject<MyToolBase> GunBase;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);

            NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (!init) Init();
            if (!_initControls) SetupControls();

            if (isServer) Run();
        }

        private void Init()
        {
            init = true;
            isServer = MyAPIGateway.Multiplayer.IsServer;
            drill = (IMyShipDrill)Entity;
            GunBase = (IMyGunObject<MyToolBase>)drill;
            id = new SerializableDefinitionId(typeof(MyObjectBuilder_Ore), "Stone");
            LoadBlockSettings();
        }

        private void SetupControls()
        {
            _initControls = true;
            MyAPIGateway.TerminalControls.CustomControlGetter += ActionControls.CreateControlsNew;
            //MyAPIGateway.TerminalControls.CustomActionGetter += ActionControls.CreateActionsNew;
            ActionControls.CreateActions(drill);
            MyAPIGateway.Multiplayer.RegisterMessageHandler(4700, Comms.MessageHandler);
        }

        private void Run()
        {
            if (drill == null) return;
            if (!enableStoneDuster || !GunBase.IsShooting) return;
 
            var blockInv = drill.GetInventory(0);
            var item = blockInv.FindItem(id);
            if (item == null) return;

            blockInv.RemoveItemAmount(item, item.Amount);
        }

        private void LoadBlockSettings()
        {
            if (Entity.Storage == null) return;

            var storage = Entity.Storage[cpmID];
            bool result = false;
            bool.TryParse(storage, out result);
            enableStoneDuster = result;
        }

        public override void Close()
        {
            if (Entity == null)
                return;
        }



        public override void OnRemovedFromScene()
        {

            base.OnRemovedFromScene();

            var Block = Entity as IMyShipDrill;

            if (Block == null)
            {
                return;
            }

            try
            {
                //Unregister any handlers here
                MyAPIGateway.TerminalControls.CustomControlGetter -= ActionControls.CreateControlsNew;
                //MyAPIGateway.TerminalControls.CustomActionGetter -= ActionControls.CreateActionsNew;
                MyAPIGateway.Multiplayer.UnregisterMessageHandler(4700, Comms.MessageHandler);
                _initControls = false;
            }
            catch (Exception exc)
            {

            }
        }
    }
}
