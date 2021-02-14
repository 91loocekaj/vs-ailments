using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Ailments
{
    public class ItemEnhancedArrow : ItemArrow
    {
        public override string GetHeldItemName(ItemStack itemStack)
        {
            if (itemStack.Attributes?["arrowPoison"] != null || itemStack.Attributes?["attackInjuries"] != null)
            {
                return "Tipped " + base.GetHeldItemName(itemStack);
            }
            return base.GetHeldItemName(itemStack);
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            if (Lang.GetIfExists("ailments:poisonName-" + inSlot.Itemstack.Attributes.GetString("poisonName")) != null) dsc.AppendLine("Covered in " + Lang.Get("ailments:poisonName-" + inSlot.Itemstack.Attributes.GetString("poisonName")));
        }

        public bool checkPoison(ITreeAttribute source, ITreeAttribute compare)
        {
            //XOR Gate
            if (source?["arrowPoison"] == null && compare?["arrowPoison"] == null) return true;
            if (source?["arrowPoison"] == null || compare?["arrowPoison"] == null) return false;

            TreeAttribute[] sarray = (source["arrowPoison"] as TreeArrayAttribute).value;
            TreeAttribute[] carray = (source["arrowPoison"] as TreeArrayAttribute).value;

            if (sarray == null && carray == null) return true;
            if (sarray == null || carray == null) return false;
            if (sarray.Length != carray.Length) return false;

            for (int i = 0; i < sarray.Length; i++)
            {
                if (sarray[i].GetString("name") != carray[i].GetString("name")) return false;
                if (sarray[i].GetBool("mech") != carray[i].GetBool("mech")) return false;
                if (sarray[i].GetInt("level") != carray[i].GetInt("level")) return false;
                if (sarray[i].GetInt("duration") != carray[i].GetInt("duration")) return false;
            }

            return true;
        }
        public override int GetMergableQuantity(ItemStack sinkStack, ItemStack sourceStack, EnumMergePriority priority)
        {
            string[] cons = { "attackInjuries", "arrowPoison" };
            cons = cons.Concat(GlobalConstants.IgnoredStackAttributes).ToArray();
            if (sinkStack.StackSize < MaxStackSize && sourceStack.Equals(api.World, sinkStack, cons) && checkPoison(sourceStack.Attributes, sinkStack.Attributes)) return Math.Min(MaxStackSize - sinkStack.StackSize, sourceStack.StackSize);

            if (sinkStack.Attributes["arrowPoison"] == null && sourceStack.ItemAttributes != null && sourceStack.ItemAttributes["arrowPoison"].Exists && priority == EnumMergePriority.DirectMerge) return 1;

            return base.GetMergableQuantity(sinkStack, sourceStack, priority);
        }

        public override void TryMergeStacks(ItemStackMergeOperation op)
        {
            JsonObject[] jpois = op.SourceSlot.Itemstack.ItemAttributes?["arrowPoison"]?.AsArray();
            List<TreeAttribute> poiList = new List<TreeAttribute>();
            if (jpois != null && jpois.Length > 0)
            {
                foreach (JsonObject jpoi in jpois)
                {
                    poiList.Add(AilmentsUtil.CreateTreeAttribute(jpoi));
                }

                bool AvsP = op.SinkSlot.StackSize > op.SourceSlot.StackSize;

                if (AvsP)
                {
                    ItemStack tipped = op.SinkSlot.TakeOut(op.SourceSlot.StackSize);
                    tipped.Attributes["arrowPoison"] = new TreeArrayAttribute(poiList.ToArray());
                    tipped.Attributes.SetString("poisonName", op.SourceSlot.Itemstack.ItemAttributes["poisonName"].AsString());
                    op.SourceSlot.Itemstack = tipped;
                    return;
                }
                else
                {
                    int dif = op.SourceSlot.StackSize - op.SinkSlot.StackSize;
                    ItemStack tipped = op.SinkSlot.TakeOut(op.SourceSlot.StackSize);
                    tipped.Attributes["arrowPoison"] = new TreeArrayAttribute(poiList.ToArray());
                    tipped.Attributes.SetString("poisonName", op.SourceSlot.Itemstack.ItemAttributes["poisonName"].AsString());
                    op.SinkSlot.Itemstack = tipped;
                    if (dif > 0) op.SourceSlot.Itemstack.StackSize = dif; else op.SourceSlot.TakeOutWhole();
                    return;
                }
            }

            base.TryMergeStacks(op);
        }
    }
}
