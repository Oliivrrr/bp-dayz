using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BPDayZ;

namespace BPDZ
{
    class DayZCore
    {
        public const string ResourceName = "BPDayZ";
        [EntryPoint(ResourceName)]
        public static void Main()
        {
            Debug.Log($"{ResourceName} being loaded in");
            SetResourceInfo();
            PlayerzEvents();
            
        }
        static void SetResourceInfo()
        {
            BP_API.Core.Resources[ResourceName].ResourceInfo.Author = "Unlucky";
            BP_API.Core.Resources[ResourceName].ResourceInfo.Description = "A DayZ plugin for Broke protocol created by Unlucky";
        }

        static void PlayerzEvents()
        {
            PlayerEvents.OnPlayerConnected += OnPlayerConnected;
            // PlayerEvents.OnGlobalChatMessage += SvGlobalChatMessage;
        }

        static void ZombieEvents()
        {
            PlayerEvents.OnNpcSpawned += Zombies.SetZombie;
        }

        static bool SvGlobalChatMessage(Player instance, ref string message)
        {
            DayZManager.SvGlobalChatMessage(instance, message);
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
