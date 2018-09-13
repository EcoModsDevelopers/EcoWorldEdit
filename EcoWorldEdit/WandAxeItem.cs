using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    using System;
    using System.ComponentModel;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Shared.Items;
    using Eco.World;
    using Eco.World.Blocks;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Core.Utils;
    using Eco.Shared.Math;
    using Eco.Gameplay.Players;
    using Eco.Core.Utils.AtomicAction;

    [Serialized]
    [Category("Hidden")]
    public partial class WandAxeItem : ToolItem
    {
        private static IDynamicValue skilledRepairCost = new ConstantValue(1);
        
        public override string FriendlyName { get { return "Wand Tool"; } }
        public override string Description { get { return "Does magical World Edit things"; } }

        public override string LeftActionDescription { get { return string.Empty; } }

        public override ClientPredictedBlockAction LeftAction { get { return ClientPredictedBlockAction.None; } }

        public override IDynamicValue SkilledRepairCost => skilledRepairCost;

        public override InteractResult OnActLeft(InteractionContext context)
        {
            try
            {
                if (context.BlockPosition == null || !context.BlockPosition.HasValue)
                    return InteractResult.Success;

                var pos = context.BlockPosition.Value;

                pos.X = pos.X < 0 ? pos.X + Shared.Voxel.World.VoxelSize.X : pos.X;
                pos.Z = pos.Z < 0 ? pos.Z + Shared.Voxel.World.VoxelSize.Z : pos.Z;

                pos.X = pos.X % Shared.Voxel.World.VoxelSize.X;
                pos.Z = pos.Z % Shared.Voxel.World.VoxelSize.Z;

                WorldEditUserData weud = WorldEditManager.GetUserData(context.Player.User.Name);
                weud.FirstPos = pos;

                context.Player.SendTemporaryMessage($"First Position set to ({pos.x}, {pos.y}, {pos.z})");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
            return InteractResult.Success;
        }

        public override InteractResult OnActRight(InteractionContext context)
        {
            try
            {
                if (context.BlockPosition == null || !context.BlockPosition.HasValue)
                    return InteractResult.Success;

                var pos = context.BlockPosition.Value;

                pos.X = pos.X < 0 ? pos.X + Shared.Voxel.World.VoxelSize.X : pos.X;
                pos.Z = pos.Z < 0 ? pos.Z + Shared.Voxel.World.VoxelSize.Z : pos.Z;

                pos.X = pos.X % Shared.Voxel.World.VoxelSize.X;
                pos.Z = pos.Z % Shared.Voxel.World.VoxelSize.Z;

                WorldEditUserData weud = WorldEditManager.GetUserData(context.Player.User.Name);
                weud.SecondPos = pos;

                context.Player.SendTemporaryMessage($"Second Position set to ({pos.x}, {pos.y}, {pos.z})");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
            return InteractResult.NoOp;
        }

        public override bool ShouldHighlight(Type block)
        {
            return true;
        }

        protected new Result PlayerPlaceBlock(Type blockType, Vector3i blockPosition, Player player, bool replaceBlock, float calorieMultiplier = 1, params IAtomicAction[] additionalActions)
        {
            return Result.Succeeded;
        }

        protected new Result PlayerPlaceBlock<T>(Vector3i blockPosition, Player player, bool replaceBlock, float calorieMultiplier = 1, params IAtomicAction[] additionalActions)
        {
            return Result.Succeeded;
        }

        protected new Result PlayerDeleteBlock(Vector3i blockPosition, Player player, bool addToInventory, float calorieMultiplier = 1, BlockItem fallbackGiveItem = null, params IAtomicAction[] additionalActions)
        {
            return Result.Succeeded;
        }

        protected new void BurnCalories(Player player, float calorieMultiplier = 1)
        {
        }
    }
}
