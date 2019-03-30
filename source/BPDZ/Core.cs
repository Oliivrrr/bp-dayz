/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using static BP_API.Core;
using static BPDZ.Variables;
using System.Collections;
using Logger = BP_API.Logger;

namespace BPDZ
{
    class Core
    {
        [EntryPoint(resourceName)]
        public static void Main()
        {
            Util.Log(resourceName + " is being loaded in!");
            Util.ValidateFiles();
            Util.ReadFiles();
            SetResourceInfo();
            RegisterEvents();
        }
        static void SetResourceInfo()
        {
            BP_API.Core.Resources[resourceName].ResourceInfo.Author = "Unlucky";
            BP_API.Core.Resources[resourceName].ResourceInfo.Description = "A DayZ plugin created by Unlucky";
        }
        static void RegisterEvents()
        {
            PlayerEvents.OnPlayerConnected += OnPlayerConnected;
            PlayerEvents.OnPlayerDisconnected += OnPlayerDisconnected;
            PlayerEvents.OnPlayerDamage += OnPlayerDamage;
            PlayerEvents.OnGlobalChatMessage += SvGlobalChatMessage;
            PlayerEvents.OnPlayerCrime += OnPlayerCrime;
            PlayerEvents.OnNpcSpawned += OnNpcSpawned;
            ServerEvents.OnStartServer += OnStartServer;
        }

        static void OnStartServer(SvManager svMan)
        {
            svMan.startMoney = 0;
            svMan.StartCoroutine(DayZLoop(SvMan));
        }

        private static IEnumerator DayZLoop(SvManager svMan)
        {
            while (true)
            {
                yield return new WaitForSeconds(60);
                ShPlayer randomPlayer = svMan.GetRandomRealPlayer();
                if (randomPlayer != null && randomPlayer.ground)
                {
                    LootDrops.Initialize(Players.GetPlayerByID(svMan.GetRandomRealPlayer().ID));
                }
            }
        }

        static bool OnNpcSpawned(Player player, ref Vector3 position, ref Quaternion rotation, ref Place place, ref Waypoint node, ref ShPlayer spawner, ref ShEntity mount, ref ShPlayer enemy)
        {
            return true;
        }

        static bool OnPlayerCrime(Player player, ref byte crimeIndex, ref ShEntity victim)
        {
            if (crimeIndex == CrimeIndex.Murder)
            {
                if (!player.IsServerSide() && !victim.svEntity.serverside)
                {
                    player.SendChatMessage(SvSendType.All, $"<color=red>Player {player.Username} killed {Players.GetPlayerByID(victim.ID).Username} using {player.shPlayer.curEquipable.itemName}</color>");
                    Debug.Log($"[BPDZ] {player.Username} killed {Players.GetPlayerByID(victim.ID).Username}");
                }
                else if (player.IsServerSide() && !victim.svEntity.serverside)
                {
                    player.SendChatMessage(SvSendType.All, $"<color=red>{Players.GetPlayerByID(victim.ID).Username} was killed by a zombie</color>");
                }
            }

            /* Need to find a way to locate restricted area colliders - Unlucky
            if (crimeIndex == CrimeIndex.Trespassing && player.shPlayer.curWearables[0].ID != -1627168389)
            {
                while (player.shPlayer.headCollider.bounds.Intersects( blah blah find a way))
                {
                    player.svPlayer.Damage(DamageIndex.Null, 5, player.shPlayer, player.shPlayer.headCollider);
                    Debug.Log("Damaged");
                }
            }*/

            return true;
        }

        static bool OnPlayerDamage(Player player, Player attacker, ref DamageIndex type, ref float amount, ref Collider collider)
        {
            foreach (var name in FileData.GoddedPlayers)
            {
                if (name != player.Username)
                    continue;
                player.SendChatMessage($"<color=#fff>Blocked {amount}HP of damage</color>");
                return true;
            }
            return false;
        }

        static bool SvGlobalChatMessage(Player player, ref string message)
        {
            Manager.SvGlobalChatMessage(player, message);
            Loggers.Chat.Log($"[{player.ID}]  {player.FilteredUsername}: {message.FilterString()}");
            return true;
        }
        static void OnPlayerConnected(Player player)
        {
            if (player.svPlayer.playerData.username != null)
            {
                Loggers.Chat.Log($"[{player.ID}] {player.Username} Joined the server ({player.UserData.GetIpV4()})");
                return;
            }
            Loggers.Misc.Log($"[{player.ID}] {player.Username} Registered ({player.UserData.GetIpV4()})");
            player.SendChatMessage(SvSendType.All, $"{player.Username} Registered!");
            player.Inventory.AddItem(-1975896234, 1);
            player.Inventory.AddItem(-1627168389, 1);
            player.Inventory.AddItem(493970259, 35);
        }

        static void OnPlayerDisconnected(Player player)
        {
            Loggers.Misc.Log($"[{player.ID}] {player.Username} Left the server");
        }

        public static Vector3 RandomPosition(Vector3 curPosition)
        {
            int num = ChooseInt(-1,1);
            Vector3 finalPosition = new Vector3(curPosition.x + 8 * num, curPosition.y, curPosition.z + 8 * num);
            return finalPosition;
        }

