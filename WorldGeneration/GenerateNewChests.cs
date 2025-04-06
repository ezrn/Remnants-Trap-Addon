using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.ComponentModel;

public class GenerateNewChests
{
    public void PlaceCavernChests(int chestGoal)
    {
        int placed = 0;
        int attempts = 0;

        int maxAttempts = chestGoal * 20;

        while (placed < chestGoal && attempts < maxAttempts)
        {
            //try to place chests underground of the same style as their home biome

            attempts++;

            (int x, int y) = GetRandomCavernPos();

            int style = GetChestStyleForBiome(x, y);

            bool sucess = WorldGen.AddBuriedChest(x, y, 0, true, style, trySlope: false, 0);

            if (sucess)
            {
                placed++;
            }
        }
    }

    private (int x, int y) GetRandomCavernPos()
    {

        int cavernStart = (int)Main.rockLayer;
        int cavernEnd = (int)(Main.maxTilesY - 200); // keep some buffer above hell

        int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
        int y = WorldGen.genRand.Next(cavernStart, cavernEnd);
        return (x, y);
    }

    private int GetChestStyleForBiome(int x, int y)
    {
        if (IsJungleBiome(x, y))
        {
            // ivy
            return 10;
        }
        if (IsIceBiome(x, y))
        {
            // frozen
            return 11;
        }

        // default to gold
        return 1;
    }

    private bool IsJungleBiome(int x, int y)
    {
        // really basic jungle check, sometimes generates these in mushroom biomes
        Tile tile = Framing.GetTileSafely(x, y);
        if (tile.TileType == TileID.JungleGrass || tile.TileType == TileID.Mud) return true;
        if (tile.WallType == WallID.JungleUnsafe || tile.WallType == WallID.MudUnsafe) return true;
        return false;
    }

    private bool IsIceBiome(int x, int y)
    {
        // really basic ice check, needs to be improved
        Tile tile = Framing.GetTileSafely(x, y);
        if (tile.WallType == WallID.SnowWallUnsafe) return true;
        if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
            return true;

        return false;
    }

    //desert chests are weird so not doing them lol


    private int FindChestDownward(int startX, int startY, int maxDown)
    {
        for (int offset = 0; offset <= maxDown; offset++)
        {
            int testY = startY + offset;

            if (testY >= Main.maxTilesY)
                break;

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;

                //find chests with startX checkY, thats likely the placed chest
                if (chest.x == startX && chest.y == testY)
                {
                    return i; //return index of that chest
                }
            }
        }
        return -1;
    }
}
