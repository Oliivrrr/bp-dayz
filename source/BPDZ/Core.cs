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
using System.ComponentModel.Design;
using BPDZ.Models;
using Newtonsoft.Json;
using UniversalUnityHooks;
using Logger = BP_API.Logger;
using Object = UnityEngine.Object;

namespace BPDZ
{
    public static class Core
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
            PlayerEvents.OnPlayerDamage += OnPlayerDamage;
            PlayerEvents.OnGlobalChatMessage += SvGlobalChatMessage;
            PlayerEvents.OnPlayerCrime += OnPlayerCrime;
            PlayerEvents.OnNpcSpawned += Zombies.SpawnZombie;
            ServerEvents.OnStartServer += OnStartServer;
            PlayerEvents.OnLocalChatMessage += OnLocalChatMessage;
            PlayerEvents.OnPlayerVoiceChat += OnPlayerVoiceChat;
            //PlayerEvents.OnPlayerVehicleEnter += OnPlayerVehicleEnter;
        }

        private static bool OnPlayerVoiceChat(Player player, ref byte[] voiceData)
        {
            foreach (var player2 in player.svPlayer.svManager.players.Where(x => x.Value.myItems.ContainsKey(2046665206) && x.Value != player.shPlayer))
                player2.Value.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.VoiceChat, player2.Value.ID, voiceData);
            return true;
        }

        private static bool OnLocalChatMessage(Player player, ref string message)
        {
            player.SendLocalMessage(message);
            return true;
        }

        [Hook("SvPlayer.SvGlobalChatMessage")]
        public static bool SvGlobalChatMessage(SvPlayer player, ref string message)
        {
            if (message.StartsWith("/"))
            {
                foreach (var player2 in player.svManager.players.Where(x => FileData.SpyPlayers.Contains(x.Value.username)))
                    Players.GetPlayerFromInternalList(player2.Value).SendChatMessage(SvSendType.Self, $"<color=magenta>[SPY CHAT] </color><color=#ff59ff>{player.player.username}: {message}</color>");
            }
            return false;
        }

        [Hook("SvPlayer.SvView")]
        public static bool SvView(SvPlayer player, ref int otherID)
        {
            if (player.GetTerritory() != null)
            {
                if (player.GetTerritory().ownerIndex != player.player.job.jobIndex)
                {
                    Players.GetPlayerFromInternalList(player.player).SendSuccessMessage($"Your gang must own this territory to access this");
                    return true;
                }
            }
            return false;
        }

        [Hook("SvPlayer.SvCollect")]
        public static bool SvCollect(SvPlayer player, ref int collectedID)
        {
            if (player.GetTerritory() != null)
            {
                if (player.GetTerritory().ownerIndex != player.player.job.jobIndex)
                {
                    Players.GetPlayerFromInternalList(player.player).SendSuccessMessage($"Your gang must own this territory to access this");
                    return true;
                }
            }
            return false;
        }

        [Hook("SvPlayer.SvEnterPlace")]
        public static bool SvEnterPlace(SvPlayer player, ref int doorID, ref ShPlayer sender)
        {
            if (sender.svPlayer.GetTerritory() != null)
            {
                if (sender.svPlayer.GetTerritory().ownerIndex != sender.job.jobIndex)
                {
                    Players.GetPlayerFromInternalList(sender).SendSuccessMessage($"Your gang must own this territory to access this");
                    return true;
                }
            }
            return false;
        }

        private static IEnumerator ContaminationLoop(Player player)
        {
            if (player.shPlayer.GetPosition().x < -999999 && !player.IsServerSide())
            {
                player.SendSuccessMessage("You have entered a Contaminated Area! You will take damage if you are not wearing a gas mask!");
                while (player.shPlayer.GetPosition().x < -252 && !player.IsServerSide())
                {
                    yield return new WaitForSeconds(4f);
                    if (player.shPlayer.GetWearable(WearableType.Head) != null)
                    {
                        if (player.shPlayer.GetWearable(WearableType.Head).index != -1627168389)
                        {
                            player.svPlayer.Damage(DamageIndex.Null, 5f, player.shPlayer, player.shPlayer.headCollider);
                        }
                    }
                    else if (player.shPlayer.GetWearable(WearableType.Head) == null)
                    {
                        player.svPlayer.Damage(DamageIndex.Null, 5f, player.shPlayer, player.shPlayer.headCollider);
                    }
                }
                player.SendSuccessMessage("You left the Contaminated Area");
            }
        }

        private static bool OnPlayerVehicleEnter(Player player, ref int seat)
        {
            player.svPlayer.svManager.StartCoroutine(CarLoop(player));
            return false;
        }

        private static IEnumerator CarLoop(Player player)
        {
            yield return new WaitForSeconds(1f);
            while (player.shPlayer.curMount)
            {
                if (!player.Inventory.HasItem(1699387113))
                {
                    player.SendSuccessMessage("You ran out of fuel!");
                }
                yield return new WaitForSeconds(1);
                player.Inventory.RemoveItem(1699387113, 1);
            }
        }

        static void OnStartServer(SvManager svMan)
        {
            svMan.startMoney = 0;
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
                    player.svPlayer.SvAddCrime(CrimeIndex.Murder, victim);
                    return true;
                }
                else if (player.IsServerSide() && !victim.svEntity.serverside)
                {
                    player.SendChatMessage(SvSendType.All, $"<color=red>{Players.GetPlayerByID(victim.ID).Username} was killed by a zombie</color>");
                }
            }

            if (crimeIndex == CrimeIndex.Trespassing)
            {
                if (player.svPlayer.GetTerritory() != null && player.Job.PlayerJob is Gangster && player.IsServerSide())
                {
                    player.svPlayer.SvTrySetJob((byte)player.svPlayer.GetTerritory().ownerIndex, true, false);
                    player.svPlayer.svManager.StartCoroutine(LookForPlayers(player.svPlayer));
                    return false;
                }
                player.svPlayer.svManager.StartCoroutine(ContaminationLoop(player));
            }
            return true;
        }

        static bool OnPlayerDamage(Player player, Player attacker, ref DamageIndex type, ref float amount, ref Collider collider)
        {
            if (attacker != null && type == DamageIndex.Melee)
            {
                if (attacker.svPlayer.serverside)
                {
                    foreach (var zombie in Zombies.GetAliveZombies())
                    {
                        if (attacker.shPlayer.username == zombie.Player.player.username)
                        {
                            player.svPlayer.Damage(DamageIndex.Null, amount * zombie.Type.DamageMultiplier, player.shPlayer, collider);
                            if(GenerateRandom(1, 100) <= 2000/(int)player.Stats.Health)
                                FileData.InfectedPlayers.Add(player.Username);
                            return true;
                        }
                    }
                }
            }
            foreach (var name in FileData.GoddedPlayers)
            {
                if (name != player.Username)
                    continue;
                player.SendChatMessage($"<color=#fff>Blocked {amount}HP of damage</color>");
                return true;
            }
            return false;
        }

        static public IEnumerator PlayerBleed(Player player, float time)
        {
            player.shPlayer.StartEffect(EffectIndex.Intoxicated);
            while (!player.shPlayer.IsDead() && time != 0)
            {
                yield return new WaitForSeconds(10f);
                time = time - 10f;
                player.svPlayer.Damage(DamageIndex.Null, GenerateRandomF(7, 13), player.shPlayer, player.shPlayer.headCollider);
                player.SendSuccessMessage("You are bleeding. Consume medkits or get another player to heal you");
            }
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
            player.SendChatMessage(SvSendType.All, $"{player.Username} Joined for the first time");
            player.shPlayer.TransferItem(DeltaInv.AddToMe, -1975896234, 1, true);
            player.shPlayer.TransferItem(DeltaInv.AddToMe, 493970259, 35, true);
        }

        public static IEnumerator KillDelay(ShEntity entity, float time)
        {
            yield return new WaitForSeconds(time);
            if (entity != null)
            {
                entity.Destroy();
            }
        }

        public static int GenerateRandom(int min, int max) => Variables.Random.Next(min, max);
        public static float GenerateRandomF(float min, float max) => UnityEngine.Random.Range(min, max);

        [Command(nameof(SpawnNPC), "Spawn a Zombie.", "Usage: /spawnzombie [TypeID]", new string[] { "spawnzombie", "zombie"}, true, true)]
        public static void SpawnNPC(Player player, int id)
        {
            player.svPlayer.SpawnBot(player.shPlayer.GetPosition(), player.shPlayer.GetRotation(), player.shPlayer.GetPlace(), null, player.shPlayer.manager.skinPrefabs[id], null, player.shPlayer);
            player.SendSuccessMessage($"Successfully Spawned Zombie!");
        }

        [Command(nameof(Help), "Opens help menu.", "Usage: /help", new string[] { "help", "h" })]
        public static void Help(Player player)
        {
            player.SendServerInfoMessage(SvSendType.Self, File.ReadAllText("server_info.txt"));
            player.SendSuccessMessage($"Opened help menu");
        }

        [Command(nameof(Feed), "Refills all stat bars.", "Usage: /feed [username]", new string[] { "feed", "f" })]
        public static void Feed(Player player, Player target)
        {
            target.Stats.UpdateStats(100f, 100f, 100f, 100f);
            player.SendSuccessMessage($"Refilled Stats");
        }

        [Command(nameof(GangChat), "Sends a message to gang members", "Usage: /gangchat [message]", new string[] { "gangchat", "gc" }, false, true)]
        public static void GangChat(Player player, string message)
        {
            if (player.Job.PlayerJob is Gangster)
            {
                foreach (var player2 in player.svPlayer.svManager.players.Where(x => x.Value.job.jobIndex == player.Job.PlayerJob.jobIndex))
                    Players.GetPlayerFromInternalList(player2.Value).SendChatMessage(SvSendType.Self, $"<color=#{ColorUtility.ToHtmlStringRGB(player.Job.PlayerJob.info.jobColor)}>[{player.Job.PlayerJob.info.jobName.ToUpper()} CHAT] {player.Username}: {message}</color>");
            }
            player.SendSuccessMessage("You are not in a gang");
        }

        [Command(nameof(SetJob), "Sets the job for specified player.", "Usage: /setjob [username] [jobindex]", new string[] { "setjob" }, true, true)]
        public static void SetJob(Player player, Player target, byte jobIndex)
        {
            target.svPlayer.SvTrySetJob(jobIndex, true, false);
            player.SendSuccessMessage($"Gave {target.Username} the job {target.Job.PlayerJob.info.jobName}");
        }

        [Command(nameof(Sudo), "Executes a message for specified player.", "Usage: /sudo [username]", new string[] { "sudo" }, true, true)]
        public static void Sudo(Player player, Player target, string message)
        {
            target.svPlayer.SvGlobalChatMessage(message);
            player.SendStaffMessage($"Sudo'd {target.Username} with {message}");
        }

        [Command(nameof(StaffChatMessage), "Sends a message to staff chat.", "Usage: /sc [message]", new string[] { "sc", "staffchat" }, true, true)]
        public static void StaffChatMessage(Player player, string message)
        {
            player.SendStaffMessage($"{player.Username}: {message}");
        }

        [Command(nameof(ClearItems), "Clears the inventory of target player.", "Usage: /clearinventory [username]", new string[] { "clearinventory", "clearinv" }, true, true)]
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

        [Command(nameof(SpawnLootDrop), "Spawn a Loot Drop.", "Usage: /spawndrop [tier]", new string[] { "spawndrop", "lootdrop" }, true, true)]
        public static void SpawnLootDrop(Player player, int tier)
        {
            LootDrops.Initialize(player.shPlayer, tier);
            player.SendSuccessMessage($"Successfully Spawned Loot Drop!");
        }

        [Command(nameof(Atm), "Atm.", "Usage: /atm", new string[] { "atm" })]
        public static void Atm(Player player)
        {
            Help(player);
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

        [Command(nameof(Heal), "Heals the player back to full HP.", "Usage: /heal [username]", new string[] { "heal" }, true)]
        public static void Heal(Player player, Player target)
        {
            target.Stats.RestoreHealth();
        }

        [Command(nameof(TeleportToTarget), "Teleports you to another player.", "Usage: /tp [username]", new string[] { "tp", "teleport" }, true, true)]
        public static void TeleportToTarget(Player player, Player target)
        {
            player.svPlayer.SvTeleport(target.ID);
            player.SendSuccessMessage($"Successfully teleported to {target.FilteredUsername}");
        }

        [Command(nameof(Give), "Gives the player an item.", "Usage: /give [itemID]", new string[] { "give" }, true, true)]
        public static void Give(Player player, int itemID, int amount)
        {
            player.shPlayer.TransferItem(DeltaInv.AddToMe, itemID, amount, true);
            player.SendSuccessMessage($"Successfully gave you {amount} {player.Inventory.GetItem(itemID).item.itemName}(s)");
        }

        [Command(nameof(TeleportToSender), "Teleports a player to you.", "Usage: /tphere [username]", new string[] { "tph", "tphere" }, true, true)]
        public static void TeleportToSender(Player player, Player target)
        {
            target.svPlayer.SvTeleport(player.ID);
            player.SendSuccessMessage($"Successfully teleported {target.FilteredUsername} to yourself.");
        }

        [Command("Kick", "Disconnects a player from the server for 10mins", "Usage: /kick [playerID] [reason]", new string[] { "kick" }, true, true)]
        public static void KickPlayer(Player player, Player target, string reason)
        {
            target.Kick(reason);
            player.SendSuccessMessage($"Successfully kicked {target.FilteredUsername} for {reason}.");
        }

        [Command("Disconnect", "Disconnects a player from the server", "Usage: /disconnect [playerID]", new string[] { "disconnect" }, true, true)]
        public static void Disconnect(Player player, Player target, string reason)
        {
            player.SendSuccessMessage($"Successfully disconnected {target.FilteredUsername}.");
            target.svPlayer.svManager.Disconnect(target.svPlayer.connection, DisconnectTypes.Normal);
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
            player.SendInfoMessage($"Discord link: {FileData.DiscordLink}");
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

        [Command("ToggleSpyChat", "Toggles Spy chat; While on you see players entering commands", "Usage: /spychat", new string[] { "spy", "spychat" }, true)]
        public static void ToggleSpyChat(Player player)
        {
            string msg = $"Successfully {{0}}.";
            if (FileData.SpyPlayers.Contains(player.Username))
            {
                FileData.SpyPlayers.Remove(player.Username);
                Util.ListToFile(FileData.SpyPlayers, SpyListFile);
                player.SendSuccessMessage(string.Format(msg, "disabled spychat"));
                return;
            }
            FileData.SpyPlayers.Add(player.Username);
            File.AppendAllText(SpyListFile, player.Username + Environment.NewLine);
            player.SendSuccessMessage(string.Format(msg, "enabled spychat"));
        }
    }
}
