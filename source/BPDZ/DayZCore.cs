using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BPDayZ;
using System.IO;

namespace BPDZ
{
    class DayZCore
    {
        public const string ResourceName = "BPDayZ";
        [EntryPoint(ResourceName)]
        public static void Main()
        {
            Debug.Log($"[BPDZ] {ResourceName} being loaded in");
            if (!Directory.Exists(@"BPDayZ"))
            {
                Debug.Log("[BPDZ] Creating directory BPDayZ...");
                Directory.CreateDirectory("BPDayZ/Index");
                Directory.CreateDirectory("BPDayZ/Groups");
                Debug.Log("[BPDZ] Successfully created directory");
                Debug.Log("[BPDZ] Creating files...");
                File.Create("BPDayZ/GodList.txt");
                File.Create("BPDayZ/DiscordLink.txt");
                Debug.Log("[BPDZ] Successfully created files");
            }

            else
            {
                Debug.Log("[BPDZ] All resources loaded");
            }

            SetResourceInfo();
            PlayerzEvents();            
        }
        static void SetResourceInfo()
        {
            BP_API.Core.Resources[ResourceName].ResourceInfo.Author = "Unlucky";
            BP_API.Core.Resources[ResourceName].ResourceInfo.Description = "A DayZ plugin created by Unlucky";
        }

        static void PlayerzEvents()
        {
            PlayerEvents.OnPlayerConnected += OnPlayerConnected;
            PlayerEvents.OnPlayerDamage += OnPlayerDamage;
            PlayerEvents.OnGlobalChatMessage += SvGlobalChatMessage;
        }

        static bool OnPlayerDamage(Player player, Player attacker, ref DamageIndex type, ref float amount, ref Collider collider)
        {
            foreach (var name in File.ReadAllLines(Commandz.GodListFile))
            {
                if (name == player.Username)
                {
                    player.SendChatMessage(SvSendType.Self, $"<color=white>Blocked {amount}HP of damage</color>");
                    return false;
                }
            }
            return true;
        }

        static void ZombieEvents()
        {
            PlayerEvents.OnNpcSpawned += Zombies.SetZombie;
            PlayerEvents.OnNpcSpawned += Zombies.AliveLoop;
        }

        static bool SvGlobalChatMessage(Player player, ref string message)
        {
            DayZManager.SvGlobalChatMessage(player, message);
            return false;
        }
        static void OnPlayerConnected(Player player)
        {
            if (player.svPlayer.playerData.username != null)
                return;
            player.Inventory.AddItem(-1975896234, 1);
            player.Inventory.AddItem(493970259, 1);
        }

        public static int RNG(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
