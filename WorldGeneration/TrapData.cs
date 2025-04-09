using System.Collections.Generic;

public class TrapData
{
    public List<(int x, int y)> LifeCrystalLocations { get; set; } = new();
    public List<(int x, int y)> BoulderLocations { get; set; } = new();
    public List<int> AllChestIndices { get; set; } = new();
    public List<int> TrappableChestIndices { get; set; } = new();


}
