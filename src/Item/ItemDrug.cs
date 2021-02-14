using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace Ailments
{
    public class ItemDrug : Item
    {
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
            if (GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity as Entity) != null && Attributes["drugCond"].Exists && byEntity.GetBehavior<EntityBehaviorCondsController>() != null)
            {
                byEntity.GetBehavior<EntityBehaviorCondsController>().addCond(AilmentsUtil.CreateTreeAttribute(Attributes["drugCond"]));
            }

        }
    }
}
