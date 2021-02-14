using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BlockDistilledContainer : BlockLiquidContainerBase
    {

        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null) return;
            IPlayer byPlayer = (byEntity as EntityPlayer)?.Player;
            bool singlePut = byPlayer.WorldData.EntityControls.Sneak;

            ItemStack contentStack = GetContent(byEntity.World, itemslot.Itemstack);
            bool isEmpty = contentStack == null || contentStack.StackSize == 0;

            Block targetedBlock = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);

            if (!byEntity.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                byEntity.World.BlockAccessor.MarkBlockDirty(blockSel.Position.AddCopy(blockSel.Face));
                byPlayer?.InventoryManager.ActiveHotbarSlot?.MarkDirty();
                return;
            }

            if (!isEmpty)
            {
                BlockLiquidContainerBase targetBucket = targetedBlock as BlockLiquidContainerBase;
                if (targetBucket != null && !(targetBucket is BlockDistilledContainer))
                {
                    WaterTightContainableProps props = GetContentProps(byEntity.World, itemslot.Itemstack);
                    int poured = targetBucket.TryPutContent(byEntity.World, blockSel.Position, contentStack, singlePut? 1 : contentStack.StackSize);

                    if (poured > 0)
                    {
                        TryTakeContent(byEntity.World, itemslot.Itemstack, singlePut ? 1 : poured);
                        byEntity.World.PlaySoundAt(props.FillSpillSound, blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);
                    }

                }
                
            }

            if (contentStack == null || contentStack.StackSize == 0)
            {
                itemslot.Itemstack = new ItemStack(byEntity.World.BlockAccessor.GetBlock(new AssetLocation("ailments:alembic-burned")));
                itemslot.MarkDirty();
            }

            // Prevent placing on normal use
            handHandling = EnumHandHandling.PreventDefaultAction;
        }


        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (!hotbarSlot.Empty && ((hotbarSlot.Itemstack.Collectible is BlockBottle) || (hotbarSlot.Itemstack.Collectible is BlockBowl)) && hotbarSlot.Itemstack.Collectible.LastCodePart() != "burned") return true;

            if (!hotbarSlot.Empty && hotbarSlot.Itemstack.Collectible.Attributes?.IsTrue("handleLiquidContainerInteract") == true)
            {
                EnumHandHandling handling = EnumHandHandling.NotHandled;
                hotbarSlot.Itemstack.Collectible.OnHeldInteractStart(hotbarSlot, byPlayer.Entity, blockSel, null, true, ref handling);
                if (handling == EnumHandHandling.PreventDefault || handling == EnumHandHandling.PreventDefaultAction) return true;
            }

            if (hotbarSlot.Empty || !(hotbarSlot.Itemstack.Collectible is ILiquidInterface)) return false;


            CollectibleObject obj = hotbarSlot.Itemstack.Collectible;

            bool singleTake = byPlayer.WorldData.EntityControls.Sneak;
            bool singlePut = byPlayer.WorldData.EntityControls.Sprint;

            /*if (obj is ILiquidSource && !singleTake)
            {
                int moved = TryPutContent(world, blockSel.Position, (obj as ILiquidSource).GetContent(world, hotbarSlot.Itemstack), singlePut ? 1 : 9999);

                if (moved > 0)
                {
                    (obj as ILiquidSource).TryTakeContent(world, hotbarSlot.Itemstack, moved);
                    (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);

                    return true;
                }
            }*/

            if (obj is ILiquidSink && !singlePut)
            {
                ItemStack owncontentStack = GetContent(world, blockSel.Position);
                int moved = 0;

                if (hotbarSlot.Itemstack.StackSize == 1)
                {
                    moved = (obj as ILiquidSink).TryPutContent(world, hotbarSlot.Itemstack, owncontentStack, singleTake ? 1 : 9999);
                }
                else
                {
                    ItemStack containerStack = hotbarSlot.Itemstack.Clone();
                    containerStack.StackSize = 1;
                    moved = (obj as ILiquidSink).TryPutContent(world, containerStack, owncontentStack, singleTake ? 1 : 9999);

                    if (moved > 0)
                    {
                        hotbarSlot.TakeOut(1);
                        if (!byPlayer.InventoryManager.TryGiveItemstack(containerStack, true))
                        {
                            api.World.SpawnItemEntity(containerStack, byPlayer.Entity.SidedPos.XYZ);
                        }
                    }
                }

                if (moved > 0)
                {
                    TryTakeContent(world, blockSel.Position, moved);
                    (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                    BEDistilledContainer bd = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEDistilledContainer;
                    if (bd != null) bd.ReplaceBlock(world);
                    return true;
                }
            }

            return true;
        }

        public override void TryFillFromBlock(EntityItem byEntityItem, BlockPos pos)
        {
            // Don't fill when dropped as item in water
        }

        public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
        {
            bool val = base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);

            if (val)
            {
                BEDistilledContainer bect = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEDistilledContainer;
                if (bect != null)
                {
                    BlockPos targetPos = blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.Opposite) : blockSel.Position;
                    double dx = byPlayer.Entity.Pos.X - (targetPos.X + blockSel.HitPosition.X);
                    double dz = byPlayer.Entity.Pos.Z - (targetPos.Z + blockSel.HitPosition.Z);
                    float angleHor = (float)Math.Atan2(dx, dz);

                    float deg22dot5rad = GameMath.PIHALF / 4;
                    float roundRad = ((int)Math.Round(angleHor / deg22dot5rad)) * deg22dot5rad;
                    bect.MeshAngle = roundRad;
                }
            }

            return val;
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction()
                {
                    ActionLangCode = "heldhelp-empty",
                    HotKeyCode = "sprint",
                    MouseButton = EnumMouseButton.Right,
                    ShouldApply = (wi, bs, es) => {
                        return GetCurrentLitres(api.World, inSlot.Itemstack) > 0;
                    }
                },
                new WorldInteraction()
                {
                    ActionLangCode = "heldhelp-place",
                    HotKeyCode = "sneak",
                    MouseButton = EnumMouseButton.Right,
                    ShouldApply = (wi, bs, es) => {
                        return true;
                    }
                }
            };
        }
    }
}
