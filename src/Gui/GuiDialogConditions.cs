using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace Ailments
{
    public class GuiDialogConditions : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "conditionsgui";
        private EntityBehaviorCondsController conds;

        public GuiDialogConditions(ICoreClientAPI capi, EntityBehavior cc) : base(capi)
        {
            conds = cc as EntityBehaviorCondsController;
            SetupDialog();
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightMiddle);

            // Just a simple 300x300 pixel box
            ElementBounds textBounds = ElementBounds.Fixed(0, 0, 300, 260);
            ElementBounds scrollBounds = ElementBounds.Fixed(350, 40, 20, 250);

            ElementBounds testBounds = ElementBounds.Fixed(0, 20, 370, 250);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(testBounds);

            

            SingleComposer = capi.Gui.CreateCompo("conditionsDialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddVerticalScrollbar(onScroll, scrollBounds, "mainScroll")
                .AddDialogTitleBar("Status & Conditions", OnTitleBarCloseClicked)
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

        private void onScroll (float scroll)
        {
            
            GuiElementRichtext text = SingleComposer.GetRichtext("condsText");
            GuiElementScrollbar bar = SingleComposer.GetScrollbar("mainScroll");

            text.Bounds.fixedY =  Math.Min(150 - scroll, 0f);
            text.Bounds.CalcWorldBounds();
        }

        public override bool TryOpen()
        {
            bool def = base.TryOpen();
            if (def)
            {
                GuiElementRichtext text = SingleComposer.GetRichtext("condsText");
                GuiElementScrollbar bar = SingleComposer.GetScrollbar("mainScroll");

                conds.MarkDirty();
                text.SetNewText(conds.condsDisplay(), CairoFont.WhiteDetailText());
                text.CalcHeightAndPositions();
                bar.SetHeights(100f, (float)text.Bounds.fixedHeight);
            }

            return def;
        }
    }


}
