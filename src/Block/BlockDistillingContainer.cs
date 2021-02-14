using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BlockDistillingContainer : Block
    {
        public override bool CanSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemStack inputStack, ItemStack outputStack)
        {
            if (GetAlcohol(cookingSlotsProvider) != 0 && outputStack == null) return true; else return false;
        }
        public override void DoSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot)
        {
            if (GetAlcohol(cookingSlotsProvider) <= 0 && outputSlot.Itemstack == null) return;

            Block dis = world.BlockAccessor.GetBlock(new AssetLocation("ailments:alembic-filled"));
            outputSlot.Itemstack = new ItemStack(dis);

            BlockLiquidContainerBase alembic = dis as BlockLiquidContainerBase;
            alembic.SetContent(outputSlot.Itemstack, new ItemStack(world.GetItem(new AssetLocation("ailments:distilledalcoholportion")), GetAlcohol(cookingSlotsProvider)));
            inputSlot.Itemstack = null;
            for (int i = 0; i < cookingSlotsProvider.Slots.Length; i++) cookingSlotsProvider.Slots[i] = null;


        }

        public override float GetMeltingDuration(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return 120 + (GetAlcohol(cookingSlotsProvider) * 12);
        }

        public override float GetMeltingPoint(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return 100f;
        }

        public int GetAlcohol(ISlotProvider cookSlots)
        {
            int wineCount = 0;
            int aleCount = 0;

            for (int i = 0; i < cookSlots.Slots.Length; i++)
            {
                if (cookSlots.Slots[i].Itemstack == null) continue;
                if (cookSlots.Slots[i].Itemstack.Collectible.Code.Path == "wineportion")
                {
                    wineCount += cookSlots.Slots[i].Itemstack.StackSize;
                }
                else if (cookSlots.Slots[i].Itemstack.Collectible.Code.Path == "aleportion")
                {
                    aleCount += cookSlots.Slots[i].Itemstack.StackSize;
                }
                else return 0;

            }

            if (aleCount == 0 && wineCount == 0 || aleCount % 20 != 0 || wineCount % 10 != 0) return 0;

            return (wineCount / 10) + (aleCount / 20);
        }
    }
}
