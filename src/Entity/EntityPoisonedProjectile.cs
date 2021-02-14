﻿using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Ailments
{
    public class EntityPoisonedProjectile : Entity
    {
        bool beforeCollided;
        bool stuck;

        long msLaunch;
        long msCollide;

        Vec3d motionBeforeCollide = new Vec3d();

        CollisionTester collTester = new CollisionTester();

        public Entity FiredBy;
        public float Weight = 0.1f;
        public float Damage;
        public ItemStack ProjectileStack;
        public float DropOnImpactChance = 0f;

        Cuboidf collisionTestBox;



        public override bool ApplyGravity
        {
            get { return !stuck; }
        }

        public override bool IsInteractable
        {
            get { return false; }
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            msLaunch = World.ElapsedMilliseconds;

            collisionTestBox = CollisionBox.Clone().OmniGrowBy(0.05f);
        }


        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
            if (ShouldDespawn) return;

            EntityPos pos = SidedPos;

            stuck = Collided || collTester.IsColliding(World.BlockAccessor, collisionTestBox, pos.XYZ);

            double impactSpeed = Math.Max(motionBeforeCollide.Length(), pos.Motion.Length());

            if (stuck)
            {
                IsColliding(pos, impactSpeed);
                return;
            }

            if (TryAttackEntity(impactSpeed))
            {
                return;
            }

            beforeCollided = false;
            motionBeforeCollide.Set(pos.Motion.X, pos.Motion.Y, pos.Motion.Z);
            SetRotation();
        }


        public override void OnCollided()
        {
            EntityPos pos = SidedPos;

            IsColliding(SidedPos, Math.Max(motionBeforeCollide.Length(), pos.Motion.Length()));
            motionBeforeCollide.Set(pos.Motion.X, pos.Motion.Y, pos.Motion.Z);
        }


        private void IsColliding(EntityPos pos, double impactSpeed)
        {
            pos.Motion.Set(0, 0, 0);

            if (!beforeCollided && World is IServerWorldAccessor && World.ElapsedMilliseconds > msCollide + 250)
            {
                if (impactSpeed >= 0.07)
                {
                    World.PlaySoundAt(new AssetLocation("sounds/arrow-impact"), this, null, false, 32);

                    // Slighty randomize orientation to make it a bit more realistic
                    //pos.Yaw += (float)(World.Rand.NextDouble() * 0.05 - 0.025);
                    //pos.Roll += (float)(World.Rand.NextDouble() * 0.05 - 0.025);

                    // Resend position to client
                    WatchedAttributes.MarkAllDirty();

                    int leftDurability = ProjectileStack == null ? 1 : ProjectileStack.Attributes.GetInt("durability", 1);
                    if (leftDurability <= 0)
                    {
                        Die();
                    }
                }

                TryAttackEntity(impactSpeed);

                msCollide = World.ElapsedMilliseconds;

                beforeCollided = true;
            }


        }


        bool TryAttackEntity(double impactSpeed)
        {
            if (World is IClientWorldAccessor || World.ElapsedMilliseconds <= msCollide + 250) return false;
            if (impactSpeed <= 0.01) return false;

            EntityPos pos = SidedPos;

            Cuboidd projectileBox = CollisionBox.ToDouble().Translate(ServerPos.X, ServerPos.Y, ServerPos.Z);

            // We give it a bit of extra leeway of 50% because physics ticks can run twice or 3 times in one game tick 
            if (ServerPos.Motion.X < 0) projectileBox.X1 += 1.5 * ServerPos.Motion.X;
            else projectileBox.X2 += 1.5 * ServerPos.Motion.X;
            if (ServerPos.Motion.Y < 0) projectileBox.Y1 += 1.5 * ServerPos.Motion.Y;
            else projectileBox.Y2 += 1.5 * ServerPos.Motion.Y;
            if (ServerPos.Motion.Z < 0) projectileBox.Z1 += 1.5 * ServerPos.Motion.Z;
            else projectileBox.Z2 += 1.5 * ServerPos.Motion.Z;

            Entity entity = World.GetNearestEntity(ServerPos.XYZ, 5f, 5f, (e) => {
                if (e.EntityId == this.EntityId || !e.IsInteractable) return false;

                if (FiredBy != null && e.EntityId == FiredBy.EntityId && World.ElapsedMilliseconds - msLaunch < 500)
                {
                    return false;
                }

                Cuboidd eBox = e.CollisionBox.ToDouble().Translate(e.ServerPos.X, e.ServerPos.Y, e.ServerPos.Z);

                return eBox.IntersectsOrTouches(projectileBox);
            });

            if (entity != null)
            {
                IServerPlayer fromPlayer = null;
                if (FiredBy is EntityPlayer)
                {
                    fromPlayer = (FiredBy as EntityPlayer).Player as IServerPlayer;
                }

                bool targetIsPlayer = entity is EntityPlayer;
                bool targetIsCreature = entity is EntityAgent;
                bool canDamage = true;

                ICoreServerAPI sapi = World.Api as ICoreServerAPI;
                if (fromPlayer != null)
                {
                    if (targetIsPlayer && (!sapi.Server.Config.AllowPvP || !fromPlayer.HasPrivilege("attackplayers"))) canDamage = false;
                    if (targetIsCreature && !fromPlayer.HasPrivilege("attackcreatures")) canDamage = false;
                }

                msCollide = World.ElapsedMilliseconds;
                World.PlaySoundAt(new AssetLocation("sounds/arrow-impact"), this, null, false, 24);

                if (canDamage)
                {
                    float dmg = Damage;
                    dmg *= FiredBy.Stats.GetBlended("rangedWeaponsDamage");

                    bool didDamage = entity.ReceiveDamage(new DamageSource()
                    {
                        Source = EnumDamageSource.Entity,
                        SourceEntity = FiredBy == null ? this : FiredBy,
                        Type = EnumDamageType.PiercingAttack
                    }, dmg);

                    EntityBehaviorCondsController cc;

                    if (didDamage && ProjectileStack?.Attributes != null && (cc = entity.GetBehavior<EntityBehaviorCondsController>()) != null)
                    {
                        ITreeAttribute attr = ProjectileStack.Attributes;

                        if (attr["attackInjuries"] != null)
                        {
                            TreeAttribute[] injs = (attr["attackInjuries"] as TreeArrayAttribute).value;
                            foreach (TreeAttribute inj in injs)
                            {
                                cc.addCond(inj);
                            }
                        }

                        if (attr["arrowPoison"] != null)
                        {
                            TreeAttribute[] conds = (attr["arrowPoison"] as TreeArrayAttribute).value;
                            foreach (TreeAttribute cond in conds)
                            {
                                cc.addCond(cond);
                            }
                        }
                    }

                    float kbresist = entity.Properties.KnockbackResistance;
                    entity.SidedPos.Motion.Add(kbresist * pos.Motion.X * Weight, kbresist * pos.Motion.Y * Weight, kbresist * pos.Motion.Z * Weight);

                    int leftDurability = ProjectileStack == null ? 1 : ProjectileStack.Attributes.GetInt("durability", 1);

                    if (World.Rand.NextDouble() < DropOnImpactChance && leftDurability > 0)
                    {
                        pos.Motion.Set(0, 0, 0);
                    }
                    else
                    {
                        Die();
                    }

                    if (FiredBy is EntityPlayer && didDamage)
                    {
                        World.PlaySoundFor(new AssetLocation("sounds/player/projectilehit"), (FiredBy as EntityPlayer).Player, false, 24);
                    }


                }
                else
                {
                    pos.Motion.Set(0, 0, 0);
                }

                return true;
            }


            return false;
        }


        public virtual void SetRotation()
        {
            EntityPos pos = (World is IServerWorldAccessor) ? ServerPos : Pos;

            double speed = pos.Motion.Length();

            if (speed > 0.01)
            {
                pos.Pitch = 0;
                pos.Yaw =
                    GameMath.PI + (float)Math.Atan2(pos.Motion.X / speed, pos.Motion.Z / speed)
                    + GameMath.Cos((World.ElapsedMilliseconds - msLaunch) / 200f) * 0.03f
                ;
                pos.Roll =
                    -(float)Math.Asin(GameMath.Clamp(-pos.Motion.Y / speed, -1, 1))
                    + GameMath.Sin((World.ElapsedMilliseconds - msLaunch) / 200f) * 0.03f
                ;
            }
        }


        public override bool CanCollect(Entity byEntity)
        {
            return Alive && World.ElapsedMilliseconds - msLaunch > 1000 && ServerPos.Motion.Length() < 0.01;
        }

        public override ItemStack OnCollected(Entity byEntity)
        {
            ProjectileStack.ResolveBlockOrItem(World);
            return ProjectileStack;
        }


        public override void OnCollideWithLiquid()
        {
            base.OnCollideWithLiquid();
        }

        public override void ToBytes(BinaryWriter writer, bool forClient)
        {
            base.ToBytes(writer, forClient);
            writer.Write(beforeCollided);
            ProjectileStack.ToBytes(writer);
        }

        public override void FromBytes(BinaryReader reader, bool fromServer)
        {
            base.FromBytes(reader, fromServer);
            beforeCollided = reader.ReadBoolean();
            ProjectileStack = new ItemStack(reader);
        }
    }
}
