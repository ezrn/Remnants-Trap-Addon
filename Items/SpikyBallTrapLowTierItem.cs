using EzrnMoreTraps.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System;

namespace EzrnMoreTraps.Items
{
    public class SpikyBallTrapLowTierItem : ModItem
    {      
        protected override bool CloneNewInstances => true;
        private readonly int placeStyle;

        public override string Texture => "EzrnMoreTraps/Items/SpikyBallTrapLowTierItem";

        public override string Name => GetInternalName();

        public static string GetInternalName()
        {
            return "SpikyBallTrapLowTier";
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.SpikyBallTrapLowTier>(), placeStyle);
            Item.width = 12;
            Item.height = 12;
            Item.value = 100;
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpikyBallTrap)
                .Register();
        }
    }
}

