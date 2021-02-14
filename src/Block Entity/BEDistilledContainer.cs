using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BEDistilledContainer : BlockEntityContainer
    {
        internal InventoryGeneric inv;
        public override InventoryBase Inventory => inv;

        public override string InventoryClassName => "distillation";
        ICoreClientAPI capi;
        public float MeshAngle { get; set; }



        public BEDistilledContainer()
        {
            inv = new InventoryGeneric(1, null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            capi = api as ICoreClientAPI;
        }

        public void ReplaceBlock(IWorldAccessor world)
        {
            if (Inventory[0].Itemstack == null || Inventory[0].Itemstack.StackSize <= 0)
            {
                Block replacement = world.BlockAccessor.GetBlock(new AssetLocation("ailments:alembic-burned"));
                world.BlockAccessor.SetBlock(replacement.Id, Pos);
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetFloat("meshAngle", MeshAngle);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            MeshAngle = tree.GetFloat("meshAngle", MeshAngle);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            mesher.AddMeshData(capi.TesselatorManager.GetDefaultBlockMesh(Block).Clone().Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, MeshAngle, 0));
            return true;
        }
    }
}
