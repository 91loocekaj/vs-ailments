using System.Collections.Generic;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class ItemEnhancedSpear : ItemSpear
    {
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (byEntity.Attributes.GetInt("aimingCancel") == 1) return;

            byEntity.Attributes.SetInt("aiming", 0);
            byEntity.StopAnimation("aim");

            if (secondsUsed < 0.35f) return;

            float damage = 1.5f;

            if (slot.Itemstack.Collectible.Attributes != null)
            {
                damage = slot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);
            }

            (api as ICoreClientAPI)?.World.AddCameraShake(0.17f);

            ItemStack stack = slot.TakeOut(1);
            slot.MarkDirty();
            stack.Collectible.DamageItem(byEntity.World, byEntity, new DummySlot(stack));

            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
            byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/throw"), byEntity, byPlayer, false, 8);

            EntityProperties type = byEntity.World.GetEntityType(new AssetLocation(Attributes["spearEntityCode"].AsString()));
            Entity entity = byEntity.World.ClassRegistry.CreateEntity(type);
            ((EntityPoisonedProjectile)entity).FiredBy = byEntity;
            ((EntityPoisonedProjectile)entity).Damage = damage;
            ((EntityPoisonedProjectile)entity).ProjectileStack = stack;
            ((EntityPoisonedProjectile)entity).DropOnImpactChance = 1.1f;
            ((EntityPoisonedProjectile)entity).Weight = 0.3f;


            float acc = (1 - byEntity.Attributes.GetFloat("aimingAccuracy", 0));
            double rndpitch = byEntity.WatchedAttributes.GetDouble("aimingRandPitch", 1) * acc * 0.75;
            double rndyaw = byEntity.WatchedAttributes.GetDouble("aimingRandYaw", 1) * acc * 0.75;

            Vec3d pos = byEntity.ServerPos.XYZ.Add(0, byEntity.LocalEyePos.Y - 0.2, 0);

            Vec3d aheadPos = pos.AheadCopy(1, byEntity.ServerPos.Pitch + rndpitch, byEntity.ServerPos.Yaw + rndyaw);
            Vec3d velocity = (aheadPos - pos) * 0.65;
            Vec3d spawnPos = byEntity.ServerPos.BehindCopy(0.21).XYZ.Add(byEntity.LocalEyePos.X, byEntity.LocalEyePos.Y - 0.2, byEntity.LocalEyePos.Z);

            entity.ServerPos.SetPos(spawnPos);
            entity.ServerPos.Motion.Set(velocity);

            //byEntity.World.SpawnParticles(1, ColorUtil.WhiteArgb, spawnPos, spawnPos, new Vec3f(), new Vec3f(), 1.5f, 0, 1);



            entity.Pos.SetFrom(entity.ServerPos);
            entity.World = byEntity.World;
            ((EntityPoisonedProjectile)entity).SetRotation();

            byEntity.World.SpawnEntity(entity);
            byEntity.StartAnimation("throw");

            //RefillSlotIfEmpty(slot, byEntity);

            byPlayer?.Entity.World.PlaySoundAt(new AssetLocation("sounds/player/strike"), byPlayer.Entity, byPlayer, 0.9f + (float)api.World.Rand.NextDouble() * 0.2f, 16, 0.5f);
        }

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
                op.SinkSlot.Itemstack.Attributes.SetInt("augmentTime", op.SourceSlot.Itemstack.ItemAttributes["augmentTime"].AsInt(20));
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
