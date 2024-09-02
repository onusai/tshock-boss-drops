using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Text.Json;
using IL.Terraria.GameContent.UI.BigProgressBar;
using IL.Terraria.GameContent.Events;
using IL.Terraria.GameContent;
using System.Reflection.Metadata.Ecma335;
using MySqlX.XDevAPI.Relational;

namespace BossDrops
{
    [ApiVersion(2, 1)]
    public class BossDrops : TerrariaPlugin
    {

        public override string Author => "Onusai";
        public override string Description => "Configure bosses to drop additional items";
        public override string Name => "BossDrops";
        public override Version Version => new Version(1, 0, 0, 0);

        Dictionary<string, bool> defeatedBosses = new Dictionary<string, bool>();
        
        Dictionary<int, string> bossIds = new Dictionary<int, string>
        {
                {50, "KS" },
                {4, "EoC"},
                {266, "BoC"},
                {13, "EoW"},
                {14, "EoW"},
                {15, "EoW"},
                {222, "QB"},
                {35, "Skeletron"},
                {668, "Deerclops"},
                {113, "WoF"},
                {657, "QS"},
                {134, "Destroyer"},
                {135, "Destroyer"},
                {136, "Destroyer"},
                {125, "Twins"},
                {126, "Twins"},
                {127, "Prime"},
                {262, "Plantera"},
                {370, "Duke"},
                {636, "Empress"},
                {245, "Golem"},
                {439, "Cultist"},
                {398, "ML"},
        };

        int ticksElapsed = 0;
        int dontDropTicks = 0;

        // Eater of Worlds is very problematic so have to do this
        int[] EoWIds = new int[] { 13, 14, 15 };
        int checkEoWTicks = 0;
        Vector2 EoWPos;

        public class ConfigData
        {
            public bool Enabled { get; set; } = true;
            public bool DropOnlyOnFirstKill { get; set; } = false;

            public Dictionary<string, int[]> DropItems { get; set; } = new Dictionary<string, int[]>
            {
                {"KS", new int[]{4022, 3532, 499}},
                {"EoC", new int[]{74}},
                {"BoC", new int[]{}},
                {"EoW", new int[]{}},
                {"QB", new int[]{}},
                {"Skeletron", new int[]{}},
                {"Deerclops", new int[]{}},
                {"WoF", new int[]{}},
                {"QS", new int[]{}},
                {"Destroyer", new int[]{}},
                {"Twins", new int[]{}},
                {"Prime", new int[]{}},
                {"Plantera", new int[]{}},
                {"Duke", new int[]{}},
                {"Empress", new int[]{}},
                {"Golem", new int[]{}},
                {"Cultist", new int[]{}},
                {"ML", new int[]{}},
            };
        }

        ConfigData config;

        public BossDrops(Main game) : base(game) { }

        public override void Initialize()
        {
            config = PluginConfig.Load("BossDrops");
            ServerApi.Hooks.GameInitialize.Register(this, OnGameLoad);
        }

