
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Ailments
{
    public class Ailments : ModSystem
    {

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            
            api.RegisterItemClass("ItemFirstAid", typeof(ItemFirstAid));
            api.RegisterItemClass("ItemEnhancedSpear", typeof(ItemEnhancedSpear));
            api.RegisterItemClass("ItemEnhancedBow", typeof(ItemEnhancedBow));
            api.RegisterItemClass("ItemEnhancedArrow", typeof(ItemEnhancedArrow));
            api.RegisterItemClass("ItemEnhancedWeapon", typeof(ItemEnhancedWeapon));
            api.RegisterItemClass("ItemDrug", typeof(ItemDrug));
            api.RegisterItemClass("ItemVaccine", typeof(ItemVaccine));

            api.RegisterBlockClass("BlockBottle", typeof(BlockBottle));
            api.RegisterBlockClass("BlockMedicineContainer", typeof(BlockMedicineContainer));
            api.RegisterBlockClass("BlockDistillingContainer", typeof(BlockDistillingContainer));
            api.RegisterBlockClass("BlockDistilledContainer", typeof(BlockDistilledContainer));
            

            api.RegisterBlockBehaviorClass("Dirty", typeof(BlockBehaviorDirty));
            api.RegisterBlockBehaviorClass("TemporalHarvest", typeof(BlockBehaviorTemporalHarvestable));

            api.RegisterBlockEntityBehaviorClass("Vector", typeof(BlockEntityBehaviorVector));

            api.RegisterBlockEntityClass("BEMedicineContainer", typeof(BEMedicineContainer));
            api.RegisterBlockEntityClass("BEDistilledContainer", typeof(BEDistilledContainer));
            

            api.RegisterEntityBehaviorClass("condscontroller", typeof(EntityBehaviorCondsController));
            api.RegisterEntityBehaviorClass("condsviewer", typeof(EntityBehaviorCondsViewer));

            api.RegisterEntity("EntityPoisonedProjectile", typeof(EntityPoisonedProjectile));
            

            
            AiTaskRegistry.Register<AiTaskTweakedMeleeAttack>("meleeattack");

            try
            {
                AilmentsConfig FromDisk;
                if ((FromDisk = api.LoadModConfig<AilmentsConfig>("D&DConfig.json")) == null)
                {
                    api.StoreModConfig<AilmentsConfig>(AilmentsConfig.Loaded, "D&DConfig.json");
                }
                else AilmentsConfig.Loaded = FromDisk;
            }
            catch
            {
                api.StoreModConfig<AilmentsConfig>(AilmentsConfig.Loaded, "D&DConfig.json");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public class ItemVaccine : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            EntityBehaviorCondsController cc;
            handling = EnumHandHandling.PreventDefault;
            if (byEntity.Api.Side != EnumAppSide.Server || slot.Itemstack.Attributes.GetBool("used")) return;


            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }


    }

    public static class AilmentsUtil
    {
        public static TreeAttribute CreateTreeAttribute(JsonObject cond, int exLevel = 1, int exDur = 1, int redStart = 1)
        {
            JsonObject[] effects = cond["effects"].AsArray();
            TreeAttribute pCond = cond.ToAttribute() as TreeAttribute;
            TreeAttribute[] pEffects = (pCond["effects"] as TreeArrayAttribute).value;
            if (cond["nextCond"].Exists) pCond["nextCond"] = CreateTreeAttribute(cond["nextCond"]);

            for (int i = 0; i < pEffects.Length; i++)
            {
                pEffects[i].SetInt("level", effects[i]["level"].AsInt());
                pEffects[i].SetInt("start", redStart <= 0? 0:effects[i]["start"].AsInt() / redStart);
                pEffects[i].SetInt("end", effects[i]["end"].AsInt());
            }

            pCond.SetInt("level", (int)pCond.GetLong("level", 0) * exLevel);
            pCond.SetInt("duration", (int)pCond.GetLong("duration", 0) * exDur);

            pCond["effects"] = new TreeArrayAttribute(pEffects);
            return pCond;

        }

        public static TreeAttribute[] CreateMultipleConds(JsonObject[] conds, int exLevel = 1, int exDur = 1, int redStart = 1)
        {
            if (conds == null || conds.Length == 0) return null;

            List<TreeAttribute> result = new List<TreeAttribute>();

            foreach (JsonObject cond in conds)
            {
                result.Add(CreateTreeAttribute(cond, exLevel, exDur, redStart));
            }

            return result.ToArray();
        }
    }

    public class AilmentsConfig
    {
        public static AilmentsConfig Loaded { get; set; } = new AilmentsConfig();
        public bool PersistentEffects { get; set; } = true;
        public bool DiseaseEnabled { get; set; } = true;
    }
}
