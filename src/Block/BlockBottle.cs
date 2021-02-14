using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BlockBottle : Block
    {
        public string BottleContentItemCode()
        {
            return Attributes["contentItemCode"].AsString();
        }

        public Block ContentBlockForContents(string contents)
        {
            if (Attributes["contentItem2BlockCodes"][contents]?.Exists != true) return null;

            return api.World.GetBlock(new AssetLocation(Attributes["contentItem2BlockCodes"][contents].AsString()));
        }

        public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity forEntity)
        {
            if ((forEntity as EntityAgent)?.Controls.Sneak != true) return null;

            return base.GetHeldTpUseAnimation(activeHotbarSlot, forEntity);
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null)
            {
                base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
                return;
            }

            BlockLiquidContainerBase blockLiqContainer = byEntity.World.BlockAccessor.GetBlock(blockSel.Position) as BlockLiquidContainerBase;
            IPlayer player = (byEntity as EntityPlayer)?.Player;

            string contents = BottleContentItemCode();

            if (blockLiqContainer != null)
            {
                if (contents == null)
                {
                    ItemStack stack = blockLiqContainer.GetContent(byEntity.World, blockSel.Position);
                    if (stack != null && ContentBlockForContents(stack.Collectible.Code.Path) != null)
                    {
                        InsertIntoBottle(slot, byEntity, stack.Collectible.Code.Path);
                        blockLiqContainer.TryTakeContent(byEntity.World, blockSel.Position, 1);
                    }

                }
                else
                {
                    ItemStack stack = blockLiqContainer.GetContent(byEntity.World, blockSel.Position);
                    if (stack == null || stack.Collectible.Code.Equals(new AssetLocation(BottleContentItemCode())))
                    {
                        Item contentItem = byEntity.World.GetItem(new AssetLocation(BottleContentItemCode()));
                        if (blockLiqContainer.TryPutContent(byEntity.World, blockSel.Position, new ItemStack(contentItem), 1) > 0)
                        {
                            EmptyOutBottle(slot, byEntity);
                        }
                    }
                }

                handHandling = EnumHandHandling.PreventDefaultAction;
                return;
            }


            handHandling = EnumHandHandling.PreventDefault;

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (blockSel != null && (byEntity.World.BlockAccessor.GetBlock(blockSel.Position) as BlockLiquidContainerBase) != null)
            {
                return false;
            }

            return base.OnHeldInteractStep(secondsUsed, slot, byEntity, blockSel, entitySel);
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
            if (GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity as Entity) != null && Attributes["drunkCond"].Exists && byEntity.GetBehavior<EntityBehaviorCondsController>() != null)
            {
                byEntity.GetBehavior<EntityBehaviorCondsController>().addCond(AilmentsUtil.CreateTreeAttribute(Attributes["drunkCond"]));
            }

        }

        private void EmptyOutBottle(ItemSlot itemslot, EntityAgent byEntity)
        {
            Block emptyBottle = byEntity.World.GetBlock(new AssetLocation(Attributes["emptiedBlockCode"].AsString()));
            ItemStack emptyStack = new ItemStack(emptyBottle);

            if (itemslot.Itemstack.StackSize <= 1)
            {
                itemslot.Itemstack = emptyStack;
            }
            else
            {
                IPlayer player = (byEntity as EntityPlayer)?.Player;

                itemslot.TakeOut(1);
                if (!player.InventoryManager.TryGiveItemstack(emptyStack, true))
                {
                    byEntity.World.SpawnItemEntity(emptyStack, byEntity.SidedPos.XYZ);
                }
            }

            itemslot.MarkDirty();
        }


        private void InsertIntoBottle(ItemSlot itemslot, EntityAgent byEntity, string contents)
        {
            Block filledBottle = ContentBlockForContents(contents);
            ItemStack stack = new ItemStack(filledBottle);

            if (itemslot.Itemstack.StackSize <= 1)
            {
                itemslot.Itemstack = stack;
            }
            else
            {
                IPlayer player = (byEntity as EntityPlayer)?.Player;

                itemslot.TakeOut(1);
                if (!player.InventoryManager.TryGiveItemstack(stack, true))
                {
                    byEntity.World.SpawnItemEntity(stack, byEntity.SidedPos.XYZ);
                }
            }

            itemslot.MarkDirty();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            ItemStack active = byPlayer?.InventoryManager?.ActiveHotbarSlot?.Itemstack;
            string drug = Attributes["turnToMed"].AsString();

            if (active == null || drug == null) return base.OnBlockInteractStart(world, byPlayer, blockSel);
            
            JsonObject[] props = active.ItemAttributes["herbProperties"].AsArray();
            if (props == null) return base.OnBlockInteractStart(world, byPlayer, blockSel);
            
            BlockPos medPos = blockSel.Position;
            Block med = world.GetBlock(new AssetLocation("ailments:bottle-medicine"));
            world.BlockAccessor.SetBlock(med.BlockId, medPos);

            BEMedicineContainer mede = world.BlockAccessor.GetBlockEntity(medPos) as BEMedicineContainer;
            if (mede == null) return base.OnBlockInteractStart(world, byPlayer, blockSel);

            
            ItemStack inf = new ItemStack(world.GetItem(new AssetLocation(drug)));

            List<TreeAttribute> conds = new List<TreeAttribute>();

            int level = Attributes["exLevel"].AsInt(1);
            int duration = Attributes["exDur"].AsInt(1);
            int start = Attributes["redStart"].AsInt(1);
            for (int i = 0; i < props.Length; i++)
            {
                conds.Add(AilmentsUtil.CreateTreeAttribute(props[i], level, duration, start));
            }

            ItemStack herb = byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(1);
            byPlayer.InventoryManager.ActiveHotbarSlot.MarkDirty();
            herb.Attributes["conds"] = new TreeArrayAttribute(conds.ToArray());


            TransitionableProperties preservative = world.GetItem(new AssetLocation("ailments:med-dummy-ready")).TransitionableProps[0];
            CarryOverFreshness(world.Api, new ItemSlot[1] { new DummySlot(herb) }, new ItemStack[1] { herb }, preservative);

            mede.Inventory[0].Itemstack = inf;
            mede.Inventory[1].Itemstack = herb;

            return true;
        }
    }
}