        void OnGameLoad(EventArgs e)
        {
            if (!config.Enabled) return;
            ServerApi.Hooks.NpcLootDrop.Register(this, OnNpcLootDrop);
            ServerApi.Hooks.GamePostInitialize.Register(this, GamePostInitialize);
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            RegisterCommand("bosslist", "", ListBosses, "Shows a list of bosses and whether they have been defeated");
            RegisterCommand("bossreset", "tshock.admin", ClearBosses, "Resets boss list");
        }
        void GamePostInitialize(EventArgs args)
        {
            defeatedBosses = GetDefeatedBosses();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameLoad);
                ServerApi.Hooks.NpcLootDrop.Deregister(this, OnNpcLootDrop);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, GamePostInitialize);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            }
            base.Dispose(disposing);
        }

        void RegisterCommand(string name, string perm, CommandDelegate handler, string helptext)
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(perm, handler, name)
            { HelpText = helptext });
        }

        void OnGameUpdate(EventArgs args)
        {
            // annoying workaround for eow
            if (ticksElapsed < 60)
            {
                ticksElapsed += 1;
                return;
            }
            ticksElapsed = 0;

            if (dontDropTicks > 0) dontDropTicks--;

            if (checkEoWTicks == 0) return;
            checkEoWTicks -= 1;

            if ((config.DropOnlyOnFirstKill ? defeatedBosses["EoW"] : false) || !NPC.downedBoss2) return;

            if (!config.DropOnlyOnFirstKill) NPC.downedBoss2 = false;

            if (!config.DropOnlyOnFirstKill && dontDropTicks > 0) return;
            dontDropTicks = 2;

            defeatedBosses["EoW"] = true;

            BossDefeated("EoW", EoWPos);
        }

        void OnNpcLootDrop(NpcLootDropEventArgs e)
        {

            bool isBoss = false;

            switch (e.NpcId)
            {
                case 50: // KS
                case 4: // EoC
                case 266: // BoC
                case 222: // QB
                case 35: // Skeletron
                case 668: // Deerclops
                case 657: // QS
                case 134: // Destroyer
                case 135: // Destroyer
                case 136: // Destroyer
                case 125: // Twins
                case 126: // Twins
                case 127: // Prime
                case 262: // Plantera
                case 370: // Duke
                case 636: // Empress
                case 245: // Golem
                case 439: // Cultist
                case 398: // ML
                    isBoss = true;
                    break;

                case 113: // WoF
                    isBoss = true;
                    // terraria doesnt keep track if wof downed, so repurposing clown variable
                    NPC.downedClown = true;
                    break;

                case 13: // EoW
                case 14:
                case 15:
                    EoWPos = e.Position;
                    checkEoWTicks = 2;
                    break;

                default:
                    break;
            }

            if (!isBoss || (config.DropOnlyOnFirstKill ? defeatedBosses[bossIds[e.NpcId]] : false)) return;
            defeatedBosses[bossIds[e.NpcId]] = true;

            if (!config.DropOnlyOnFirstKill && dontDropTicks > 0) return;
            dontDropTicks = 2;

            BossDefeated(bossIds[e.NpcId], e.Position);
        }


        void BossDefeated(string id, Vector2 pos)
        {
            

            //TShock.Utils.Broadcast(String.Format("Killed {0}", id), Color.Yellow);
            foreach(int itemId in config.DropItems[id])
            {
                Item.NewItem(null, pos, Vector2.Zero, itemId, 1, false, 0, true, false);
            }
            
        }

        Dictionary<string, bool> GetDefeatedBosses()
        {
            Dictionary<string, bool> defeated = new Dictionary<string, bool>
            {
                {"KS", NPC.downedSlimeKing },
                {"EoC", NPC.downedBoss1},
                {"BoC", NPC.downedBoss2},
                {"EoW", NPC.downedBoss2},
                {"QB", NPC.downedQueenBee},
                {"Skeletron", NPC.downedBoss3},
                {"Deerclops", NPC.downedDeerclops},
                {"WoF", NPC.downedClown},
                {"QS", NPC.downedQueenSlime},
                {"Destroyer", NPC.downedMechBoss1},
                {"Twins", NPC.downedMechBoss2},
                {"Prime", NPC.downedMechBoss3},
                {"Plantera", NPC.downedPlantBoss},
                {"Duke", NPC.downedFishron},
                {"Empress", NPC.downedEmpressOfLight},
                {"Golem", NPC.downedGolemBoss},
                {"Cultist", NPC.downedAncientCultist},
                {"ML", NPC.downedMoonlord},
            };

            return defeated;
        }

        void ClearBosses(CommandArgs args)
        {
            NPC.downedSlimeKing = false;
            NPC.downedBoss1 = false;
            NPC.downedBoss2 = false;
            NPC.downedQueenBee = false;
            NPC.downedBoss3 = false;
            NPC.downedDeerclops = false;
            NPC.downedClown = false;
            NPC.downedQueenSlime = false;
            NPC.downedMechBoss1 = false;
            NPC.downedMechBoss2 = false;
            NPC.downedMechBoss3 = false;
            NPC.downedPlantBoss = false;
            NPC.downedFishron = false;
            NPC.downedEmpressOfLight = false;
            NPC.downedGolemBoss = false;
            NPC.downedAncientCultist = false;
            NPC.downedMoonlord = false;
            defeatedBosses = GetDefeatedBosses();
            args.Player.SendInfoMessage("Boss list cleared");
        }

        void ListBosses(CommandArgs args)
        {
            foreach (KeyValuePair<string, bool> item in defeatedBosses)
            {
                if (item.Key == "BoC") continue;

                if (item.Key == "EoW")
                {
                    TShock.Utils.Broadcast(String.Format("EoW/BoC: {1}", item.Key, item.Value), Color.White);
                }
                else
                {
                    TShock.Utils.Broadcast(String.Format("{0}: {1}", item.Key, item.Value), Color.White);
                }
            }
        }

        public static class PluginConfig
        {
            public static string filePath;
            public static ConfigData Load(string Name)
            {
                filePath = String.Format("{0}/{1}.json", TShock.SavePath, Name);

                if (!File.Exists(filePath))
                {
                    var data = new ConfigData();
                    Save(data);
                    return data;
                }

                var jsonString = File.ReadAllText(filePath);
                var myObject = JsonSerializer.Deserialize<ConfigData>(jsonString);

                return myObject;
            }

            public static void Save(ConfigData myObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(myObject, options);

                File.WriteAllText(filePath, jsonString);
            }
        }
    }
}