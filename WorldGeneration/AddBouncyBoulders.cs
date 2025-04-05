using EzrnMoreTraps;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class AddBouncyBoulders : ITrapModifier
{
   public void ModifyWorldWithTraps(TrapData data)
    {
        if (!ModContent.GetInstance<EzrnMoreTrapsConfig>().DoBouncyBoulders)
            return;

        float frequency = ModContent.GetInstance<EzrnMoreTrapsConfig>().BouncyBoulderFrequency;

        foreach ((int x, int y) in data.BoulderLocations)
        {
            if (Main.rand.NextFloat() < frequency)
            {
                Main.tile[x, y].TileType = TileID.BouncyBoulder;
            }
        }
    }

}
