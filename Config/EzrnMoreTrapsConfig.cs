using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace EzrnMoreTraps
{
    public class EzrnMoreTrapsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("CavernChestSettings")]
        [Label("Cavern Chest Frequency")]
        [Tooltip("How many additional chests do you want to generate in the cavern layer? (necessary for Dead Man's Chests)")]
        [Range(0f, 1f)]
        [DefaultValue(0.7f)]
        public float CavernChestsFrequency;

        [Label("Add More Chest Items")]
        [Tooltip("Adds modded items from calamity and thorium to underground loot pools")]
        [DefaultValue(true)]
        public bool AddMoreChestItems;

        [Label("Stack webs in remnants chests")]
        [Tooltip("Not recommended with WGI jumbled chests")]
        [DefaultValue(false)]
        public bool WebStackingEnabled;

        [Header("NewTrapsSettings")]
        [Label("Trap Frequency")]
        [Tooltip("How many traps do you want in your world?")]
        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        public float NewTrapsFrequency;

        [Label("Spiky Ball Traps")]
        [Tooltip("Do you want spiky ball traps?")]
        [DefaultValue(true)]
        public bool UseSpikyBallTraps;

        [Label("Dead Man's Chests")]
        [Tooltip("Do you want Dead Man's Chests?")]
        [DefaultValue(true)]
        public bool DoDeadManChests;

        [Label("Dead Man's Chest Frequency")]
        [Tooltip("How many Dead Man's Chests do you want?")]
        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        public float DeadManChestFrequency;

        [Label("Heart Crystal Boulders")]
        [Tooltip("Do you want heart crystal boulders?")]
        [DefaultValue(true)]
        public bool DoHeartBoulders;

        [Label("Heart Crystal Boulder Frequency")]
        [Tooltip("How many heart boulders do you want?")]
        [Range(0f, 1f)]
        [DefaultValue(0.2f)]
        public float HeartBoulderFrequency;

        [Label("Bouncy Boulders")]
        [Tooltip("Do you want bouncy boulders?")]
        [DefaultValue(true)]
        public bool DoBouncyBoulders;

        [Label("Bouncy Boulder Frequency")]
        [Tooltip("How many bouncy boulders do you want?")]
        [Range(0f, 1f)]
        [DefaultValue(0.2f)] 
        public float BouncyBoulderFrequency;

        [Header("TrapDamageSettings")]
        [Label("Flamethrower Damage")]
        [Tooltip("Adjust the damage of flamethrower traps")]
        [DefaultValue(25)]
        public int FlameDamage;

        [Label("Dart Damage")]
        [Tooltip("Adjust the damage of dart traps")]
        [DefaultValue(25)]
        public int DartDamage;

        [Label("Spiky Ball Damage")]
        [Tooltip("Adjust the damage of spiky ball traps")]
        [DefaultValue(25)]
        public int SpikyBallDamage;
    }
}
