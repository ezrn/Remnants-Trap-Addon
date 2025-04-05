using EzrnMoreTraps;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class AddHeartBoulders : ITrapModifier
{
    public void ModifyWorldWithTraps(TrapData data)
    {
        if (!ModContent.GetInstance<EzrnMoreTrapsConfig>().DoHeartBoulders)
            return;

        float frequency = ModContent.GetInstance<EzrnMoreTrapsConfig>().HeartBoulderFrequency;
        
        foreach ((int x, int y) in data.LifeCrystalLocations)
        {
            if (Main.rand.NextFloat() < frequency)
            {
                Main.tile[x, y].TileType = TileID.LifeCrystalBoulder;
            }
        }
    }
}
