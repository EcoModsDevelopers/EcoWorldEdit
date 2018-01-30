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
    using Eco.Mods.TechTree;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;

    [Serialized]
    [Category("Hidden")]
    public partial class WandAxeItem : DevtoolItem
    {
        public override string FriendlyName { get { return "Wand Tool"; } }
        public override string Description { get { return "Does magical World Edit things"; } }

        public override string LeftActionDescription { get { return string.Empty; } }

        public override ClientPredictedBlockAction LeftAction { get { return ClientPredictedBlockAction.None; } }

        public override InteractResult OnActLeft(InteractionContext context)
        {
            try
            {
                if (context.BlockPosition == null || !context.BlockPosition.HasValue)
                    return InteractResult.Success;

                var pos = context.BlockPosition.Value;

                WorldEditUserData weud = WorldEditManager.GetUserData(context.Player.User.Name);
                weud.FirstPos = pos;

                context.Player.SendTemporaryMessage("First Position set to (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
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

                WorldEditUserData weud = WorldEditManager.GetUserData(context.Player.User.Name);
                weud.SecondPos = pos;

                context.Player.SendTemporaryMessage("Second Position set to (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
            return InteractResult.Success;
        }

        public override bool ShouldHighlight(Type block)
        {
            return true;
        }
    }
}
