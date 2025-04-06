using Terraria;
using Terraria.ID;
using EzrnMoreTraps.WorldGen; 
using EzrnMoreTraps;
using Terraria.ModLoader;


public class AddDeadmansChests : ITrapModifier
{
    public void ModifyWorldWithTraps(TrapData data)
    {
        if (!ModContent.GetInstance<EzrnMoreTrapsConfig>().DoDeadManChests)
            return;

        float frequency = ModContent.GetInstance<EzrnMoreTrapsConfig>().DeadManChestFrequency;

        // chance for each gold chest(stored in trapData) to become a dead mans chest
        foreach (int chestIndex in data.TrappableChestIndices)
        {
            if (Main.rand.NextFloat() < frequency)
            {
                Chest c = Main.chest[chestIndex];
                if (c is null)
                    continue;

                ConvertToDeadMansChest(c);
            }
        }
    }

    private void ConvertToDeadMansChest(Chest chest)
    {
        int x = chest.x;
        int y = chest.y;

        short baseFrameX = (short)(4 * 36);
        short topRowFrameY = 0;
        short bottomRowFrameY = 18;

        // set all tiles to dead mans chest

        Tile tTopLeft = Framing.GetTileSafely(x, y);
        tTopLeft.TileType = (ushort)TileID.Containers2;
        tTopLeft.TileFrameX = baseFrameX;
        tTopLeft.TileFrameY = topRowFrameY;
        tTopLeft.HasTile = true;

        Tile tTopRight = Framing.GetTileSafely(x + 1, y);
        tTopRight.TileType = (ushort)TileID.Containers2;
        tTopRight.TileFrameX = (short)(baseFrameX + 18);
        tTopRight.TileFrameY = topRowFrameY;
        tTopRight.HasTile = true;

        Tile tBottomLeft = Framing.GetTileSafely(x, y + 1);
        tBottomLeft.TileType = (ushort)TileID.Containers2;
        tBottomLeft.TileFrameX = baseFrameX;
        tBottomLeft.TileFrameY = bottomRowFrameY;
        tBottomLeft.HasTile = true;

        Tile tBottomRight = Framing.GetTileSafely(x + 1, y + 1);
        tBottomRight.TileType = (ushort)TileID.Containers2;
        tBottomRight.TileFrameX = (short)(baseFrameX + 18);
        tBottomRight.TileFrameY = bottomRowFrameY;
        tBottomRight.HasTile = true;

        AddTraps traps = new AddTraps();

        //random trap for chest
        (int ceilingX, int ceilingY) = traps.FindAirBelowSolid(x, y, 50);
        bool trapPlaced = false;

        //50% chance for dynamite, 50% chance for another trap. If traps fail to place, defaults to dynamite
        float roll = (float)Main.rand.NextDouble();
        if (roll < 0.5f)
        {
            float altRoll = (float)Main.rand.NextDouble();
            if (altRoll < 0.5f)
            {
                trapPlaced = traps.SetupSpikyBallTrap(x, y + 1);
            }
            else
            {
                trapPlaced = traps.TryPlaceFlameOrDartTrap(x, y + 1);
            }
            if (!trapPlaced)
            {
                PlaceDynamiteTrapUnderChest(x, y);
                trapPlaced = true;
            }
        }
        else
        {
            PlaceDynamiteTrapUnderChest(x, y);
            trapPlaced = true;
        }
        

        //refresh visuals of chest
        Terraria.WorldGen.SquareTileFrame(x, y);
        Terraria.WorldGen.SquareTileFrame(x + 1, y);
        Terraria.WorldGen.SquareTileFrame(x, y + 1);
        Terraria.WorldGen.SquareTileFrame(x + 1, y + 1);
    }

    private void PlaceDynamiteTrapUnderChest(int chestX, int chestY) //places the dynamite trap
    {
        int primaryX = chestX;
        int primaryY = chestY + 4;
        int secondaryX = chestX + 1;
        int secondaryY = chestY + 4;

        int supportY = primaryY + 1;

        Terraria.WorldGen.KillTile(primaryX, primaryY, noItem: true);
        Terraria.WorldGen.PlaceTile(primaryX, supportY, TileID.Stone, forced: true);
        Terraria.WorldGen.PlaceTile(primaryX, primaryY, TileID.Explosives, forced: true);

        bool placeSecond = Main.rand.NextFloat() < 0.5f;
        if (placeSecond)
        {
            Terraria.WorldGen.KillTile(secondaryX, secondaryY, noItem: true);
            Terraria.WorldGen.PlaceTile(secondaryX, supportY, TileID.Stone, forced: true);
            Terraria.WorldGen.PlaceTile(secondaryX, secondaryY, TileID.Explosives, forced: true);
        }

        Framing.GetTileSafely(chestX, chestY).RedWire = true;
        Framing.GetTileSafely(chestX + 1, chestY).RedWire = true;
        Framing.GetTileSafely(chestX, chestY + 1).RedWire = true;
        Framing.GetTileSafely(chestX + 1, chestY + 1).RedWire = true;

        Framing.GetTileSafely(primaryX, primaryY).RedWire = true;
        Framing.GetTileSafely(primaryX, primaryY - 1).RedWire = true;
        Framing.GetTileSafely(primaryX, primaryY - 2).RedWire = true;

        if (placeSecond)
        {
            Framing.GetTileSafely(secondaryX, secondaryY).RedWire = true;
            Framing.GetTileSafely(secondaryX, secondaryY - 1).RedWire = true;
            Framing.GetTileSafely(secondaryX, secondaryY - 2).RedWire = true;
        }

        Terraria.WorldGen.SquareTileFrame(primaryX, primaryY);
        if (placeSecond)
        {
            Terraria.WorldGen.SquareTileFrame(secondaryX, secondaryY);
        }
    }

}

