using EzrnMoreTraps.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace EzrnMoreTraps.Items
{
    public class DartTrapLowTierItem : ModItem
    {
        public class DartTrapLowTierItemLoader : ILoadable
        {
            public void Load(Mod mod)
            {
                mod.AddContent(new DartTrapLowTierItem(0));
                mod.AddContent(new DartTrapLowTierItem(1));
                mod.AddContent(new DartTrapLowTierItem(2)); //third type for single stackable item
            }
            public void Unload() { }
        }
        protected override bool CloneNewInstances => true;
        private readonly int placeStyle;

        public override string Texture => "EzrnMoreTraps/Items/DartTrapLowTierItem";

        public override string Name => GetInternalNameFromStyle(placeStyle);

        public static string GetInternalNameFromStyle(int style)
        {
            if (style == 0)
                return "DartTrapLowTier_Left";
            else if (style == 1)
                return "DartTrapLowTier_Right";
            else if (style == 2)
            {
                return "DartTrapLowTier";
            }
            throw new Exception("Invalid style");
        }

        public DartTrapLowTierItem(int placeStyle)
        {
            this.placeStyle = placeStyle;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DartTrapLowTier>(), placeStyle);
            Item.width = 12;
            Item.height = 12;
            Item.value = 100;
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .Register();
        }


    }
}
