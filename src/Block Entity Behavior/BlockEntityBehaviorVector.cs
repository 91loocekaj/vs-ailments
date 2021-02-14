using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Ailments
{
    public class BlockEntityBehaviorVector : BlockEntityBehavior
    {
        EntityPartitioning entityUtil;
        double timer;

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            entityUtil = api.ModLoader.GetModSystem<EntityPartitioning>();
            timer = timer == 0 ? Api.World.Calendar.TotalHours : timer;
            Api.World.RegisterGameTickListener(spreadDisease, 3000);
        }

        public void spreadDisease(float dt)
        {
            if (Api.World.Calendar.TotalHours - timer < 1) return;
            timer += 1;

            JsonObject[] jcos = properties["diseases"].AsArray();
            if (jcos == null || jcos.Length == 0) return;

            entityUtil.WalkEntities(Blockentity.Pos.ToVec3d(), properties["spreadRange"].AsDouble(6), (ent) =>
            {
                EntityBehaviorCondsController ecc;
                if ((ecc = ent.GetBehavior<EntityBehaviorCondsController>()) == null) return false;

                foreach (JsonObject jco in jcos)
                {
                    if (isVector(ent.Code.Path, jco["vectors"].AsArray<string>())) ecc.catchDisease(AilmentsUtil.CreateTreeAttribute(jco));
                }

                return false;
            });
        }

        public bool isVector(string contact, string[] vectors)
        {
            if (vectors == null || vectors.Length == 0) return false;
            foreach (string vector in vectors)
            {
                if (vector.EndsWith("*"))
                {
                    if (contact.StartsWith(vector.Substring(0, vector.Length - 1))) return true;
                }
                else if (contact == vector) return true;
            }
            return false;
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetDouble("timer", timer);
        }

        public override void FromTreeAtributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAtributes(tree, worldAccessForResolve);
            timer = tree.GetDouble("timer");
        }

        public BlockEntityBehaviorVector(BlockEntity blockentity) : base(blockentity)
        {
        }
    }
}
