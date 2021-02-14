using System.Collections.Generic;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Ailments
{
    public class ItemEnhancedWeapon : Item
    {
        public override int GetMergableQuantity(ItemStack sinkStack, ItemStack sourceStack, EnumMergePriority priority)
        {
            if (sinkStack.Attributes?.GetInt("augmentTime") == 0 && sourceStack.ItemAttributes != null && sourceStack.ItemAttributes["attackInjuries"].Exists && priority == EnumMergePriority.DirectMerge) return 1;

            return base.GetMergableQuantity(sinkStack, sourceStack, priority);
        }

        public override void TryMergeStacks(ItemStackMergeOperation op)
        {
            JsonObject[] jInjs = op.SourceSlot.Itemstack?.ItemAttributes?["attackInjuries"].AsArray();
            List<TreeAttribute> injs = new List<TreeAttribute>();

            if (jInjs != null && jInjs.Length > 0)
            {
                foreach (JsonObject jInj in jInjs)
                {
                    injs.Add(AilmentsUtil.CreateTreeAttribute(jInj));
                }

                op.SinkSlot.Itemstack.Attributes["attackInjuries"] = new TreeArrayAttribute(injs.ToArray());
                op.SinkSlot.Itemstack.Attributes.SetInt("augmentTime", op.SourceSlot.Itemstack.ItemAttributes["augmentTime"].AsInt(50));
                op.SinkSlot.Itemstack.Attributes.SetInt("durability", op.SinkSlot.Itemstack.Attributes.GetInt("durability") == 0 ?
                  op.SourceSlot.Itemstack.ItemAttributes["augmentTime"].AsInt(20) + op.SinkSlot.Itemstack.Collectible.Durability :
                  op.SinkSlot.Itemstack.Attributes.GetInt("durability") + op.SourceSlot.Itemstack.ItemAttributes["augmentTime"].AsInt(20));
                op.SinkSlot.Itemstack.Attributes.SetString("augmentName", op.SourceSlot.Itemstack.ItemAttributes["augmentName"].AsString());
                op.SourceSlot.TakeOut(1);
                return;
            }

            base.TryMergeStacks(op);
        }

        public override string GetHeldItemName(ItemStack itemStack)
        {
            if (itemStack.Attributes.GetInt("augmentTime") > 0) return "Modified " + base.GetHeldItemName(itemStack);
            return base.GetHeldItemName(itemStack);
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            if (inSlot.Itemstack.Attributes.GetInt("augmentTime") > 0)
            {
                if (Lang.GetIfExists("ailments:augmentName-" + inSlot.Itemstack.Attributes.GetString("augmentName")) != null) dsc.AppendLine(Lang.Get("ailments:augmentName-" + inSlot.Itemstack.Attributes.GetString("augmentName")));
                dsc.AppendLine("Augment Uses " + inSlot.Itemstack.Attributes.GetInt("augmentTime"));
            }
        }
    }
}
