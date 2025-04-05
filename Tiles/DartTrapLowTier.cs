using EzrnMoreTraps.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace EzrnMoreTraps.Tiles
{
    public class DartTrapLowTier : ModTile
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
                Language.GetOrRegister("Mods.EzrnMoreTraps.Tiles.DartTrapLowTier", () => "Low-Tier Dart Trap"));
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(Mod.Find<ModItem>(Items.DartTrapLowTierItem.GetInternalNameFromStyle(2)).Type); //always drop single item type no matter the style
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            int style = 0; // style can always start at 0
            Tile tile = Main.tile[i, j];
            tile.TileFrameY = (short)(style * 18);
            if (Main.LocalPlayer.direction == 1)
            {
                tile.TileFrameX += 18;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        public static void SetStyle(int i, int j, int style)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = (short)(style * 18);
            Terraria.WorldGen.SquareTileFrame(i, j);
        }

        private static int[] frameXCycle = [0, 1];
        public override bool Slope(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int nextFrameX = frameXCycle[tile.TileFrameX / 18];
            tile.TileFrameX = (short)(nextFrameX * 18);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
            return false;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int style = tile.TileFrameX / 18;
            Vector2 spawnPosition;
            float projectileSpeed = 45;
            int damage = ModContent.GetInstance<EzrnMoreTrapsConfig>().DartDamage;

            int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
            int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
            if (style == 0) // facing right
            {
                if (Wiring.CheckMech(i, j, 200))
                {

                    spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position if you need to.
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), spawnPosition, new Vector2(projectileSpeed, 0), ProjectileID.PoisonDart, damage, 1f, Main.myPlayer);

                }
            }
            else if (style == 1) //facing left
            {
                if (Wiring.CheckMech(i, j, 200))
                {

                    spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position if you need to.
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), spawnPosition, new Vector2(-projectileSpeed, 0), ProjectileID.PoisonDart, damage, 1f, Main.myPlayer);
                }
            }
        }

    }
}
