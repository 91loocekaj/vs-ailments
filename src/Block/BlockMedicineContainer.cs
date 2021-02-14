using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BlockMedicineContainer : BlockContainer
    {
        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
        {
            base.OnBlockPlaced(world, blockPos, byItemStack);
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (!byEntity.Controls.Sneak)
            {
                ItemStack[] stacks = GetNonEmptyContents(byEntity.World, slot.Itemstack);
                if (stacks.Length != 2 || stacks[1].Attributes["conds"] == null || stacks[0].Collectible.LastCodePart() != "ready") return;
                EntityBehaviorCondsController cc = byEntity.GetBehavior<EntityBehaviorCondsController>();
                if (cc == null) return;

                byEntity.World.RegisterCallback((dt) =>
                {
                    if (byEntity.Controls.HandUse == EnumHandInteract.HeldItemInteract)
                    {
                        IPlayer player = null;
                        if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                        byEntity.PlayEntitySound("eat", player);
                    }
                }, 500);

                byEntity.AnimManager?.StartAnimation("eat");

                handHandling = EnumHandHandling.PreventDefaultAction;
            }
            else if (entitySel != null)
            {
                ItemStack[] stacks = GetNonEmptyContents(byEntity.World, slot.Itemstack);
                if (stacks.Length != 2 || stacks[1].Attributes["conds"] == null || stacks[0].Collectible.LastCodePart() != "ready") return;
                handHandling = EnumHandHandling.PreventDefaultAction;
                EntityBehaviorCondsController cc = entitySel.Entity.GetBehavior<EntityBehaviorCondsController>();
                if (cc == null) return;
            }
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!byEntity.Controls.Sneak)
            {
                Vec3d pos = byEntity.Pos.AheadCopy(0.4f).XYZ;
                pos.X += byEntity.LocalEyePos.X;
                pos.Y += byEntity.LocalEyePos.Y - 0.4f;
                pos.Z += byEntity.LocalEyePos.Z;


                if (secondsUsed > 0.5f && (int)(30 * secondsUsed) % 7 == 1)
                {
                    byEntity.World.SpawnCubeParticles(pos, slot.Itemstack, 0.3f, 4, 0.5f, (byEntity as EntityPlayer)?.Player);
                }


                if (byEntity.World is IClientWorldAccessor)
                {
                    ModelTransform tf = new ModelTransform();

                    tf.EnsureDefaultValues();

                    tf.Origin.Set(0f, 0, 0f);

                    if (secondsUsed > 0.5f)
                    {
                        tf.Translation.Y = Math.Min(0.02f, GameMath.Sin(20 * secondsUsed) / 10);
                    }

                    tf.Translation.X -= Math.Min(1f, secondsUsed * 4 * 1.57f);
                    tf.Translation.Y -= Math.Min(0.05f, secondsUsed * 2);

                    tf.Rotation.X += Math.Min(30f, secondsUsed * 350);
                    tf.Rotation.Y += Math.Min(80f, secondsUsed * 350);

                    byEntity.Controls.UsingHeldItemTransformAfter = tf;

                    return secondsUsed <= 1f;
                }

                // Let the client decide when he is done eating
                return true;
            }
            else if (entitySel != null && byEntity.Api.Side == EnumAppSide.Server)
            {
                if (secondsUsed >= 3) System.Diagnostics.Debug.WriteLine("# has passed");
                return secondsUsed <= 3f;
            }

            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!byEntity.Controls.Sneak)
            {
                ItemStack[] stacks = GetNonEmptyContents(byEntity.World, slot.Itemstack);
                if (stacks.Length != 2 || stacks[1].Attributes["conds"] == null || stacks[0].Collectible.LastCodePart() != "ready") return;
                EntityBehaviorCondsController cc = byEntity.GetBehavior<EntityBehaviorCondsController>();
                if (cc == null || secondsUsed < 1f) return;

                TreeAttribute[] conds = (stacks[1].Attributes["conds"] as TreeArrayAttribute).value;

                foreach (TreeAttribute cond in conds)
                {
                    cc.addCond(cond);
                }

                stacks[1].Attributes.SetInt("usedTimes", stacks[1].Attributes.GetInt("usedTimes") + 1);
                System.Diagnostics.Debug.WriteLine("Used " + stacks[1].Attributes.GetInt("usedTimes"));
                if (stacks[1].Attributes.GetInt("usedTimes") >= stacks[0].ItemAttributes?["maxUses"].AsInt(10))
                {
                    slot.TakeOut(1);
                    slot.MarkDirty();

                    IPlayer byPlayer = null;
                    IWorldAccessor world = byEntity.World;
                    if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                    ItemStack stack = new ItemStack(world.GetBlock(new AssetLocation("ailments:bottle-burned")));
                    if (byPlayer?.InventoryManager.TryGiveItemstack(stack) == false)
                    {
                        byEntity.World.SpawnItemEntity(stack, byEntity.SidedPos.XYZ);
                    }

                }
            }
            else if (entitySel != null)
            {
                ItemStack[] stacks = GetNonEmptyContents(byEntity.World, slot.Itemstack);
                if (stacks.Length != 2 || stacks[1].Attributes["conds"] == null || stacks[0].Collectible.LastCodePart() != "ready") return;
                EntityBehaviorCondsController cc = entitySel.Entity.GetBehavior<EntityBehaviorCondsController>();
                if (cc == null) return;


                TreeAttribute[] conds = (stacks[1].Attributes["conds"] as TreeArrayAttribute).value;

                foreach (TreeAttribute cond in conds)
                {
                    cc.addCond(cond);
                }

                stacks[1].Attributes.SetInt("usedTimes", stacks[1].Attributes.GetInt("usedTimes") + 1);
                System.Diagnostics.Debug.WriteLine("Used " + stacks[1].Attributes.GetInt("usedTimes"));
                entitySel.Entity.Api.World.PlaySoundAt(new AssetLocation("game:sounds/environment/smallsplash.ogg"), entitySel.Entity);


                if (stacks[1].Attributes.GetInt("usedTimes") >= stacks[0].ItemAttributes?["maxUses"].AsInt(10))
                {
                    slot.TakeOut(1);
                    slot.MarkDirty();

                    IPlayer byPlayer = null;
                    IWorldAccessor world = byEntity.World;
                    if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                    ItemStack stack = new ItemStack(world.GetBlock(new AssetLocation("ailments:bottle-burned")));
                    if (byPlayer?.InventoryManager.TryGiveItemstack(stack) == false)
                    {
                        byEntity.World.SpawnItemEntity(stack, byEntity.SidedPos.XYZ);
                    }

                }
                else
                {
                    ItemStack stack = slot.Itemstack.Clone();

                    slot.TakeOut(1);
                    slot.MarkDirty();

                    IPlayer byPlayer = null;
                    IWorldAccessor world = byEntity.World;
                    if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                    
                    if (byPlayer?.InventoryManager.TryGiveItemstack(stack) == false)
                    {
                        byEntity.World.SpawnItemEntity(stack, byEntity.SidedPos.XYZ);
                    }
                }
            }
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            DummyInventory inv = new DummyInventory(api);
            ItemStack[] stacks = GetNonEmptyContents(world, inSlot.Itemstack);

            if (stacks == null || stacks.Length == 0)
            {
                dsc.AppendLine("Empty");
                return;
            }

            ItemSlot dummy = GetDummySlotForFirstPerishableStack(world, stacks[0], null, inv);
            dummy.Itemstack?.Collectible.AppendPerishableInfoText(dummy, dsc, world);
            if (stacks[0].Item.LastCodePart() == "ready")
            {
                dsc.AppendLine((stacks[0].ItemAttributes["maxUses"].AsInt(10) - stacks[1].Attributes.GetInt("usedTimes")) + " uses left.");
            }
            else if (stacks[0].Item.LastCodePart(1) == "tea")
            {
                dsc.AppendLine("Needs to be boiled.");
            }
            else
            {
                dsc.AppendLine("Not ready yet.");
            }
        }

        public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            BEMedicineContainer mede = world.BlockAccessor.GetBlockEntity(pos) as BEMedicineContainer;
            if (mede == null) return null;

            ItemStack[] stacks = mede.Inventory.Where(slot => !slot.Empty).Select(slot => slot.Itemstack).ToArray();

            if (stacks == null || stacks.Length == 0)
            {
                return "Empty";
            }

            StringBuilder dsc = new StringBuilder();

            DummyInventory inv = new DummyInventory(api);
            ItemSlot dummy = GetDummySlotForFirstPerishableStack(world, stacks[0], null, inv);
            dummy.Itemstack?.Collectible.AppendPerishableInfoText(dummy, dsc, world);

            if (stacks[0].Item.LastCodePart() == "ready")
            {
                dsc.AppendLine((stacks[0].ItemAttributes["maxUses"].AsInt(10) - stacks[1].Attributes.GetInt("usedTimes")) + " uses left.");
            }
            else if (stacks[0].Item.LastCodePart(1) == "tea")
            {
                dsc.AppendLine("Needs to be boiled.");
            }
            else
            {
                dsc.AppendLine("Not ready yet.");
            }

            return dsc.ToString();
        }

        public static ItemSlot GetDummySlotForFirstPerishableStack(IWorldAccessor world, ItemStack stack, Entity forEntity, InventoryBase slotInventory)
        {
            if (stack != null)
            {


                TransitionableProperties[] props = stack.Collectible.GetTransitionableProperties(world, stack, forEntity);
                if (!(props != null && props.Length > 0))
                { 
                    stack = null;
                }

            }

            DummySlot slot = new DummySlot(stack, slotInventory);
            slot.MarkedDirty += () => true;

            return slot;
        }

        public override string GetHeldItemName(ItemStack itemStack)
        {
            if ((itemStack?.Attributes?.GetTreeAttribute("contents")?.Values[0] as ItemstackAttribute)?.value.Collectible.LastCodePart() == "rot")
            {
                return Lang.Get("ailments:bottle-rot");
            }
            string name = Lang.Get("ailments:herbName-" + (itemStack?.Attributes?.GetTreeAttribute("contents")?.Values[1] as ItemstackAttribute)?.value.Collectible.Code.Path);
            string type = (itemStack?.Attributes.GetTreeAttribute("contents")?.Values[0] as ItemstackAttribute)?.value?.GetName();
            return type + name;
        }

        public override bool CanSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemStack inputStack, ItemStack outputStack)
        {
            ItemStack cookable = GetContents(world, inputStack)[0];

            return
                cookable != null
                && cookable.Item.CombustibleProps?.SmeltedStack?.ResolvedItemstack != null
                && outputStack == null;
        }

        public override void DoSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot)
        {
            if (!CanSmelt(world, cookingSlotsProvider, inputSlot.Itemstack, outputSlot.Itemstack)) return;
            ItemStack[] contents = GetContents(world, inputSlot.Itemstack);

            ItemStack cooked = contents[0].Item.CombustibleProps.SmeltedStack.ResolvedItemstack.Clone();
            if (outputSlot.Itemstack == null)
            {
                outputSlot.Itemstack = inputSlot.Itemstack.Clone();
                cooked.Attributes.SetString("infusionName", cooked.ItemAttributes?["infusionName"]?.AsString("Infusion") != null ? cooked.ItemAttributes?["infusionName"]?.AsString() : "Infusion of ");
                contents[0] = cooked;
                SetContents(outputSlot.Itemstack, contents);
                inputSlot.Itemstack = null;
            }
        }

        public override float GetMeltingPoint(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return GetContents(world, inputSlot.Itemstack)[0].Item.CombustibleProps.MeltingPoint;
        }

        public override float GetMeltingDuration(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return GetContents(world, inputSlot.Itemstack)[0].Item.CombustibleProps.MeltingDuration;
        }

        public override void OnGroundIdle(EntityItem entityItem)
        {
            base.OnGroundIdle(entityItem);

            if (entityItem.World.Side == EnumAppSide.Client) return;

            if (GetContents(entityItem.World, entityItem.Itemstack)[0].Collectible.LastCodePart() == "rot" && entityItem.Swimming)
            {
                entityItem.World.SpawnItemEntity(new ItemStack(entityItem.World.GetItem(new AssetLocation("game:rot"))), entityItem.ServerPos.XYZ);
                entityItem.World.SpawnItemEntity(new ItemStack(entityItem.World.GetBlock(new AssetLocation("ailments:bottle-burned"))), entityItem.ServerPos.XYZ);
                entityItem.Die();
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction()
                {
                    ActionLangCode = "ailments:heldhelp-usemedicine",
                    MouseButton = EnumMouseButton.Right
                },
                new WorldInteraction()
                {
                    ActionLangCode = "ailments:heldhelp-applymedicine",
                    HotKeyCode = "sneak",
                    MouseButton = EnumMouseButton.Right
                }
            };
        }
    }
}