        public static IEnumerator KillDelay(ShEntity entity, float time)
        {
            yield return new WaitForSeconds(time);
            entity.Destroy();
        }

        public static int GenerateRandom(int min, int max) => Variables.Random.Next(min, max);
        public static float GenerateRandomF(float min, float max) => UnityEngine.Random.Range(min, max);

        public static int ChooseInt(int option1, int option2)
        {
            int num = GenerateRandom(0, 1);
            if (num == 0)
            {
                return option1;
            }
            else
            {
                return option2;
            }
        }

        [Command(nameof(SpawnNPC), "Spawn a Zombie.", "Usage: /spawnzombie [TypeID]", new string[] { "spawnzombie", "zombie"}, true, true)]
        public static void SpawnNPC(Player player, int id)
        {
            ShEntity Zombie = player.svPlayer.svManager.AddNewEntity(player.shPlayer.manager.skinPrefabs[id], player.shPlayer.GetPlace(), player.shPlayer.GetPosition(), player.shPlayer.GetRotation(), false);
            foreach (var item in Zombie.myItems.Values)
            {
                Zombie.RemoveFromMyItems(item.item.ID, item.count);
            }
            player.SendSuccessMessage($"Successfully Spawned Zombie!");
        }

        [Command(nameof(SpawnLootDrop), "Spawn a Loot Drop.", "Usage: /spawndrop [Tier]", new string[] { "spawndrop", "lootdrop" }, true, true)]
        public static void SpawnLootDrop(Player player, int id)
        {
            ShEntity Zombie = player.svPlayer.svManager.AddNewEntity(player.shPlayer.manager.skinPrefabs[1], player.shPlayer.GetPlace(), player.shPlayer.GetPosition(), player.shPlayer.GetRotation(), false);
            LootDrops.Initialize(Players.GetPlayerByID(Zombie.ID));
            player.SendSuccessMessage($"Successfully Spawned Loot Drop!");
        }

        [Command(nameof(Godmode), "Prevents the player from taking damage.", "Usage: /god [username]", new string[] { "godmode", "god" }, true)]
        public static void Godmode(Player player, Player target)
        {
            var msg = $"Successfully {{0}} godmode for {target.FilteredUsername}.";
            if (FileData.GoddedPlayers.Contains(target.Username))
            {
                FileData.GoddedPlayers.Remove(target.Username);
                Util.ListToFile(FileData.GoddedPlayers, GodListFile);
                player.SendSuccessMessage(string.Format(msg, "removed"));
                return;
            }
            FileData.GoddedPlayers.Add(target.Username);
            File.AppendAllText(GodListFile, target.Username + Environment.NewLine);
            player.SendSuccessMessage(string.Format(msg, "gave"));
        }

        [Command(nameof(TeleportToTarget), "Teleports you to another player.", "Usage: /tp [username]", new string[] { "tp", "teleport" }, true, true)]
        public static void TeleportToTarget(Player player, Player target)
        {
            player.Location.SetPosition(target.Location.GetPosition());
            player.SendSuccessMessage($"Successfully teleported to {target.FilteredUsername}");
        }

        [Command(nameof(TeleportToSender), "Teleports a player to you.", "Usage: /tphere [username]", new string[] { "tph", "tphere" }, true, true)]
        public static void TeleportToSender(Player player, Player target)
        {
            target.Location.SetPosition(player.Location.GetPosition());
            player.SendSuccessMessage($"Successfully teleported {target.FilteredUsername} to yourself.");
        }
        [Command("Kick", "Disconnects a player from the server for 10mins", "Usage: /kick [playerID] [reason]", new string[] { "kick" }, true, true)]
        public static void KickPlayer(Player player, Player target, string reason)
        {
            target.Kick(reason);
            player.SendSuccessMessage($"Successfully kicked {target.FilteredUsername} for {reason}.");
        }

        [Command("Ban", "Bans a player from the server", "Usage: /ban [playerID] [reason]", new string[] { "ban" }, true, true)]
        public static void BanPlayer(Player player, Player target, string reason)
        {
            target.Ban(reason);
            player.SendChatMessage(SvSendType.All, $"<color=green>{player.Username} banned {target.Username} for {reason}</color>");
            player.SendSuccessMessage($"Successfully banned {target.FilteredUsername} for {reason}.");
        }

        [Command("Discord Link", "Sends you the link to the discord server", "Usage: /discord", new string[] { "discord", "discordlink" }, true, true)]
        public static void SendDiscordLink(Player player)
        {
            player.SendInfoMessage($"Discord link: (link here)");
        }

        [Command("Mute", "Mutes a player on the server", "Usage: /mute [username]", new string[] { "mute" }, true)]
        public static void MutePlayer(Player player, Player target)
        {
            var msg = $"Successfully {{0}} {target.FilteredUsername}.";
            if (FileData.MutedPlayers.Contains(target.Username))
            {
                FileData.MutedPlayers.Remove(target.Username);
                Util.ListToFile(FileData.MutedPlayers, MuteFilePath);
                player.SendSuccessMessage(string.Format(msg, "unmuted"));
                return;
            }
            FileData.MutedPlayers.Add(target.Username);
            File.AppendAllText(MuteFilePath, target.Username + Environment.NewLine);
            player.SendSuccessMessage(string.Format(msg, "muted"));
        }
    }
}
