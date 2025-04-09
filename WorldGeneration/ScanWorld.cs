using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using EzrnMoreTraps.WorldGen;
using EzrnMoreTraps;
using System;

public class ScanWorld : ModSystem
{

    private TrapData trapData;
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) //world gen passes
    {
        int oasisIndex = tasks.FindIndex(pass => pass.Name == "Oasis");
        if (oasisIndex != -1)
        {
            tasks.Insert(oasisIndex + 1,
                new PassLegacy("Generate Extra Cavern Chests", GenerateCavernChestsPass)
            );
        }
        int removeBrokenTrapsIndex = tasks.FindIndex(pass => pass.Name == "Remove Broken Traps");
        if (removeBrokenTrapsIndex != -1)
        {
            tasks.Insert(removeBrokenTrapsIndex + 1,
                new PassLegacy("Add New Traps", AddNewTrapsPass)
            );
        }
        int addNewTrapsIndex = tasks.FindIndex(pass => pass.Name == "Add New Traps");
        if (addNewTrapsIndex != -1)
        {
            tasks.Insert(addNewTrapsIndex + 1,
                new PassLegacy("Add More Items to Chests", AddMoreChestItems)
            );
        }

    }

    private void GenerateCavernChestsPass(GenerationProgress progress, GameConfiguration config)
    {
        progress.Message = "Generating additional chests in Cavern layer...";

        float frequency = ModContent.GetInstance<EzrnMoreTrapsConfig>().CavernChestsFrequency;
        int chestGoal = (int)(500 * frequency);

        GenerateNewChests generator = new GenerateNewChests();
        generator.PlaceCavernChests(chestGoal);
    }

    private void AddNewTrapsPass(GenerationProgress progress, GameConfiguration config)
    {
        progress.Message = "Scanning world & adding new traps...";

        trapData = ScanWorldForTilesAndChests();

        AddHeartBoulders heartBoulders = new AddHeartBoulders();
        AddBouncyBoulders bouncyBoulders = new AddBouncyBoulders();
        AddDeadmansChests deadmansChests = new AddDeadmansChests();

        heartBoulders.ModifyWorldWithTraps(trapData);
        bouncyBoulders.ModifyWorldWithTraps(trapData);
        deadmansChests.ModifyWorldWithTraps(trapData);

        float trapsFrequency = ModContent.GetInstance<EzrnMoreTrapsConfig>().NewTrapsFrequency;

        AddTraps trapGen = new AddTraps();
        trapGen.PlaceTraps(trapsFrequency);
    }

    private void AddMoreChestItems(GenerationProgress progress, GameConfiguration config)
    {
        progress.Message = "Scattering better loot...";

        if (!ModContent.GetInstance<EzrnMoreTrapsConfig>().AddMoreChestItems)
            return;

        AddMoreLootToChests generator = new AddMoreLootToChests();

        TrapData data = new();

        generator.AddLootToMyChests(trapData.AllChestIndices);
    }

    private TrapData ScanWorldForTilesAndChests()
    {
        TrapData data = new TrapData();

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile == null)
                    continue;

                // life crystal storing
                if (tile.TileType == TileID.Heart)
                {
                    data.LifeCrystalLocations.Add((x, y));
                }

                // boulder storing
                if (tile.TileType == TileID.Boulder)
                {
                    data.BoulderLocations.Add((x, y));
                }
            }
        }

        //chest/container storing
        for (int i = 0; i < Main.maxChests; i++)
        {
            Chest c = Main.chest[i];
            if (c == null)
                continue;

            Tile chestTile = Main.tile[c.x, c.y];
            if (chestTile == null)
                continue;

            if (chestTile.TileType == TileID.Containers || chestTile.TileType == TileID.Containers2)
            {
                data.AllChestIndices.Add(i);

                if (c.y < Main.rockLayer)
                    continue;

                Tile blockUnderChest = Framing.GetTileSafely(c.x, c.y + 2);

                if (blockUnderChest.TileType != TileID.Mud && blockUnderChest.TileType != TileID.RichMahogany &&
                    blockUnderChest.TileType != TileID.SnowBrick && blockUnderChest.TileType != TileID.WoodBlock)
                {
                    data.TrappableChestIndices.Add(i); //dont store chests that generate inside of buildings
                    //somewhat of a rudimentary check, could be improved
                    //no check to make sure its a gold chest, so if a different type of chest is picked it will 
                    //convert it to deadmans nonetheless

                }
            }
        }

        return data;
    }

}
