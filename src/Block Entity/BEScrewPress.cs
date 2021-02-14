using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Ailments
{
    class BEScrewPress : BlockEntityContainer
    {
        public override string InventoryClassName => "screwpress";

        public override InventoryBase Inventory => inv;
        internal InventoryGeneric inv;

        public ItemSlot inputSlot { get { return inv[0]; } }
        public ItemSlot outputSlot { get { return inv[1]; } }
        public int maxCap = 40;
        public bool isSqueezing;
        public double squeezeUntil;
        public float MeshAngle { get; set; }
        ICoreClientAPI capi;



        public BEScrewPress()
        {
            inv = new InventoryGeneric(2, null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            RegisterGameTickListener(OnGameTick, 100);
            if (api.Side != EnumAppSide.Client) return;
            capi = api as ICoreClientAPI;

            
        }

        public void OnGameTick(float dt)
        {
            releasePressure();
        }

        public bool TryAdd(ItemSlot slot, int quantity = 1)
        {
            if (isSqueezing || outputSlot.StackSize > 0) return false;

            if (!slot.Itemstack.ItemAttributes["squeezeInto"].Exists) return false;
            int moveq = Math.Min(quantity, maxCap - inputSlot.StackSize);
            int originalSize = slot.StackSize;


            slot.TryPutInto(Api.World, inputSlot, moveq);
            MarkDirty();
            return originalSize != slot.StackSize;
        }

        public bool usePress(IPlayer player = null)
        {
            if (isSqueezing || outputSlot.StackSize > 0 || inputSlot.StackSize <= 0) return false;

            int ratio = inputSlot.Itemstack.ItemAttributes["squeezeRatio"].AsInt();
            if (ratio > inputSlot.StackSize)
            {
                inv.DropSlots(Pos.ToVec3d(), new int[1] {0});
                return false;
            }

            isSqueezing = true;
            squeezeUntil = Api.World.Calendar.TotalHours + 4;
            Api.World.PlaySoundAt(new AssetLocation("game:sounds/block/creak/woodcreak_long3.ogg"), Pos.X, Pos.Y, Pos.Z);
            return true;
            
        }

        public void releasePressure()
        {
            if (squeezeUntil > Api.World.Calendar.TotalHours || !isSqueezing) return;

            outputSlot.Itemstack = new ItemStack(Api.World.GetItem(new AssetLocation(inputSlot.Itemstack.ItemAttributes["squeezeInto"].AsString("game:waterportion"))), inputSlot.StackSize / inputSlot.Itemstack.ItemAttributes["squeezeRatio"].AsInt(1));
            inputSlot.TakeOut(inputSlot.StackSize - (inputSlot.StackSize % inputSlot.Itemstack.ItemAttributes["squeezeRatio"].AsInt(1)));
            isSqueezing = false;
            squeezeUntil = 0;
        }

        public void dropLeftovers()
        {
            inv.DropSlots(Pos.ToVec3d(), new int[1] { 0 });
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetDouble("squeezeUntil", squeezeUntil);
            tree.SetFloat("meshAngle", MeshAngle);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            squeezeUntil = tree.GetDouble("squeezeUntil");
            isSqueezing = squeezeUntil > 0;
            MeshAngle = tree.GetFloat("meshAngle", MeshAngle);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            mesher.AddMeshData(capi.TesselatorManager.GetDefaultBlockMesh(Block).Clone().Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, MeshAngle, 0));
            return true;
        }

    }
}
