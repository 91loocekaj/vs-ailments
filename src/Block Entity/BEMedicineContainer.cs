using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Ailments
{
    public class BEMedicineContainer : BlockEntityContainer
    {
        internal InventoryGeneric inv;
        public override InventoryBase Inventory => inv;
        public override string InventoryClassName => "medicinebottle";



        public BEMedicineContainer()
        {
            inv = new InventoryGeneric(2, null, null);
        }

        public override void OnBlockBroken()
        {
            //base.OnBlockBroken();
        }
    }
}
