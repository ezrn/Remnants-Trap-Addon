using EzrnMoreTraps.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace EzrnMoreTraps.Tiles
{
    public class SpikyBallTrapLowTier : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true; 

            AddMapEntry(new Color(150, 100, 100),
                Language.GetOrRegister("Mods.EzrnMoreTraps.Tiles.SpikyBallTrapLowTier", () => "Low-Tier Spiky Ball Trap"));
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<SpikyBallTrapLowTierItem>());
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int style = tile.TileFrameX / 18;
            Vector2 spawnPosition;
            int damage = ModContent.GetInstance<EzrnMoreTrapsConfig>().SpikyBallDamage;
            int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
            int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
          
            if (Wiring.CheckMech(i, j, 200))
            {

                spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position if you need to.
                Projectile.NewProjectile(new EntitySource_TileBreak(i, j), spawnPosition, new Vector2(0, 0), ProjectileID.SpikyBallTrap, damage, 2f, Main.myPlayer);

            }
            
        }
    }
}
