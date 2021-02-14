using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using Vintagestory.API.Server;
using System.Text;
using Vintagestory.API.Config;
using Newtonsoft.Json.Linq;

namespace Ailments
{
    public class EntityBehaviorCondsController : EntityBehavior
    {



        private ITreeAttribute condsTree;
        private TreeAttribute[] spontaneousDiseases = new TreeAttribute[0];
        private TreeAttribute[] attackDiseases = new TreeAttribute[0];
        private string[] carrierOf = new string[0];
        public bool mech;
        private Dictionary<string, TreeAttribute[]> injuries = new Dictionary<string, TreeAttribute[]>();
        EntityPartitioning entityfinder;
        public bool defenseless;

        //This keeps our time
        public double timeKeeper
        {
            get { return condsTree.GetDouble("timeKeeper"); }
            set { condsTree.SetDouble("timeKeeper", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How fast our condition updates
        public double speed
        {
            get { return condsTree.GetDouble("speed"); }
            set { condsTree.SetDouble("speed", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //The array that stores conditions
        public TreeArrayAttribute conds
        {
            get { return condsTree["conds"] as TreeArrayAttribute; }
            set { condsTree["conds"] = value; entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //Types of disease resistances
        public StringArrayAttribute resistances
        {
            get { return condsTree["resistances"] as StringArrayAttribute; }
            set { condsTree["resistances"] = value; entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //Diseases we are immune to
        public StringArrayAttribute immunities
        {
            get { return condsTree["immunities"] as StringArrayAttribute; }
            set { condsTree["immunities"] = value; entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //Diseases we can spread
        public StringArrayAttribute contagions
        {
            get { return condsTree["contagions"] as StringArrayAttribute; }
            set { condsTree["contagions"] = value; entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        #region Amounts
        //How much poison is in our system (Postive only)
        public int poisonAmount
        {
            get { return condsTree.GetInt("poisonAmount"); }
            set { condsTree.SetInt("poisonAmount", value < 0 ? 0 : value ); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How much we bounce back (Postive only)
        public int regenAmount
        {
            get { return condsTree.GetInt("regenAmount"); }
            set { condsTree.SetInt("regenAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How badly we are bleeding (Postive only)
        public int bleedAmount
        {
            get { return condsTree.GetInt("bleedAmount"); }
            set { condsTree.SetInt("bleedAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How fast we move
        public int speedAmount
        {
            get { return condsTree.GetInt("speedAmount"); }
            set { condsTree.SetInt("speedAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("walkspeed", "condController", value * 0.05f, true); }
        }

        //How healthy we are
        public int healthAmount
        {
            get { return condsTree.GetInt("healthAmount"); }
            set { condsTree.SetInt("healthAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How effective our healing is
        public int coagAmount
        {
            get { return condsTree.GetInt("coagAmount"); }
            set { condsTree.SetInt("coagAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("healingeffectivness", "condController", value * 0.05f, true); }
        }

        //How tolerant we are of poisons
        public int tolerateAmount
        {
            get { return condsTree.GetInt("tolerateAmount"); }
            set { condsTree.SetInt("tolerateAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("poisonresistance", "condController", value * 0.05f, true); }
        }

        //How much we eat
        public int hungerAmount
        {
            get { return condsTree.GetInt("hungerAmount"); }
            set { condsTree.SetInt("hungerAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("hungerrate", "condController", value * 0.05f, true); }
        }

        //How fast we aim
        public int aimAmount
        {
            get { return condsTree.GetInt("aimAmount"); }
            set { condsTree.SetInt("aimAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("rangedWeaponsSpeed", "condController", value * 0.05f, true); }
        }

        //How many times we can be ressurected
        public int reviveAmount
        {
            get { return condsTree.GetInt("reviveAmount"); }
            set { condsTree.SetInt("reviveAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How stable we are in the stream of time
        public int temporalAmount
        {
            get { return condsTree.GetInt("temporalAmount"); }
            set { condsTree.SetInt("temporalAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How tough we are
        public int defenseAmount
        {
            get { return condsTree.GetInt("defenseAmount"); }
            set { condsTree.SetInt("defenseAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How immune we are to disease
        public int immuneAmount
        {
            get { return condsTree.GetInt("immuneAmount"); }
            set { condsTree.SetInt("immuneAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How contagious we are
        public int contagiousAmount
        {
            get { return condsTree.GetInt("contagiousAmount"); }
            set { condsTree.SetInt("contagiousAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How we absorb enemy health (Postive only)
        public int absorbAmount
        {
            get { return condsTree.GetInt("absorbAmount"); }
            set { condsTree.SetInt("absorbAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How we reflect enemy damage (Postive only)
        public int recoilAmount
        {
            get { return condsTree.GetInt("recoilAmount"); }
            set { condsTree.SetInt("recoilAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //After death surprise (Postive only)
        public int revengeAmount
        {
            get { return condsTree.GetInt("revengeAmount"); }
            set { condsTree.SetInt("revengeAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //Player only amounts

        //How many gears we get
        public int gearAmount
        {
            get { return condsTree.GetInt("gearAmount"); }
            set { condsTree.SetInt("gearAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("rustyGearDropRate", "condController", value * 0.05f, true); }
        }

        //How hard we are on armor
        public int armorAmount
        {
            get { return condsTree.GetInt("armorAmount"); }
            set { condsTree.SetInt("armorAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("armorDurabilityLoss", "condController", value * 0.05f, true); }
        }

        //How sneaky we are
        public int hideAmount
        {
            get { return condsTree.GetInt("hideAmount"); }
            set { condsTree.SetInt("hideAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("animalSeekingRange", "condController", hideAmount * -0.05f, true); }
        }

        //How how much animal products we get and how fast we harvest
        public int huntAmount
        {
            get { return condsTree.GetInt("huntAmount"); }
            set { condsTree.SetInt("huntAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree");
                entity.Stats.Set("animalLootDropRate", "condController", value * 0.05f, true);
                entity.Stats.Set("animalHarvestingTime", "condController", value * -0.05f, true);
            }
        }

        //How much ore we get
        public int oreAmount
        {
            get { return condsTree.GetInt("oreAmount"); }
            set { condsTree.SetInt("oreAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("oreDropRate", "condController", value * 0.05f, true); }
        }


        //How much we forage
        public int forageAmount
        {
            get { return condsTree.GetInt("forageAmount"); }
            set { condsTree.SetInt("forageAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("forageDropRate", "condController", value * 0.05f, true); }
        }


        //How much we get from vessels
        public int vesselAmount
        {
            get { return condsTree.GetInt("vesselAmount"); }
            set { condsTree.SetInt("vesselAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("vesselContentsDropRate", "condController", value * 0.05f, true); }
        }

        //How much we get from crops
        public int cropAmount
        {
            get { return condsTree.GetInt("cropAmount"); }
            set { condsTree.SetInt("cropAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("wildCropDropRate", "condController", value * 0.05f, true); }
        }

        //How fast we mine
        public int mineAmount
        {
            get { return condsTree.GetInt("mineAmount"); }
            set { condsTree.SetInt("mineAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("miningSpeedMul", "condController", value * 0.05f, true); }
        }

        //How fast we throw projectiles
        public int bowAmount
        {
            get { return condsTree.GetInt("bowAmount"); }
            set { condsTree.SetInt("bowAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("bowDrawingStrength", "condController", value * 0.05f, true); }
        }

        // How accurate we are aiming
        public int accAmount
        {
            get { return condsTree.GetInt("accAmount"); }
            set { condsTree.SetInt("accAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("rangedWeaponsAcc", "condController", value * 0.05f, true); }
        }

        // How much ranged damage we do
        public int throwAmount
        {
            get { return condsTree.GetInt("throwAmount"); }
            set { condsTree.SetInt("throwAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("rangedWeaponsDamage", "condController", value * 0.05f, true); }
        }

        //How much melee damage we do
        public int punchAmount
        {
            get { return condsTree.GetInt("punchAmount"); }
            set { condsTree.SetInt("punchAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("meleeWeaponsDamage", "condController", value * 0.05f, true); }
        }

        //How immortal are we
        public int immortalAmount
        {
            get { return condsTree.GetInt("immortalAmount"); }
            set { condsTree.SetInt("immortalAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        // How well we eat our food
        public int digestAmount
        {
            get { return condsTree.GetInt("digestAmount"); }
            set { condsTree.SetInt("digestAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("digestion", "condController", value * 0.05f, true); }
        }

        //How well we dodge
        public int dodgeAmount
        {
            get { return condsTree.GetInt("dodgeAmount"); }
            set { condsTree.SetInt("dodgeAmount", value < 0 ? 0 : value); entity.WatchedAttributes.MarkPathDirty("condsTree"); }
        }

        //How well we break down machines
        public int mechAmount
        {
            get { return condsTree.GetInt("mechAmount"); }
            set { condsTree.SetInt("mechAmount", value); entity.WatchedAttributes.MarkPathDirty("condsTree"); entity.Stats.Set("mechanicalsDamage", "condController", value * 0.05f, true); }
        }
        #endregion





        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            condsTree = entity.WatchedAttributes.GetTreeAttribute("condsTree");

            if (properties.Attributes != null) mech = properties.Attributes.IsTrue("isMechanical"); else mech = false;
            defenseless = attributes.IsTrue("defenseless");
            entityfinder = entity.Api.ModLoader.GetModSystem<EntityPartitioning>();

            IAsset asset = entity.Api.Assets.Get(new AssetLocation("ailments:config/injuries.json"));
            JsonObject jasset = new JsonObject(JToken.Parse(asset.ToText()));
            injuries["burn"] = AilmentsUtil.CreateMultipleConds(jasset["burn"].AsArray());
            injuries["fall"] = AilmentsUtil.CreateMultipleConds(jasset["fall"].AsArray());
            injuries["slash"] = AilmentsUtil.CreateMultipleConds(jasset["slash"].AsArray());
            injuries["pierce"] = AilmentsUtil.CreateMultipleConds(jasset["pierce"].AsArray());
            injuries["blunt"] = AilmentsUtil.CreateMultipleConds(jasset["blunt"].AsArray());

            if (attributes["spontaneousDiseases"].AsArray() != null)
            {
                foreach (JsonObject cond in attributes["spontaneousDiseases"].AsArray())
                {
                    spontaneousDiseases = spontaneousDiseases.Concat(new TreeAttribute[] { AilmentsUtil.CreateTreeAttribute(cond) }).ToArray();
                }
            }

            if (attributes["attackDiseases"].AsArray() != null)
            {
                foreach (JsonObject cond in attributes["attackDiseases"].AsArray())
                {
                    attackDiseases = attackDiseases.Concat(new TreeAttribute[] { AilmentsUtil.CreateTreeAttribute(cond) }).ToArray();
                }
            }

            if (attributes["carrierOf"].AsArray<string>() != null)
            {
                carrierOf = attributes["carrierOf"].AsArray<string>();
            }

            if (condsTree == null)
            {
                Random rnd = entity.World.Rand;
                entity.WatchedAttributes.SetAttribute("condsTree", condsTree = new TreeAttribute());
                timeKeeper = entity.World.Calendar.TotalHours;
                speed = getSpeed( attributes["speed"], rnd);
                condsTree["conds"] = new TreeArrayAttribute(new TreeAttribute[0]);
                condsTree["resistances"] = new StringArrayAttribute(attributes["resistances"].AsArray<string>(new string[0]));
                condsTree["immunities"] = new StringArrayAttribute(attributes["immunities"].AsArray<string>(new string[0]));
                condsTree["contagions"] = new StringArrayAttribute(attributes["contagions"].AsArray<string>(new string[0]));
                poisonAmount = getStat( attributes["poisonAmount"], rnd);
                bleedAmount = getStat( attributes["bleedAmount"], rnd);
                regenAmount = getStat( attributes["regenAmount"], rnd);
                speedAmount = getStat( attributes["speedAmount"], rnd);
                healthAmount = getStat( attributes["healthAmount"], rnd);
                coagAmount = getStat( attributes["coagAmount"], rnd);
                hungerAmount = getStat( attributes["hungerAmount"], rnd);
                tolerateAmount = getStat( attributes["tolerateAmount"], rnd);
                reviveAmount = getStat( attributes["reviveAmount"], rnd);
                absorbAmount = getStat(attributes["absorbAmount"], rnd);
                defenseAmount = getStat(attributes["defenseAmount"], rnd);
                immuneAmount = getStat(attributes["immuneAmount"], rnd);
                contagiousAmount = getStat(attributes["contagiousAmount"], rnd);
                recoilAmount = getStat(attributes["recoilAmount"], rnd);
                revengeAmount = getStat(attributes["revengeAmount"], rnd);
                immortalAmount = getStat(attributes["immortalAmount"], rnd);
                aimAmount = getStat(attributes["aimAmount"], rnd);
                bowAmount = getStat(attributes["bowAmount"], rnd);
                accAmount = getStat(attributes["accAmount"], rnd);
                throwAmount = getStat(attributes["throwAmount"], rnd);
                punchAmount = getStat(attributes["punchAmount"], rnd);
                digestAmount = getStat(attributes["digestAmount"], rnd);
                hideAmount = getStat(attributes["hideAmount"], rnd);
                dodgeAmount = getStat(attributes["dodgeAmount"], rnd);
                mechAmount = getStat(attributes["mechAmount"], rnd);

                //Player only amounts
                temporalAmount = getStat(attributes["temporalAmount"], rnd);                
                gearAmount = getStat(attributes["gearAmount"], rnd);
                armorAmount = getStat(attributes["armorAmount"], rnd);
                huntAmount = getStat(attributes["huntAmount"], rnd);                
                mineAmount = getStat(attributes["mineAmount"], rnd);
                cropAmount = getStat(attributes["cropAmount"], rnd);
                oreAmount = getStat(attributes["oreAmount"], rnd);
                vesselAmount = getStat(attributes["vesselAmount"], rnd);
                forageAmount = getStat(attributes["forageAmount"], rnd);
                

                return;
            }

            timeKeeper = condsTree.GetDouble("timeKeeper");
            speed = condsTree.GetDouble("speed");
            poisonAmount = condsTree.GetInt("poisonAmount");
            bleedAmount = condsTree.GetInt("bleedAmount");
            regenAmount = condsTree.GetInt("regenAmount");
            speedAmount = condsTree.GetInt("speedAmount");
            healthAmount = condsTree.GetInt("healthAmount");
            coagAmount = condsTree.GetInt("coagAmount");
            hungerAmount = condsTree.GetInt("hungerAmount");
            tolerateAmount = condsTree.GetInt("tolerateAmount");
            reviveAmount = condsTree.GetInt("reviveAmount");
            absorbAmount = condsTree.GetInt("absorbAmount");
            defenseAmount = condsTree.GetInt("defenseAmount");
            immuneAmount = condsTree.GetInt("immuneAmount");
            contagiousAmount = condsTree.GetInt("contagiousAmount");
            recoilAmount = condsTree.GetInt("recoilAmount");
            revengeAmount = condsTree.GetInt("revengeAmount");
            immortalAmount = condsTree.GetInt("immortalAmount");
            aimAmount = condsTree.GetInt("aimAmount");
            bowAmount = condsTree.GetInt("bowAmount");
            accAmount = condsTree.GetInt("accAmount");
            punchAmount = condsTree.GetInt("punchAmount");
            throwAmount = condsTree.GetInt("throwAmount");
            digestAmount = condsTree.GetInt("digestAmount");
            hideAmount = condsTree.GetInt("hideAmount");
            dodgeAmount = condsTree.GetInt("dodgeAmount");
            mechAmount = condsTree.GetInt("mechAmount");

            // Player only amounts
            temporalAmount = condsTree.GetInt("temporalAmount");
            gearAmount = condsTree.GetInt("gearAmount");
            forageAmount = condsTree.GetInt("forageAmount");
            cropAmount = condsTree.GetInt("cropAmount");
            vesselAmount = condsTree.GetInt("vesselAmount");
            mineAmount = condsTree.GetInt("mineAmount");
            oreAmount = condsTree.GetInt("oreAmount");
            armorAmount = condsTree.GetInt("armorAmount");            
            huntAmount = condsTree.GetInt("huntAmount");
            

            conds = condsTree["conds"] as TreeArrayAttribute;
            resistances = condsTree["resistances"] as StringArrayAttribute;
            immunities = condsTree["immunities"] as StringArrayAttribute;
            contagions = condsTree["contagions"] as StringArrayAttribute;


            if (entity.Api.Side == EnumAppSide.Client)
            {
                ICoreClientAPI capi = entity.Api as ICoreClientAPI;

                GuiDialog dialog = new GuiDialogConditions(capi, this);

                capi.Input.RegisterHotKey("conditionsgui", "Displays current conditions", GlKeys.U, HotkeyType.GUIOrOtherControls);
                capi.Input.SetHotKeyHandler("conditionsgui", (KeyCombination comb) => {
                    if (dialog.IsOpened()) dialog.TryClose();
                    else dialog.TryOpen();

                    return true;
                });
            }

            if (!AilmentsConfig.Loaded.PersistentEffects || entity is EntityPlayer) timeKeeper = entity.World.Calendar.TotalHours;

        }

        public override void OnGameTick(float deltaTime)
        {
            if (entity.World.Calendar.TotalHours - timeKeeper < speed) return;
            timeKeeper += Math.Max(speed, 0);

            
            int condsNumber = conds.value.Length;
            for (int i = 0; i < condsNumber; i++)
            {
                updateCond(conds.value[i], i);
            }

            conds.value = conds.value.Where(x => x.GetString("name") != null).ToArray();
            if (AilmentsConfig.Loaded.DiseaseEnabled) spreadDisease();
            dealDamage();
            updateStability();

            //Sets up hunger for animals
            if (!(entity is EntityPlayer) && entity.WatchedAttributes.GetTreeAttribute("hunger") != null)
            {
               ITreeAttribute htree = entity.WatchedAttributes.GetTreeAttribute("hunger");
                htree.SetFloat("saturation", Math.Max(-10, htree.GetFloat("saturation") + (hungerAmount * -0.1f)));
            }

            //Random spontaneous disease
            if (spontaneousDiseases != null && spontaneousDiseases.Length > 0 && AilmentsConfig.Loaded.DiseaseEnabled) catchDisease(spontaneousDiseases[entity.World.Rand.Next(0, spontaneousDiseases.Length)]);
        }

        public void addCond(TreeAttribute posCond, double chanceMult = 1)
        {
            if (!entity.Alive || posCond == null) return;
            if (posCond.GetDouble("chance", 1) * chanceMult < entity.World.Rand.NextDouble()) return;
            if (canResist(posCond.GetString("type"), posCond.GetString("name"), posCond.GetBool("mech"))) return;
            if (!checkCond(posCond.GetString("name")) || !posCond.GetBool("combine"))
            {
                bool carry = canCarry(posCond.GetString("name"));
                TreeAttribute cond = posCond.Clone() as TreeAttribute;
                if (cond == null) return;

                cond["reproduce"] = cond.Clone() as TreeAttribute;

                int level = cond.GetInt("level", 1);
                int duration = cond.GetInt("duration", 1);
                if (!carry)
                {
                    TreeArrayAttribute effects = cond["effects"] as TreeArrayAttribute;
                    TreeAttribute[] elist = effects.value;
                    for (int i = 0; i < elist.Length; i++)
                    {
                        if (elist[i].GetString("name") == "contagious")
                        {
                            effects.value[i].SetString("addInfo", cond.GetString("name"));
                        }
                        if (initEffect(effects, i, level, duration)) cond.SetBool("active", true);
                    }
                }
                else
                {
                    if (cond.GetBool("contagiousCarrier"))
                    {
                        contagions.value = contagions.value.Concat(new string[] { cond.GetString("name") }).ToArray();
                    }

                    cond.SetBool("carry", carry);
                    cond.SetInt("carryDur", getDuration(cond, duration));
                }

                conds.value = conds.value.Concat(new TreeAttribute[] { cond }).ToArray();
                return;
            }
            else
            {
                TreeAttribute oCond = getCond(posCond.GetString("name"));


                if (posCond.GetInt("level") > oCond.GetInt("level") || posCond.GetInt("duration") > oCond.GetInt("duration"))
                {
                    int i = Array.IndexOf(conds.value, oCond);
                    removeCond(i, false);
                    conds.value[i] = replaceCond(posCond, 1, true);
                }
            }
            

        }

        public TreeAttribute replaceCond(TreeAttribute posCond, double chanceMult = 1, bool skipChance = false)
        {
            if (posCond == null) return new TreeAttribute();
            if (!skipChance && posCond.GetDouble("chance", 1) * chanceMult < entity.World.Rand.NextDouble()) return new TreeAttribute();
            if (canResist(posCond.GetString("type"), posCond.GetString("name"), posCond.GetBool("mech"))) return new TreeAttribute();
            if (!checkCond(posCond.GetString("name")) || !posCond.GetBool("combine"))
            {
                TreeAttribute cond = posCond.Clone() as TreeAttribute;
                bool carry = canCarry(posCond.GetString("name"));

                int level = cond.GetInt("level", 1);
                int duration = cond.GetInt("duration", 1);
                if (!carry)
                {
                    TreeArrayAttribute effects = cond["effects"] as TreeArrayAttribute;
                    TreeAttribute[] elist = effects.value;
                    for (int i = 0; i < elist.Length; i++)
                    {
                        if (elist[i].GetString("name") == "contagious")
                        {
                            effects.value[i].SetString("addInfo", cond.GetString("name"));
                        }
                        if (initEffect(effects, i, level, duration)) cond.SetBool("active", true);
                    }
                }
                else
                {
                    if (cond.GetBool("contagiousCarrier"))
                    {
                        contagions.value = contagions.value.Concat(new string[] { cond.GetString("name") }).ToArray();
                    }

                    cond.SetBool("carry", carry);
                    cond.SetInt("carryDur", getDuration(cond, duration));
                }


                return cond;
            }
            else
            {
                TreeAttribute oCond = getCond(posCond.GetString("name"));

                    if (posCond.GetInt("level") > oCond.GetInt("level") || posCond.GetInt("duration") > oCond.GetInt("duration"))
                    {
                        int i = Array.IndexOf(conds.value, oCond);
                        removeCond(i, false);
                        return replaceCond(posCond, 1 , true);
                    }
            }

            return new TreeAttribute();
        }

        public void removeCond(int index, bool secondary = true)
        {
            TreeAttribute cond = conds.value[index];
            if (!cond.GetBool("carry"))
            {
                if (cond["effects"] != null)
                {
                    TreeArrayAttribute effects = cond["effects"] as TreeArrayAttribute;
                    TreeAttribute[] earray = effects.value;
                    for (int i = 0; i < earray.Length; i++)
                    {
                        removeEffect(effects, i);
                    }
                }
            }
            else if (cond.GetBool("contagiousCarrier"))
            {
                contagions.value = contagions.value.Where((source, ind) => ind != Array.IndexOf(contagions.value, cond.GetString("name"))).ToArray();
            }

            if (secondary)
            {
                addCond(cond["nextCond"] as TreeAttribute, cond.GetDouble("nextCondChance", 1));
            }
            conds.value[index] = new TreeAttribute();
        }

        public void removeAll()
        {
            for (int i = 0; i < conds.value.Length; i++)
            {
                removeCond(i, false);
            }
            conds = new TreeArrayAttribute(new TreeAttribute[0]);
            MarkDirty();
        }

        public bool checkCond(string name)
        { 
           foreach (TreeAttribute condition in conds.value)
            {
                if (condition.GetString("name") == name) return true;
            }
            return false;
        }

        public TreeAttribute getCond(string name)
        {
            foreach (TreeAttribute condition in conds.value)
            {
                if (condition.GetString("name") == name) return condition;
            }

            return null;
        }

        public void updateCond(TreeAttribute cond, int index)
        {
            
            if (canResist(cond.GetString("type"), cond.GetString("name"), cond.GetBool("mech"))) { removeCond(index, false); return; }

            if (cond.GetBool("carry"))
            {
                int dur = cond.GetInt("carryDur");
                if (dur <= 0) { removeCond(index); return; }
                cond.SetInt("carryDur", dur - 1);
                return;
            }

            if (cond["effects"] != null)
            {
                TreeArrayAttribute effects = cond["effects"] as TreeArrayAttribute;
                for (int i = 0; i < effects.value.Length; i++)
                {
                    if (updateEffect(effects, i)) cond.SetBool("active", true);
                }

                effects.value = effects.value.Where(effect => effect.GetString("name") != null).ToArray();

                if (effects.value.Length == 0) removeCond(index);
            }
            else
            {
                removeCond(index);
            }

        }

        public bool updateEffect(TreeArrayAttribute effects, int index)
        {
            //Updates individual effect
            bool active = false;
            TreeAttribute effect = effects.value[index];
            string name = effect.GetString("name");
            int level = effect.GetInt("level");
            int start = effect.GetInt("start");
            int end = effect.GetInt("end");
            bool applied = effect.GetBool("applied");


            
            if (end <= 0)
            {
                
                removeEffect(effects, index);
                return false;
            }

            if (start > 0)
            {
                start -= 1;
                effect.SetInt("start", start);
                return false;
            }
            if (!applied)
            {
                updateCondition(name, level, true, effect.GetString("addInfo"));
                effect.SetBool("applied", true);
                active = true;
            }

            end -= 1;

            effect.SetInt("end", end);

            return active;
        }

        public void removeEffect(TreeArrayAttribute effects, int index)
        {
            // Removes effect
            TreeAttribute effect = effects.value[index];
            bool applied = effect.GetBool("applied");

            if (applied) updateCondition(effect.GetString("name"), effect.GetInt("level"), false, effect.GetString("addInfo"));

            effects.value[index] = new TreeAttribute();
        }

        public bool initEffect(TreeArrayAttribute effects, int index, int levelMult = 1, int durMult = 1)
        {
            //Multiplies the effect duration and level by condition level and duration
            TreeAttribute effect = effects.value[index];
            int level = effect.GetInt("level", 1) * levelMult;
            int start = effect.GetInt("start", 1);
            int end = effect.GetInt("end", 1) * durMult;
            string name = effect.GetString("name");

            if (end <= 0 && start <= 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Should never see this!!!!! {0} - {1} - {2} - {3} - {4}", name, level, start, end, effect.GetBool("applied")));
                effect.SetBool("applied", false);
                removeEffect(effects, index);
                return false;
            }

            if (end <= 0)
            {
                removeEffect(effects, index);
                return false;
            }

            effect.SetInt("level", level);
            effect.SetInt("start", start);
            effect.SetInt("end", end);

            if (start <= 0)
            {
                updateCondition(name, level, true, effect.GetString("addInfo"));
                effect.SetBool("applied", true);
                return true;
            }
            else
            {
                effect.SetBool("applied", false);
            }

            return false;
        }

        public bool lookupEffect(TreeArrayAttribute effects, string name)
        {
            foreach (TreeAttribute effect in effects.value)
            {
                if (effect.GetString("name") == name) return true;
            }
            return false;
        }

        public void updateCondition(string name, int level, bool add, string addInfo)
        {
            level = Math.Abs(level);
            switch (name)
            {
                case "poison":
                    if (add)
                    {
                        poisonAmount += level;
                    }
                    else
                    {
                        poisonAmount -= level;
                    }
                    break;
                case "regen":
                    if (add)
                    {
                        regenAmount += level;
                    }
                    else
                    {
                        regenAmount -= level;
                    }
                    break;
                case "bleed":
                    if (add)
                    {
                        bleedAmount += level;
                    }
                    else
                    {
                        bleedAmount -= level;
                    }
                    break;
                case "speed+":
                    if (add)
                    {
                        speedAmount += level;
                    }
                    else
                    {
                        speedAmount -= level;
                    }
                    
                    break;
                case "speed-":
                    if (add)
                    {
                        speedAmount -= level;
                    }
                    else
                    {
                        speedAmount += level;
                    }
                    
                    break;
                case "health+":
                    if (add)
                    {
                        healthAmount += level;
                    }
                    else
                    {
                        healthAmount -= level;
                    }
                    updateHealth();
                    break;
                case "health-":
                    if (add)
                    {
                        healthAmount -= level;
                    }
                    else
                    {
                        healthAmount += level;
                    }
                    updateHealth();
                    break;
                case "coag+":
                    if (add)
                    {
                        coagAmount += level;
                    }
                    else
                    {
                        coagAmount -= level;
                    }
                    
                    break;
                case "coag-":
                    if (add)
                    {
                        coagAmount -= level;
                    }
                    else
                    {
                        coagAmount += level;
                    }
                    
                    break;
                case "tolerate+":
                    if (add)
                    {
                        tolerateAmount += level;
                    }
                    else
                    {
                        tolerateAmount -= level;
                    }
                    
                    break;
                case "tolerate-":
                    if (add)
                    {
                        tolerateAmount -= level;
                    }
                    else
                    {
                        tolerateAmount += level;
                    }
                    
                    break;
                case "hunger+":
                    if (add)
                    {
                        hungerAmount += level;
                    }
                    else
                    {
                        hungerAmount -= level;
                    }
                    
                    break;
                case "hunger-":
                    if (add)
                    {
                        hungerAmount -= level;
                    }
                    else
                    {
                        hungerAmount += level;
                    }
                    
                    break;
                case "aim+":
                    if (add)
                    {
                        aimAmount += level;
                    }
                    else
                    {
                        aimAmount -= level;
                    }
                    
                    break;
                case "aim-":
                    if (add)
                    {
                        aimAmount -= level;
                    }
                    else
                    {
                        aimAmount += level;
                    }
                    
                    break;
                case "revive":
                    if (add)
                    {
                        reviveAmount += level;
                    }
                    else
                    {
                        reviveAmount -= level;
                    }
                    break;
                case "temporal+":
                    if (add)
                    {
                        temporalAmount += level;
                    }
                    else
                    {
                        temporalAmount -= level;
                    }
                    break;
                case "temporal-":
                    if (add)
                    {
                        temporalAmount -= level;
                    }
                    else
                    {
                        temporalAmount += level;
                    }
                    break;
                case "metabolism+":
                    //Increases speed of conditions
                    if (add)
                    {
                        speed -= level * 0.1;
                    }
                    else
                    {
                        speed += level * 0.1;
                    }
                    break;
                case "metabolism-":
                    //Decreases speed of conditions
                    if (add)
                    {
                        speed += level * 0.1;
                    }
                    else
                    {
                        speed -= level * 0.1;
                    }
                    break;
                case "absorb":
                    //Give health back on attack
                    if (add)
                    {
                        absorbAmount += level;
                    }
                    else
                    {
                        absorbAmount -= level;
                    }
                    break;
                case "defense+":
                    //Take less damage
                    if (add)
                    {
                        defenseAmount += level;
                    }
                    else
                    {
                        defenseAmount -= level;
                    }
                    break;
                case "defense-":
                    //Take more damage
                    if (add)
                    {
                        defenseAmount -= level;
                    }
                    else
                    {
                        defenseAmount += level;
                    }
                    break;
                case "resistance":
                    //Gain a resistance temporarily
                    if (addInfo == null) break;
                    if (add)
                    {
                        resistances.value = resistances.value.Concat(new string[] { addInfo}).ToArray();
                    }
                    else
                    {
                        resistances.value = resistances.value.Where((source, index) => index != Array.IndexOf(resistances.value, addInfo)).ToArray();
                    }
                    break;
                case "immunity":
                    //Gain a immunity temporarily
                    if (addInfo == null) break;
                    if (add)
                    {
                        immunities.value = immunities.value.Concat(new string[] { addInfo }).ToArray();
                    }
                    else
                    {
                        immunities.value = immunities.value.Where((source, index) => index != Array.IndexOf(immunities.value, addInfo)).ToArray();
                    }
                    break;
                case "contagious":
                    //Is able to spread to other vectors
                    if (addInfo == null) break;
                    if (add)
                    {
                        contagions.value = contagions.value.Concat(new string[] { addInfo }).ToArray();
                    }
                    else
                    {
                        contagions.value = contagions.value.Where((source, index) => index != Array.IndexOf(contagions.value, addInfo)).ToArray();
                    }
                    break;
                case "spread+":
                    if (add)
                    {
                        contagiousAmount += level;
                    }
                    else
                    {
                        contagiousAmount -= level;
                    }
                    break;
                case "spread-":
                    if (add)
                    {
                        contagiousAmount -= level;
                    }
                    else
                    {
                        contagiousAmount += level;
                    }
                    break;
                case "immune+":
                    if (add)
                    {
                        immuneAmount += level;
                        //System.Diagnostics.Debug.WriteLine("Adding");
                    }
                    else
                    {
                        immuneAmount -= level;
                        //System.Diagnostics.Debug.WriteLine("Remove");
                    }
                    break;
                case "immune-":
                    if (add)
                    {
                        immuneAmount -= level;
                    }
                    else
                    {
                        immuneAmount += level;
                    }
                    break;
                case "revenge":
                    if (add)
                    {
                        revengeAmount += level;
                    }
                    else
                    {
                        revengeAmount -= level;
                    }
                    break;
                case "recoil":
                    if (add)
                    {
                        recoilAmount += level;
                    }
                    else
                    {
                        recoilAmount -= level;
                    }
                    break;
                case "gear+":
                    if (add)
                    {
                        gearAmount += level;
                    }
                    else
                    {
                        gearAmount -= level;
                    }
                    
                    break;
                case "gear-":
                    if (add)
                    {
                        gearAmount -= level;
                    }
                    else
                    {
                        gearAmount += level;
                    }
                    
                    break;
                case "hunt+":
                    if (add)
                    {
                        huntAmount += level;
                    }
                    else
                    {
                        huntAmount -= level;
                    }
                    
                    break;
                case "hunt-":
                    if (add)
                    {
                        huntAmount -= level;
                    }
                    else
                    {
                        huntAmount += level;
                    }

                    break;
                case "forage+":
                    if (add)
                    {
                        forageAmount += level;
                    }
                    else
                    {
                        forageAmount -= level;
                    }
                    
                    break;
                case "forage-":
                    if (add)
                    {
                        forageAmount -= level;
                    }
                    else
                    {
                        forageAmount += level;
                    }
                    
                    break;
                case "crop+":
                    if (add)
                    {
                        cropAmount += level;
                    }
                    else
                    {
                        cropAmount -= level;
                    }
                    
                    break;
                case "crop-":
                    if (add)
                    {
                        cropAmount -= level;
                    }
                    else
                    {
                        cropAmount += level;
                    }
                    
                    break;
                case "ore+":
                    if (add)
                    {
                        oreAmount += level;
                    }
                    else
                    {
                        oreAmount -= level;
                    }
                    
                    break;
                case "ore-":
                    if (add)
                    {
                        oreAmount -= level;
                    }
                    else
                    {
                        oreAmount += level;
                    }
                    
                    break;
                case "mine+":
                    if (add)
                    {
                        mineAmount += level;
                    }
                    else
                    {
                        mineAmount -= level;
                    }
                    
                    break;
                case "mine-":
                    if (add)
                    {
                        mineAmount -= level;
                    }
                    else
                    {
                        mineAmount += level;
                    }
                    
                    break;
                case "hide":
                    if (add)
                    {
                        hideAmount += level;
                    }
                    else
                    {
                        hideAmount -= level;
                    }

                    break;
                case "immortal":
                    if (add)
                    {
                        immortalAmount += level;
                    }
                    else
                    {
                        immortalAmount -= level;
                    }

                    break;
                case "dodge":
                    if (add)
                    {
                        dodgeAmount += level;
                    }
                    else
                    {
                        dodgeAmount -= level;
                    }

                    break;
                case "vessel+":
                    if (add)
                    {
                        vesselAmount += level;
                    }
                    else
                    {
                        vesselAmount -= level;
                    }
                    
                    break;
                case "vessel-":
                    if (add)
                    {
                        vesselAmount -= level;
                    }
                    else
                    {
                        vesselAmount += level;
                    }
                    
                    break;
                case "bow+":
                    if (add)
                    {
                        bowAmount += level;
                    }
                    else
                    {
                        bowAmount -= level;
                    }

                    break;
                case "bow-":
                    if (add)
                    {
                        bowAmount -= level;
                    }
                    else
                    {
                        bowAmount += level;
                    }

                    break;
                case "acc+":
                    if (add)
                    {
                        accAmount += level;
                    }
                    else
                    {
                        accAmount -= level;
                    }

                    break;
                case "acc-":
                    if (add)
                    {
                        accAmount -= level;
                    }
                    else
                    {
                        accAmount += level;
                    }

                    break;
                case "armor+":
                    if (add)
                    {
                        armorAmount += level;
                    }
                    else
                    {
                        armorAmount -= level;
                    }
                    
                    break;
                case "armor-":
                    if (add)
                    {
                        armorAmount -= level;
                    }
                    else
                    {
                        armorAmount += level;
                    }
                    
                    break;
                case "throw+":
                    if (add)
                    {
                        throwAmount += level;
                    }
                    else
                    {
                        throwAmount -= level;
                    }

                    break;
                case "throw-":
                    if (add)
                    {
                        throwAmount -= level;
                    }
                    else
                    {
                        throwAmount += level;
                    }

                    break;
                case "punch+":
                    if (add)
                    {
                        punchAmount += level;
                    }
                    else
                    {
                        punchAmount -= level;
                    }

                    break;
                case "punch-":
                    if (add)
                    {
                        punchAmount -= level;
                    }
                    else
                    {
                        punchAmount += level;
                    }

                    break;
                case "digest+":
                    if (add)
                    {
                        digestAmount += level;
                    }
                    else
                    {
                        digestAmount -= level;
                    }

                    break;
                case "digest-":
                    if (add)
                    {
                        digestAmount -= level;
                    }
                    else
                    {
                        digestAmount += level;
                    }

                    break;
                case "mech+":
                    if (add)
                    {
                        mechAmount += level;
                    }
                    else
                    {
                        mechAmount -= level;
                    }

                    break;
                case "mech-":
                    if (add)
                    {
                        mechAmount -= level;
                    }
                    else
                    {
                        mechAmount += level;
                    }

                    break;
                default:
                    entity.World.Logger.Error("Not a valid effect: " + name);
                    break;
            }
        }

        public bool canResist(string type, string name, bool mechanical)
        {
            //Determines if we are resistant or immune to a condition

            if (mech != mechanical && !(type == "injury" || type == "envInjury")) return true;

            foreach (string resistance in resistances.value)
            {
                if (type == resistance) return true;
            }

            foreach (string immunity in immunities.value)
            {
                if (name == immunity) return true;
            }

            if (type == "injury" && entity is EntityPlayer)
            {
                IPlayer player = entity.World.PlayerByUid(((EntityPlayer)entity).PlayerUID);
                double chance = 0;
                IInventory inv = player.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName);

                if (!inv[12].Empty && (inv[12].Itemstack.Item is ItemWearable) && inv[12].Itemstack.ItemAttributes != null && inv[12].Itemstack.ItemAttributes["protectionModifiers"].Exists)
                {
                    chance += inv[12].Itemstack.ItemAttributes["protectionModifiers"]["relativeProtection"].AsDouble();
                }
                if (!inv[13].Empty && (inv[13].Itemstack.Item is ItemWearable) && inv[13].Itemstack.ItemAttributes != null && inv[13].Itemstack.ItemAttributes["protectionModifiers"].Exists)
                {
                    chance += inv[13].Itemstack.ItemAttributes["protectionModifiers"]["relativeProtection"].AsDouble();
                }
                if (!inv[14].Empty && (inv[14].Itemstack.Item is ItemWearable) && inv[14].Itemstack.ItemAttributes != null && inv[14].Itemstack.ItemAttributes["protectionModifiers"].Exists)
                {
                    chance += inv[14].Itemstack.ItemAttributes["protectionModifiers"]["relativeProtection"].AsDouble();
                }
                return (1 - chance/3) < entity.World.Rand.NextDouble();
            }

            return false;
        }

        public void dealDamage()
        {
            ITreeAttribute health = entity.WatchedAttributes.GetTreeAttribute("health");
            if (health == null) return;
            float maxhealth = health.GetFloat("maxhealth", 1);
            float healing = maxhealth * 0.05f * regenAmount;
            float damage = (maxhealth * 0.05f * bleedAmount / Math.Max(entity.Stats.GetBlended("healingeffectivness"), 0.01f)) + Math.Max((maxhealth * 0.05f * poisonAmount) - ((entity.Stats.GetBlended("poisonresistance") - 1) * (maxhealth * 0.05f * poisonAmount)), 0);

            if (healing > damage)
            {
                healing -= damage;
                entity.ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Internal, Type = EnumDamageType.Heal }, healing);
                
            }
            else if (healing < damage)
            {
                damage -= healing;
                entity.ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Internal, Type = EnumDamageType.Poison }, damage);

            }
        }

        public void spreadDisease()
        {
            string counted = "";
            foreach (string contagion in contagions.value)
            {
                TreeAttribute cond = getCond(contagion);

                if ((cond?["reproduce"] as TreeAttribute) == null || counted.Contains("|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|")) continue;

                entityfinder.WalkEntities(entity.ServerPos.XYZ, contagiousAmount + 4, (vector) =>
                {
                    if (vector.EntityId == entity.EntityId || !isVector(vector.Code.Path, cond.GetStringArray("vectors")) || vector.GetBehavior<EntityBehaviorCondsController>() == null) return true;
                    EntityBehaviorCondsController cc = vector.GetBehavior<EntityBehaviorCondsController>();
                    entity.World.RegisterCallback((dt) => { cc.catchDisease(cond); }, 500);
                    return true;
                });

                counted += "|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|";
            }
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

        public void catchDisease(TreeAttribute disease)
        {
            if (AilmentsConfig.Loaded.DiseaseEnabled) addCond(disease, disease.GetDouble("contagiousChance", 1) * Math.Max(0.5 - immuneAmount * 0.05, 0.05));
        }

        public void infectEnemy(Entity enemy)
        {
            EntityBehaviorCondsController ecc = enemy.GetBehavior<EntityBehaviorCondsController>();
            if (ecc == null) return;
            if (attackDiseases != null && attackDiseases.Length > 0)
            {
                foreach (TreeAttribute cond in attackDiseases)
                {
                    ecc.addCond(attackDiseases[entity.World.Rand.Next(0, attackDiseases.Length - 1)]);
                }

            }

            string counted = "";

            foreach (string contagion in contagions.value)
            {
                TreeAttribute cond = getCond(contagion);
                if (cond == null || !(cond["reproduce"] is TreeAttribute) || counted.Contains("|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|")) continue;

                ecc.catchDisease(cond["reproduce"] as TreeAttribute);
                counted += "|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|";
            }    
                
            
        }

        public float handleDefense(float dmg, DamageSource source)
        {
            if (source.Source == EnumDamageSource.Revive || source.Type == EnumDamageType.Heal) return dmg;
            if (source.Source != EnumDamageSource.Internal && dodgeAmount * 0.01 >= entity.World.Rand.NextDouble())
            {
                return 0f;
            }
            if (source.SourceEntity != null && source.SourceEntity.GetBehavior<EntityBehaviorCondsController>() != null)
            {
                EntityBehaviorCondsController cc = source.SourceEntity.GetBehavior<EntityBehaviorCondsController>();
                if (cc.absorbAmount > 0 || recoilAmount > 0) source.SourceEntity.ReceiveDamage(
                    new DamageSource() { Type = cc.absorbAmount > recoilAmount ? EnumDamageType.Heal : EnumDamageType.Poison },
                    cc.absorbAmount > recoilAmount? 0.05f * (cc.absorbAmount - recoilAmount) : 0.05f * (recoilAmount - cc.absorbAmount));
            }

            dmg = dmg - Math.Min(dmg, (dmg * 0.05f * defenseAmount));

            EntityBehaviorHealth bh = entity.GetBehavior<EntityBehaviorHealth>();

            if (bh != null) dmg = Math.Min(dmg, Math.Max(bh.Health - (bh.MaxHealth * 0.05f * immortalAmount), 0));

            return dmg;
        }

        public bool canCarry(string name)
        {
            foreach (string disease in carrierOf)
            {
                if (disease == name) return true;
            }

            return false;
        }

        public int getDuration(TreeAttribute cond, int duration)
        {
            int maxDur = 0;

            foreach (TreeAttribute effect in (cond["effects"] as TreeArrayAttribute).value)
            {
                maxDur = Math.Max(maxDur, effect.GetInt("start") + (effect.GetInt("end") * duration));
            }

            return maxDur;
        }

        public bool tryToRemoveCond(string name)
        {
            for (int i = 0; i < conds.value.Length; i++)
            {
                if (conds.value[i].GetString("name") == name)
                {
                    removeCond(i, false);
                    return true;
                }
            }

            return false;
        }

        public void updateHealth()
        {
            EntityBehaviorHealth he = entity.GetBehavior<EntityBehaviorHealth>();
            if (he == null) return;
            he.MaxHealthModifiers["condscontroller"] = healthAmount * 0.05f * he.BaseMaxHealth;
            he.MarkDirty();

        }

        public void updateStability()
        {
            double stability = entity.WatchedAttributes.GetDouble("temporalStability", -1);
            if (stability == -1) return;
            stability += temporalAmount * 0.05;
            entity.WatchedAttributes.SetDouble("temporalStability", stability);
        }

        public override void DidAttack(DamageSource source, EntityAgent targetEntity, ref EnumHandling handled)
        {
            if (!(entity is EntityPlayer) || targetEntity == null) return;

            EntityPlayer player = entity as EntityPlayer;
            ItemStack weapon = player.RightHandItemSlot.Itemstack;
            EntityBehaviorCondsController cc = targetEntity.GetBehavior<EntityBehaviorCondsController>();

            if (weapon == null || cc == null || weapon.ItemAttributes?["attackInjuries"] == null) return;

            JsonObject[] jconds = weapon.ItemAttributes["attackInjuries"].AsArray();
            if (jconds != null)
            {
                foreach (JsonObject jcond in jconds)
                {
                    cc.addCond(AilmentsUtil.CreateTreeAttribute(jcond));
                }
            }
            
            TreeAttribute[] aconds = (weapon.Attributes["attackInjuries"] as TreeArrayAttribute)?.value;
            if (aconds != null && weapon.Attributes.GetInt("augmentTime") > 0)
            {
                foreach (TreeAttribute cond in aconds)
                {
                    cc.addCond(cond);
                }
            }

            if (weapon.Attributes.GetInt("augmentTime") > 0) weapon.Attributes.SetInt("augmentTime", weapon.Attributes.GetInt("augmentTime") - 1);



        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            if (revengeAmount > 0 && entity.Api.Side ==EnumAppSide.Server)
            {
                ((IServerWorldAccessor)entity.World).CreateExplosion(entity.ServerPos.AsBlockPos, EnumBlastType.EntityBlast, revengeAmount + 4, revengeAmount + 8);
            }
            if (reviveAmount > 0)
            {
                entity.Revive();
            }

            removeAll();
            reviveAmount = 0;
        }

        public int getStat(JsonObject stat, Random rnd)
        {
            if (stat.AsInt() != 0) return stat.AsInt();
            int[] boost = stat.AsArray<int>();
            if (boost == null || boost.Length == 0) return 0;
            if (boost.Length == 1) return boost[0];
            return rnd.Next(Math.Min(boost[0], boost[1]), Math.Max(boost[0], boost[1]) + 1);
        }

        public double getSpeed(JsonObject speed, Random rnd)
        {
            if (speed.AsDouble() != 0) return speed.AsDouble();
            double[] boost = speed.AsArray<double>();
            if (boost == null || boost.Length == 0) return 1;
            if (boost.Length == 1) return boost[0];
            return Math.Min(boost[0], boost[1]) + (rnd.NextDouble() * (Math.Max(boost[0], boost[1]) - Math.Min(boost[0], boost[1])));
        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, float damage)
        {
            getInjuries(damage, damageSource.Type, damageSource.DamageTier);
            if (damageSource.SourceEntity != null)
            {
                Entity attacker = damageSource.SourceEntity;
                EntityBehaviorCondsController ecc = damageSource.SourceEntity.GetBehavior<EntityBehaviorCondsController>();

                if (ecc != null && AilmentsConfig.Loaded.DiseaseEnabled)
                {

                    if (defenseless) infectEnemy(attacker);

                    ecc.infectEnemy(entity);
                }
            }
        }

        public void getInjuries(float damage, EnumDamageType type, int tier)
        {
            if (entity.GetBehavior<EntityBehaviorTaskAI>()?.taskManager?.GetTask<AiTaskMeleeAttack>() != null)
            {
                tier = Math.Max( 1, tier - entity.GetBehavior<EntityBehaviorTaskAI>().taskManager.GetTask<AiTaskMeleeAttack>().damageTier);
            }
            else tier = tier > 0? tier : 1;
            
            switch(type)
            {
                case EnumDamageType.Gravity:
                    addCond(injuries["fall"][entity.World.Rand.Next(0, injuries["fall"].Length)], damage/50);
                    break;
                case EnumDamageType.PiercingAttack:
                    addCond(injuries["pierce"][entity.World.Rand.Next(0, injuries["pierce"].Length)], tier);
                    break;
                case EnumDamageType.BluntAttack:
                    addCond(injuries["blunt"][entity.World.Rand.Next(0, injuries["blunt"].Length)], tier);
                    break;
                case EnumDamageType.SlashingAttack:
                    addCond(injuries["slash"][entity.World.Rand.Next(0, injuries["slash"].Length)], tier);
                    break;
                case EnumDamageType.Fire:
                    addCond(injuries["burn"][entity.World.Rand.Next(0, injuries["burn"].Length)]);
                    break;
            }
        }

        public override void OnEntityLoaded()
        {
            updateHealth();
            

            EntityBehaviorHealth bh = entity.GetBehavior<EntityBehaviorHealth>();
            if (bh != null)
            {
                bh.onDamaged += (dmg, source) => handleDefense(dmg, source);
            }
        }

        public override void OnEntitySpawn()
        {
            updateHealth();
            

            EntityBehaviorHealth bh = entity.GetBehavior<EntityBehaviorHealth>();
            if (bh != null)
            {
                bh.onDamaged += (dmg, source) => handleDefense(dmg, source);
            }

            

        }

        public string condsDisplay()
        {
            StringBuilder dsc = new StringBuilder();

            
            dsc.AppendLine(Lang.Get("ailments:menu-metabolism", entity.WatchedAttributes.GetTreeAttribute("condsTree").GetDouble("speed") * 60));
            dsc.AppendLine(Lang.Get("ailments:menu-statstitle"));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("healthAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-healthAmount", GetStatColored("healthAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("coagAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-coagAmount", GetStatColored("coagAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("hungerAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-hungerAmount", GetStatColored("hungerAmount", 0.05f, true)));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("tolerateAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-tolerateAmount", GetStatColored("tolerateAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("speedAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-speedAmount", GetStatColored("speedAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("immuneAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-immuneAmount", GetStatColored("immuneAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("contagiousAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-contagiousAmount", GetStatColored("contagiousAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("defenseAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-defenseAmount", GetStatColored("defenseAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("punchAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-punchAmount", GetStatColored("punchAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("throwAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-throwAmount", GetStatColored("throwAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("temporalAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-temporalAmount", GetStatColored("temporalAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("mineAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-mineAmount", GetStatColored("mineAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("forageAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-forageAmount", GetStatColored("forageAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("huntAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-huntAmount", GetStatColored("huntAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("hideAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-hideAmount", GetStatColored("hideAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("cropAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-cropAmount", GetStatColored("cropAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("oreAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-oreAmount", GetStatColored("oreAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("gearAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-gearAmount", GetStatColored("gearAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("armorAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-armorAmount", GetStatColored("armorAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("accAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-accAmount", GetStatColored("accAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("aimAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-aimAmount", GetStatColored("aimAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("bowAmount") != 0) dsc.AppendLine(Lang.Get("ailments:menu-bowAmount", GetStatColored("bowAmount")));

            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("regenAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-regenAmount", GetStatColored("regenAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("bleedAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-bleedAmount", GetStatColored("bleedAmount", 0.05f, true)));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("poisonAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-poisonAmount", GetStatColored("poisonAmount", 0.05f, true)));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("recoilAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-recoilAmount", GetStatColored("recoilAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("absorbAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-absorbAmount", GetStatColored("absorbAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("revenegeAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-revengeAmount", GetStatColored("revengeAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("immortalAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-immortalAmount", GetStatColored("immortalAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("dodgeAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-dodgeAmount", GetStatColored("dodgeAmount")));
            if (entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt("reviveAmount") > 0) dsc.AppendLine(Lang.Get("ailments:menu-reviveAmount"));

            dsc.AppendLine();
            dsc.AppendLine(Lang.Get("ailments:menu-condstitle"));
            string counted = "";

            foreach (TreeAttribute cond in conds.value)
            {
                if (counted.Contains("|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|")) continue;
                int num = conds.value.Where((source, index) => source.GetString("name") == cond.GetString("name") && source.GetInt("level") == cond.GetInt("level") && source.GetInt("duration") == cond.GetInt("duration") && source.GetBool("active")).ToArray().Length;
                EnumGameMode mode = (entity is EntityPlayer) ? entity.World.PlayerByUid((entity as EntityPlayer).PlayerUID).WorldData.CurrentGameMode : EnumGameMode.Spectator;
                if ((cond.GetBool("active") && Lang.GetIfExists("ailments:conditionDisplay-" + cond.GetString("name")) != null) || mode == EnumGameMode.Creative)
                {
                    if (num > 1) dsc.AppendLine(Lang.Get("ailments:conditionDisplay-" + cond.GetString("name")) + " x" + num); else dsc.AppendLine(Lang.Get("ailments:conditionDisplay-" + cond.GetString("name")));
                    dsc.AppendLine(Lang.Get("ailments:menu-condslev", cond.GetInt("level"), cond.GetInt("duration")));
                    if (Lang.GetIfExists("ailments:conditionDisplayType-" + cond.GetString("type")) != null || mode == EnumGameMode.Creative) dsc.AppendLine(Lang.Get("ailments:conditionDisplayType-" + cond.GetString("type")));
                    dsc.AppendLine();
                    counted += "|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|";
                }
            }

            return dsc.ToString();
        }

        public string GetStatColored(string name, float perc = 0.05f, bool reverse = false)
        {
            int amount = entity.WatchedAttributes.GetTreeAttribute("condsTree").GetInt(name);
            string result = "";

            if (!reverse)
            {
                if (amount > 0) result = "+" + (amount * perc * 100) + "%";
                else if (amount < 0) result = (amount * perc * 100) + "%";
                else result = "0%";
            }
            else
            {
                if (amount > 0) result = "+" + (amount * perc * 100) + "%";
                else if (amount < 0) result = (amount * perc * 100) + "%";
                else result = "0%";
            }

            return result;
        }

        public void MarkDirty()
        {
            entity.WatchedAttributes.MarkPathDirty("condsTree");
        }

        public EntityBehaviorCondsController(Entity entity) : base(entity)
        {
        }

        public override string PropertyName()
        {
            return "condscontroller";
        }
    }
}
