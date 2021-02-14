using System;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace Ailments
{
    public class ItemFirstAid : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            
            JsonObject attr = slot.Itemstack.Collectible.Attributes;
            if (attr == null || (!attr["health"].Exists && !attr["firstAidRemoval"].Exists)) return;

            if (!byEntity.Controls.Sneak)
            {
                handling = EnumHandHandling.PreventDefault;

                byEntity.World.RegisterCallback((dt) =>
                {
                    if (byEntity.Controls.HandUse == EnumHandInteract.HeldItemInteract)
                    {
                        IPlayer player = null;
                        if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                        byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/poultice"), byEntity, player);
                    }
                }, 200);

                return;
            }
            else if (entitySel != null)
            {
                handling = EnumHandHandling.PreventDefault;

                byEntity.World.RegisterCallback((dt) =>
                {
                    if (byEntity.Controls.HandUse == EnumHandInteract.HeldItemInteract)
                    {
                        IPlayer player = null;
                        if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                        byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/poultice"), byEntity, player);
                    }
                }, 200);

                return;
            }
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {

            if (byEntity.World is IClientWorldAccessor && !byEntity.Controls.Sneak)
            {
                ModelTransform tf = new ModelTransform();

                tf.EnsureDefaultValues();

                tf.Origin.Set(0f, 0, 0f);

                tf.Translation.X -= Math.Min(1.5f, secondsUsed * 4 * 1.57f);

                //tf.Rotation.X += Math.Min(30f, secondsUsed * 350);
                tf.Rotation.Y += Math.Min(130f, secondsUsed * 350);

                byEntity.Controls.UsingHeldItemTransformAfter = tf;

                return secondsUsed < 0.75f;
            }
            else if (entitySel != null && byEntity.Api.Side == EnumAppSide.Server)
            {
                return secondsUsed < 3;
            }

            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (secondsUsed > 0.7f && !byEntity.Controls.Sneak)
            {
                JsonObject attr = slot.Itemstack.Collectible.Attributes;
                float health = attr["health"].AsFloat();
                string[] repairs = attr["firstAidRemoval"].AsArray<string>();
                EntityBehaviorCondsController cc = byEntity.GetBehavior<EntityBehaviorCondsController>();
                if (health != 0 && byEntity.World.Side == EnumAppSide.Server) byEntity.ReceiveDamage(new DamageSource()
                {
                    Source = EnumDamageSource.Internal,
                    Type = health > 0 ? EnumDamageType.Heal : EnumDamageType.Poison
                }, Math.Abs(health));

                if (byEntity.Alive && repairs != null && repairs.Length > 0 && cc != null)
                {
                    foreach (string repair in repairs)
                    {
                        cc.tryToRemoveCond(repair);
                    }
                }


                slot.TakeOut(1);
                slot.MarkDirty();
            }
            else if (entitySel != null && byEntity.Api.Side == EnumAppSide.Server && secondsUsed > 2.9f)
            {
                Entity creature = entitySel.Entity;
                JsonObject attr = slot.Itemstack.Collectible.Attributes;
                float health = attr["health"].AsFloat();
                string[] repairs = attr["firstAidRemoval"].AsArray<string>();
                EntityBehaviorCondsController cc = creature.GetBehavior<EntityBehaviorCondsController>();
                if (health != 0 && byEntity.World.Side == EnumAppSide.Server) creature.ReceiveDamage(new DamageSource()
                {
                    Source = EnumDamageSource.Entity,
                    SourceEntity = byEntity,
                    Type = health > 0 ? EnumDamageType.Heal : EnumDamageType.Poison
                }, Math.Abs(health));

                if (creature.Alive && repairs != null && repairs.Length > 0 && cc != null)
                {
                    bool repaired = false;

                    foreach (string repair in repairs)
                    {
                        repaired = cc.tryToRemoveCond(repair);
                        if (repaired) break;
                    }
                }


                slot.TakeOut(1);
                slot.MarkDirty();
            }
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            JsonObject attr = inSlot.Itemstack.Collectible.Attributes;
            if (attr != null && attr["health"].Exists)
            {
                float health = attr["health"].AsFloat();
                dsc.AppendLine(Lang.Get("When used: +{0} hp", health));
            }
        }


        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[] {
                new WorldInteraction()
                {
                    ActionLangCode = "heldhelp-heal",
                    MouseButton = EnumMouseButton.Right,
                },
                new WorldInteraction()
                {
                    ActionLangCode = "ailments:heldhelp-applymedicine",
                    HotKeyCode = "sneak",
                    MouseButton = EnumMouseButton.Right,
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
}
