using System;
using System.Linq;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Ailments
{
    public class EntityBehaviorCondsViewer : EntityBehavior
    {

        bool mech;
        GuiDialogStatus dialog;

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (properties.Attributes != null) mech = properties.Attributes.IsTrue("isMechanical"); else mech = false;
        }

        public override void GetInfoText(StringBuilder infotext)
        {
            if (entity.WatchedAttributes["condsTree"] == null) return;

            if (entity.Code.Path.Contains("drifter") && (entity.WatchedAttributes["condsTree"] as TreeAttribute).GetInt("recoilAmount") != 0)
            {
                infotext.AppendLine(Lang.Get("ailments:bioDisplay-recoil"));
            }

            if (entity.Code.Path.Contains("locust") && (entity.WatchedAttributes["condsTree"] as TreeAttribute).GetInt("reviveAmount") != 0)
            {
                infotext.AppendLine(Lang.Get("ailments:mechDisplay-revive"));
            }

            if (entity.Code.Path.Contains("bell") && (entity.WatchedAttributes["condsTree"] as TreeAttribute).GetInt("revengeAmount") != 0)
            {
                infotext.AppendLine(Lang.Get("ailments:mechDisplay-revenge"));
            }

            if ((entity.Code.Path.Contains("hyena") || entity.Code.Path.Contains("wolf")) && (entity.WatchedAttributes["condsTree"] as TreeAttribute).GetInt("absorbAmount") > 0)
            {
                infotext.AppendLine(Lang.Get("ailments:bioDisplay-absorb"));
            }

            TreeAttribute[] conds = ((entity.WatchedAttributes["condsTree"] as TreeAttribute)["conds"] as TreeArrayAttribute).value;

            foreach(TreeAttribute cond in conds)
            {
                if (cond.GetString("type") == "viral" || cond.GetString("type") == "bacterial" || cond.GetString("type") == "fungal" || cond.GetString("type") == "parasitic")
                {
                    infotext.AppendLine(Lang.Get("ailments:bioDisplay-diseased"));
                }
            }

            /*string counted = "";

            foreach(TreeAttribute cond in conds)
            {
                if (mech)
                {
                    if (Lang.GetIfExists("ailments:mechDisplay-" + cond.GetString("name")) != null && !counted.Contains("|" + cond.GetString("name") + "|"))
                    {
                        int num = conds.Where((source, index) => cond.GetString("name") == source.GetString("name")).ToArray().Length;
                        if (num > 0) infotext.AppendLine(Lang.Get("ailments:mechDisplay-" + cond.GetString("name")) + " x" + num); else infotext.AppendLine(Lang.Get("ailments:mechDisplay-" + cond.GetString("name")));
                        counted += "|" + cond.GetString("name") + "|";
                    }
                }
                else
                {
                    if (Lang.GetIfExists("ailments:bioDisplay-" + cond.GetString("name")) != null && !counted.Contains("|" + cond.GetString("name") + "|"))
                    {
                        int num = conds.Where((source, index) => cond.GetString("name") == source.GetString("name")).ToArray().Length;
                        if (num > 0) infotext.AppendLine(Lang.Get("ailments:bioDisplay-" + cond.GetString("name")) + " x" + num); else infotext.AppendLine(Lang.Get("ailments:bioDisplay-" + cond.GetString("name")));
                        counted += "|" + cond.GetString("name") + "|";
                    }
                }
            }*/

        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode, ref EnumHandling handled)
        {
            base.OnInteract(byEntity, itemslot, hitPosition, mode, ref handled);
            if (mode == EnumInteractMode.Attack || itemslot.Itemstack?.Item?.Code?.Path != "lense" || !entity.Alive) return; 
            if (dialog == null) dialog = new GuiDialogStatus((ICoreClientAPI)entity.Api, this);
            if (entity.Api.Side == EnumAppSide.Client) if (dialog.IsOpened()) dialog.TryClose(); else dialog.TryOpen();
        }

        public string GetMagnified()
        {
            StringBuilder dsc = new StringBuilder();


            dsc.AppendLine(Lang.Get("ailments:menu-metabolism", entity.WatchedAttributes.GetTreeAttribute("condsTree").GetDouble("speed") * 60));
            dsc.AppendLine(Lang.Get("ailments:menu-statstitle2"));
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

            TreeAttribute[] conds = ((entity.WatchedAttributes["condsTree"] as TreeAttribute)["conds"] as TreeArrayAttribute).value;

            dsc.AppendLine();
            dsc.AppendLine(Lang.Get("ailments:menu-condstitle"));
            string counted = "";

            foreach (TreeAttribute cond in conds)
            {
                if (counted.Contains("|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|")) continue;
                int num = conds.Where((source, index) => source.GetString("name") == cond.GetString("name") && source.GetInt("level") == cond.GetInt("level") && source.GetInt("duration") == cond.GetInt("duration") && source.GetBool("active")).ToArray().Length;
                
                if ((cond.GetBool("active") && Lang.GetIfExists("ailments:conditionDisplay-" + cond.GetString("name")) != null))
                {
                    if (num > 1) dsc.AppendLine(Lang.Get("ailments:conditionDisplay-" + cond.GetString("name")) + " x" + num); else dsc.AppendLine(Lang.Get("ailments:conditionDisplay-" + cond.GetString("name")));
                    dsc.AppendLine(Lang.Get("ailments:menu-condslev", cond.GetInt("level"), cond.GetInt("duration")));
                    if (Lang.GetIfExists("ailments:conditionDisplayType-" + cond.GetString("type")) != null) dsc.AppendLine(Lang.Get("ailments:conditionDisplayType-" + cond.GetString("type")));
                    dsc.AppendLine();
                    counted += "|" + cond.GetString("name") + cond.GetInt("level") + cond.GetInt("duration") + "|";
                }
            }

            return dsc.ToString();

        }

        public string GetStatColored(string name, float perc = 0.05f,bool reverse = false)
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

        public EntityBehaviorCondsViewer(Entity entity) : base(entity)
        {
        }

        public override string PropertyName()
        {
           return "condsviewer";
        }
    }

    public class GuiDialogStatus : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "statusgui";
        private EntityBehaviorCondsViewer conds;

        public GuiDialogStatus(ICoreClientAPI capi, EntityBehavior cc) : base(capi)
        {
            conds = cc as EntityBehaviorCondsViewer;
            SetupDialog();
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            // Just a simple 300x300 pixel box
            ElementBounds textBounds = ElementBounds.Fixed(0, 0, 300, 260);
            ElementBounds scrollBounds = ElementBounds.Fixed(350, 40, 20, 250);

            ElementBounds testBounds = ElementBounds.Fixed(0, 20, 370, 250);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(testBounds);



            SingleComposer = capi.Gui.CreateCompo("statusDialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddVerticalScrollbar(onScroll, scrollBounds, "mainScroll")
                .AddDialogTitleBar("Current Status", OnTitleBarCloseClicked)
                .BeginChildElements()
                .BeginClip(testBounds)
                .AddRichtext("", CairoFont.WhiteDetailText(), textBounds, "condsText")
                .EndClip()
                .EndChildElements()
                .Compose()
            ;
            GuiElementScrollbar bar = SingleComposer.GetScrollbar("mainScroll");
            GuiElementRichtext text = SingleComposer.GetRichtext("condsText");


            bar.SetHeights(100f, (float)text.Bounds.fixedHeight);


        }

        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }

        private void onScroll(float scroll)
        {

            GuiElementRichtext text = SingleComposer.GetRichtext("condsText");
            GuiElementScrollbar bar = SingleComposer.GetScrollbar("mainScroll");

            text.Bounds.fixedY = Math.Min(150 - scroll, 0f);
            text.Bounds.CalcWorldBounds();
        }

        public override bool TryOpen()
        {
            bool def = base.TryOpen();
            if (def)
            {
                GuiElementRichtext text = SingleComposer.GetRichtext("condsText");
                GuiElementScrollbar bar = SingleComposer.GetScrollbar("mainScroll");

                text.SetNewText(conds.GetMagnified(), CairoFont.WhiteDetailText());
                text.CalcHeightAndPositions();
                bar.SetHeights(100f, (float)text.Bounds.fixedHeight);
            }

            return def;
        }


    }
}
