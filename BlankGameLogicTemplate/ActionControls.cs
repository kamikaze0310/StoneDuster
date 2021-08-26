using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System.Text;
using VRage.ModAPI;
using VRage.Utils;

namespace StoneDuster
{
    public static class ActionControls
    {
        public static bool controlsCreated = false;
        public static bool actionCreated = false;

        public static void CreateControlsNew(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {

            if (block as IMyShipDrill != null)
            {
                CreateControls(block, controls);
            }
        }

        public static void CreateActionsNew(IMyTerminalBlock block, List<IMyTerminalAction> controls)
        {

            if (block as IMyShipDrill != null)
            {
                //CreateActions(block, controls);
            }
        }

        public static void CreateControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (block as IMyShipDrill == null || controlsCreated == true)
            {
                return;
            }

            controlsCreated = true;

            // Enable Stone Duster Box
            var enableStoneDuster = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCheckbox, IMyShipDrill>("EnableStoneDuster");
            enableStoneDuster.Enabled = Block => true;
            enableStoneDuster.SupportsMultipleBlocks = true;
            enableStoneDuster.Visible = Block => true;
            enableStoneDuster.Title = MyStringId.GetOrCompute("Enable Stone Duster");
            enableStoneDuster.Tooltip = MyStringId.GetOrCompute("If enabled, this will remove all stone from the drill's inventory when drilling");
            enableStoneDuster.Getter = GetControlStatus;
            enableStoneDuster.Setter = SetControlStatus;
            MyAPIGateway.TerminalControls.AddControl<IMyShipDrill>(enableStoneDuster);
            controls.Add(enableStoneDuster);
        }

        public static void CreateActions(IMyTerminalBlock block)
        {
            if (block as IMyShipDrill == null || actionCreated == true)
            {
                return;
            }

            actionCreated = true;

            // Toggle Stone Duster Action
            var action = MyAPIGateway.TerminalControls.CreateAction<IMyShipDrill>("EnableStoneDuster");
            action.Enabled = Block => true;
            action.ValidForGroups = true;
            action.Name = new StringBuilder("Toggle Stone Duster");
            action.Action = ToggleControlStatus;
            action.Writer = (Block, Builder) =>
            {

                Builder.Clear();
                Builder.Append(GetControlStatus(Block).ToString());

            };
            MyAPIGateway.TerminalControls.AddAction<IMyShipDrill>(action);
            //controls.Add(action);
            //MyVisualScriptLogicProvider.ShowNotification($"Actions Created", 10000, "Green");
        }

        private static bool GetControlStatus(IMyTerminalBlock block)
        {
            var gamelogic = block.GameLogic.GetAs<GameLogic>();

            if (gamelogic == null) return false;
            return gamelogic.enableStoneDuster;
        }

        private static void SetControlStatus(IMyTerminalBlock block, bool boxValue)
        {
            var gamelogic = block.GameLogic.GetAs<GameLogic>();

            if (gamelogic == null) return;
            gamelogic.enableStoneDuster = boxValue;

            SyncData data = new SyncData();
            data.EntityId = block.EntityId;
            data.DusterEnabled = boxValue;
            Comms.SyncToOthers(data);
        }

        private static void ToggleControlStatus(IMyTerminalBlock block)
        {
            var gamelogic = block.GameLogic.GetAs<GameLogic>();

            if (gamelogic == null) return;
            bool result = gamelogic.enableStoneDuster;

            if (!result)
            {
                gamelogic.enableStoneDuster = true;
            }
            else
            {
                gamelogic.enableStoneDuster = false;
            }
            result = gamelogic.enableStoneDuster;

            SyncData data = new SyncData();
            data.EntityId = block.EntityId;
            data.DusterEnabled = result;
            Comms.SyncToOthers(data);
        }

        public static void SyncDrillData(SyncData data)
        {
            if (data == null) return;

            IMyEntity blockEntity = null;
            MyAPIGateway.Entities.TryGetEntityById(data.EntityId, out blockEntity);
            if (blockEntity == null) return;

            var block = blockEntity as IMyTerminalBlock;
            var gamelogic = block.GameLogic.GetAs<GameLogic>();

            if (gamelogic == null) return;
            gamelogic.enableStoneDuster = data.DusterEnabled;

            if (MyAPIGateway.Multiplayer.IsServer) SaveBlockSettings(block, data.DusterEnabled);
        }

        public static void SaveBlockSettings(IMyTerminalBlock block, bool result)
        {
            if(block.Storage == null)
            {
                block.Storage = new MyModStorageComponent();
            }

            block.Storage[GameLogic.cpmID] = result.ToString();
        }
    }
}


