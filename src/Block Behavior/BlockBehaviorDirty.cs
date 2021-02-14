using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Ailments
{
    public class BlockBehaviorDirty : BlockBehavior
    {
        TreeAttribute[] diseases;

        public override void Initialize(JsonObject properties)
        {
            TreeAttribute[] diseases = AilmentsUtil.CreateMultipleConds(properties["diseases"].AsArray());
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            EntityBehaviorCondsController cc;
            if (byPlayer.Entity == null || (cc = byPlayer.Entity.GetBehavior<EntityBehaviorCondsController>()) == null) return;

            foreach (TreeAttribute disease in diseases)
            {
                cc.catchDisease(disease);
            }

            base.OnBlockBroken(world, pos, byPlayer, ref handling);
        }

        public BlockBehaviorDirty(Block block) : base(block)
        {
        }
    }
}
