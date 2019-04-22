﻿/*
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
using UniversalUnityHooks;
using Logger = BP_API.Logger;
using Object = UnityEngine.Object;

namespace BPDZ
{
    public class Core
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
            PlayerEvents.OnNpcSpawned += Zombies.SpawnZombie;
            ServerEvents.OnStartServer += OnStartServer;
            //PlayerEvents.OnPlayerVehicleEnter += OnPlayerVehicleEnter;
        }

        private static IEnumerator ContaminationLoop(Player player)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (player.shPlayer.GetPosition().x < -252)
                {
                    player.SendSuccessMessage("You have entered a Contaminated Area! You will take damage if you are not wearing a gas mask!");
                    yield return new WaitForSeconds(4f);
                    if (player.shPlayer.GetWearable(WearableType.Head).ID == -1627168389)
                    {
                        player.svPlayer.Damage(DamageIndex.Null, 5f, player.shPlayer, player.shPlayer.headCollider);
                    }
                }
            }
            
        }

        private static bool OnPlayerVehicleEnter(Player player, ref int seat)
        {
            player.svPlayer.svManager.StartCoroutine(CarLoop(player));
            return false;
        }

        private static IEnumerator CarLoop(Player player)
        {
            float time = 60f;
            yield return new WaitForSeconds(1f);
            while (player.shPlayer.curMount)
            {
                if (!player.Inventory.HasItem(1699387113))
                {
                    player.SendSuccessMessage("You ran out of fuel!");
                }
                yield return new WaitForSeconds(time);
                player.Inventory.RemoveItem(1699387113, 1);
            }
        }

        static void OnStartServer(SvManager svMan)
        {
            svMan.startMoney = 0;
            svMan.StartCoroutine(SpawnLootLoop(SvMan));
        }

        private static IEnumerator SpawnLootLoop(SvManager svMan)
        {
            while (true)
            {
                yield return new WaitForSeconds(60);
                ShPlayer randomPlayer = svMan.GetRandomRealPlayer();
                if (randomPlayer != null && randomPlayer.ground && randomPlayer.curMount == null && randomPlayer.GetPlaceIndex() == 0)
                {
                    LootDrops.Initialize(randomPlayer);
                }
            }
        }

        static public IEnumerator LookForPlayers(SvPlayer player)
        {
            while (!player.player.IsDead())
            {
                yield return new WaitForSeconds(5f);
                foreach (Sector sector in player.localSectors)
                {
                    foreach (ShEntity shEntity in sector.centered)
                    {
                        if (!(shEntity == player.entity))
                        {
                            ShPlayer shPlayer = shEntity as ShPlayer;
                            if (shPlayer && player.player.CanSeeEntity(shPlayer) && shPlayer.job.jobIndex != player.player.job.jobIndex)
                            {
                                player.targetEntity = shPlayer;
                                player.SetState(10);
                            }
                        }
                    }
                }
            }
        }

        static bool OnPlayerCrime(Player player, ref byte crimeIndex, ref ShEntity victim)
        {
            if (crimeIndex == CrimeIndex.Murder)
            {
                if (!player.IsServerSide() && !victim.svEntity.serverside)
                {
                    player.SendChatMessage(SvSendType.All, $"<color=red>{player.Username} killed {Players.GetPlayerByID(victim.ID).Username} using {player.shPlayer.curEquipable.itemName}</color>");
                    Debug.Log($"[BPDZ] {player.Username} killed {Players.GetPlayerByID(victim.ID).Username}");
                }
                else if (player.IsServerSide() && !victim.svEntity.serverside)
                {
                    player.SendChatMessage(SvSendType.All, $"<color=red>{Players.GetPlayerByID(victim.ID).Username} was killed by a zombie</color>");
                }
            }
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

            if (type == DamageIndex.Melee)
            {
                foreach (var zombie in Zombies.GetAliveZombies())
                {
                    if (attacker.Username == zombie.Player.player.username && zombie.Type != null)
                    {
                        amount = amount * zombie.Type.DamageMultiplier;
                    }
                }
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
            player.svPlayer.svManager.StartCoroutine(ContaminationLoop(player));
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
            int num = GenerateRandom(1, 3);
            Vector3 f = new Vector3(curPosition.x - (-8 * num), curPosition.y, curPosition.z - (-8 ^ num)); // Still working on better position management
            return f;
        }

        public static IEnumerator KillDelay(ShEntity entity, float time)
        {
            yield return new WaitForSeconds(time);
            entity.Destroy();
        }

        public static int GenerateRandom(int min, int max) => Variables.Random.Next(min, max);
        public static float GenerateRandomF(float min, float max) => UnityEngine.Random.Range(min, max);

        [Command(nameof(SpawnNPC), "Spawn a Zombie.", "Usage: /spawnzombie [TypeID]", new string[] { "spawnzombie", "zombie"}, true, true)]
        public static void SpawnNPC(Player player, int id)
        {
            ShEntity zombie = player.svPlayer.svManager.AddNewEntity(player.shPlayer.manager.skinPrefabs[id], player.shPlayer.GetPlace(), player.shPlayer.GetPosition(), player.shPlayer.GetRotation(), false);
            foreach (var item in zombie.myItems.Values)
            {
                zombie.RemoveFromMyItems(item.item.ID, item.count);
            }
            player.SendSuccessMessage($"Successfully Spawned Zombie!");
        }

        [Command(nameof(ClearItems), "Clears the inventory of target player.", "Usage: /clear [username]", new string[] { "spawnzombie", "zombie" }, true, true)]
        public static void ClearItems(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            foreach (var item in targetPlayer.shPlayer.myItems)
            {
                player.Inventory.RemoveItem(item.Value.item.ID, item.Value.count);
            }
            player.SendSuccessMessage($"Successfully Cleared Player {targetPlayer.Username} Inventory");
            targetPlayer.SendSuccessMessage($"Your Inventory Was Cleared by {player.Username}");
        }

        [Command(nameof(SpawnLootDrop), "Spawn a Loot Drop.", "Usage: /spawndrop [Tier]", new string[] { "spawndrop", "lootdrop" }, true, true)]
        public static void SpawnLootDrop(Player player, int id)
        {
            LootDrops.Initialize(player.shPlayer);
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
