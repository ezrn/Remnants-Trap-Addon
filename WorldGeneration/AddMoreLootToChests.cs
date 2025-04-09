using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; 
using EzrnMoreTraps;

public class AddMoreLootToChests
{
    public class LootItem
    {
        public string internalName { get; set; }
        public int amount { get; set; }
    }

    private static List<LootItem> lootItems;

    static AddMoreLootToChests()
    {
        lootItems = LoadLootItems();
    }

    private static List<LootItem> LoadLootItems()
    {
        var mod = ModContent.GetInstance<EzrnMoreTraps.EzrnMoreTraps>();
        List<LootItem> lootItems = new List<LootItem>();

        try
        {
            using (Stream stream = mod.GetFileStream("lootItems.json"))
            {
                if (stream == null)
                {
                    mod.Logger.Warn("Missing lootItems.json file.");
                    return lootItems;
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    lootItems = JsonConvert.DeserializeObject<List<LootItem>>(json);
                }
            }

            if (lootItems == null)
            {
                mod.Logger.Warn("lootItems.json file is empty or improperly formatted.");
                lootItems = new List<LootItem>();
            }
            return lootItems;
        }
        catch (System.Exception ex)
        {
            mod.Logger.Warn("Improperly formatted lootItems.json file: " + ex.Message);
            return new List<LootItem>();
        }
    }

    public void AddLootToMyChests(List<int> chestIndices) //dont do this to any chest above cavern layer
    {
        foreach (int chestIndex in chestIndices)
        {
            Chest c = Main.chest[chestIndex];

            if (c == null || c.y < Main.rockLayer)
                continue;

            // fixed 50% chance to add items to a chest
            if (Main.rand.NextFloat() < 0.50f)
            {
                if (c == null)
                    continue;

                ShiftChestLoot(c);
            }
            StackWebs(c);
        }
    }

    private void ShiftChestLoot(Chest chest)
    {
        if (lootItems.Count == 0)
        {
            return;
        }

        // randomly select one loot item from the loaded JSON list
        LootItem selectedLoot = lootItems[Main.rand.Next(lootItems.Count)];
        // resolve the internal name to an item type
        int type = GetItemTypeByInternalName(selectedLoot.internalName);
        if (type == -1)
        {
            return;
        }

        Item[] items = chest.item;
        for (int slot = items.Length - 1; slot > 0; slot--)
        {
            items[slot] = items[slot - 1];
        }

        items[0] = new Item();
        items[0].SetDefaults(type);
        items[0].stack = selectedLoot.amount;
    }

    private int GetItemTypeByInternalName(string internalName)
    {
        var mod = ModContent.GetInstance<EzrnMoreTraps.EzrnMoreTraps>();

        // check for a modName:itemName pattern
        int colonIndex = internalName.IndexOf(':');
        if (colonIndex != -1)
        {
            // parse out both parts
            string modName = internalName.Substring(0, colonIndex);
            string itemName = internalName.Substring(colonIndex + 1);

            // search for mod item
            if (ModContent.TryFind(modName, itemName, out ModItem modItem))
            {
                return modItem.Type;
            }
            return -1;
        }
        else
        {
            // vanilla item if no colon is present
            int type = ItemID.Search.GetId(internalName);
            if (type != -1)
            {
                return type;
            }

            return -1;
        }
    }

    private void StackWebs(Chest chest)
    {
        if (!ModContent.GetInstance<EzrnMoreTrapsConfig>().WebStackingEnabled)
            return;

        Item[] items = chest.item;
        int firstWebIndex = -1;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].type == ItemID.Cobweb && items[i].stack > 0)
            {
                firstWebIndex = i;
                break;
            }
        }

        if (firstWebIndex == -1)
        {
            return;
        }

        int totalWebCount = items[firstWebIndex].stack;

        for (int i = firstWebIndex + 1; i < items.Length; i++)
        {
            if (items[i] != null && items[i].type == ItemID.Cobweb && items[i].stack > 0)
            {
                totalWebCount += items[i].stack;
                items[i] = new Item();
            }
        }
        items[firstWebIndex].stack = totalWebCount;
    }
}
