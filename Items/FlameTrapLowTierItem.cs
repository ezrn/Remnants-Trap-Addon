using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EzrnMoreTraps.Items
{
    public class FlameTrapLowTierItem : ModItem
    {
        public class FlameTrapLowTierItemLoader : ILoadable
        {
            public void Load(Mod mod)
            {
                mod.AddContent(new FlameTrapLowTierItem(0));
                mod.AddContent(new FlameTrapLowTierItem(1));
                mod.AddContent(new FlameTrapLowTierItem(2)); //need third type for stackable single item
            }
            public void Unload() { }
        }
        protected override bool CloneNewInstances => true;
        private readonly int placeStyle;

        public override string Texture => "EzrnMoreTraps/Items/FlameTrapLowTierItem";

        public override string Name => GetInternalNameFromStyle(placeStyle);

        public static string GetInternalNameFromStyle(int style)
        {
            if (style == 0)
                return "FlameTrapLowTier_Left";
            else if (style == 1)
                return "FlameTrapLowTier_Right";
            else if(style == 2)
            {
                return "FlameTrapLowTier";
            }
            throw new Exception("Invalid style");
        }

        public FlameTrapLowTierItem(int placeStyle)
        {
            this.placeStyle = placeStyle;
        }       

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FlameTrapLowTier>(), placeStyle);
            Item.width = 12;
            Item.height = 12;
            Item.value = 100;
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlameTrap)
                .Register();
        }

        
    }
}
