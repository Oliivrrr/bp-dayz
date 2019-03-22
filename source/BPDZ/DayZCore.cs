using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BPDayZ;
using System.IO;
using static BP_API.Core;

namespace BPDZ
{
    class DayZCore
    {
        public static string GodListFile = @"BPDayZ/GodList.txt";
        public static string MuteFilePath = @"BPDayZ/MuteList.txt";
        public static string CommandzFilePath = @"BPDayZ/HelpMessage.txt";
        public static string[] ListOfCommands = File.ReadAllLines(CommandzFilePath);
        public const string ResourceName = "BPDayZ";

        [EntryPoint(ResourceName)]
        public static void Main()
        {
            Debug.Log($"[BPDZ] {ResourceName} being loaded in");
            if (!Directory.Exists(@"BPDayZ"))
            {
                Debug.Log("[BPDZ] Creating directory BPDayZ...");
                Directory.CreateDirectory("BPDayZ/Groups");
                Debug.Log("[BPDZ] Successfully created directory");
                Debug.Log("[BPDZ] Creating files...");
                File.Create("BPDayZ/GodList.txt");
                File.Create("BPDayZ/DiscordLink.txt");
                File.Create("BPDayZ/HelpMessage.txt");
                File.Create("BPDayZ/MuteList.txt");
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
            PlayerEvents.OnPlayerDisconnected += OnPlayerDisconnected;
            PlayerEvents.OnPlayerDamage += OnPlayerDamage;
            PlayerEvents.OnGlobalChatMessage += SvGlobalChatMessage;
        }

        static bool OnPlayerDamage(Player player, Player attacker, ref DamageIndex type, ref float amount, ref Collider collider)
        {
            foreach (var name in File.ReadAllLines(GodListFile))
            {
                if (name == player.Username)
                {
                    player.SendChatMessage(SvSendType.Self, $"<color=white>Blocked {amount}HP of damage</color>");
                    return true;
                }
            }
            return false;
        }

        static bool SvGlobalChatMessage(Player player, ref string message)
        {
            DayZManager.SvGlobalChatMessage(player, message);
            Loggers.Chat.Log($"[{player.svPlayer.player.ID}]  {player.svPlayer.player.username}: {message}");
            return true;
        }
        static void OnPlayerConnected(Player player)
        {
            Debug.Log($"[{player.ID}] {player.Username} Joined the server ({player.UserData.GetIpV4()})");
            if (player.svPlayer.playerData.username != null)
                return;
            player.Inventory.AddItem(-1975896234, 1);
            player.Inventory.AddItem(-1627168389, 1);
            player.Inventory.AddItem(493970259, 35);
        }

        static void OnPlayerDisconnected(Player player)
        {
            Debug.Log($"[{player.ID}] {player.Username} Left the server");
        }

        public static int RNG(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        [Command("Godmode", "Prevents the player from taking damage.", "Usage: /god [username]", new string[] { "godmode", "god" }, true)]
        public static void Godmode(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            string list = "";
            string[] allusers = File.ReadAllLines(GodListFile);

            if (File.ReadAllText(GodListFile).Contains(targetPlayer.Username))
            {
                foreach (var user in allusers)
                {
                    if (user != targetPlayer.Username)
                    {
                        list = $"{list} \n {user}";
                    }
                }
                File.WriteAllText(GodListFile, list);
                player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully removed godmode from {targetPlayer.Username}</color>");
            }
            else
            {
                foreach (var user in allusers)
                {
                    if (user != player.Username)
                    {
                        list = $"{list}\n{user}";
                    }
                }
                list = $"{list}\n{targetPlayer.Username}";
                File.WriteAllText(GodListFile, list);
                player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully gave godmode to {targetPlayer.Username}</color>");
            }
        }

        [Command("Teleport", "Teleports you to another player.", "Usage: /tp [username]", new string[] { "tp", "teleport" }, true, true)]
        public static void TeleportToTarget(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            player.Location = targetPlayer.Location;
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully teleported to {targetPlayer.Username}</color>");
        }

        [Command("Teleport here", "Teleports a player to you.", "Usage: /tphere [username]", new string[] { "tph", "tphere" }, true, true)]
        public static void TeleportToSender(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.Location = player.Location;
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully teleported {targetPlayer.Username} to yourself</color>");
        }

        [Command("Kick", "Disconnects a player from the server for 10mins", "Usage: /kick [playerID] [reason]", new string[] { "kick" }, true, true)]
        public static void KickPlayer(Player player, string target, string reason)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were kicked by {player.Username} for {reason}");
            targetPlayer.Kick(reason);
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Kicked {targetPlayer.Username} for {reason}</color>");
        }

        [Command("Ban", "Bans a player from the server", "Usage: /ban [playerID] [reason]", new string[] { "ban" }, true, true)]
        public static void BanPlayer(Player player, string target, string reason)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were banned by {player.Username} for {reason}");
            targetPlayer.Ban(reason);
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Banned {targetPlayer.Username} for {reason}</color>");
        }

        [Command("Discord Link", "Sends you the link to the discord server", "Usage: /discord", new string[] { "discord", "discordlink" }, true, true)]
        public static void SendDiscordLink(Player player)
        {
            string discordlink = File.ReadAllText(@"BPDayZ/DiscordLink.txt");
            player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]:</color><color=yellow> {discordlink} </color>");
        }

        [Command("Help", "Opens a menu containing all commands", "Usage: /help", new string[] { "help" })]
        public static void SendHelpMenu(Player player)
        {
            string text = "";
            foreach (var item in ListOfCommands)
            {
                text = text + "\n" + item;
            }
            player.svPlayer.Send(SvSendType.Self, Channel.Fragmented, ClPacket.ServerInfo, $"<color=green>[BPDayZ] List of commands </color> " + text);
        }

        [Command("Mute", "Mutes a player on the server", "Usage: /mute [username] [reason]", new string[] { "mute" }, true)]
        public static void MutePlayer(Player player, string victim)
        {
            string[] data = File.ReadAllLines(MuteFilePath);
            Player targetPlayer = Players.GetPlayerByUsername(victim);
            string list = "";
            if (data.Contains(victim))
            {
                foreach(var user in data)
                {
                    if (user != victim)
                    {
                        list = $"{list}\n{user}";
                    }
                }
                File.WriteAllText(MuteFilePath, list);
                player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully unmuted {targetPlayer.Username}</color>");
            }

            else
            {
                foreach (var user in data)
                {
                    list = $"{list}\n{user}";
                }
                list = $"{list}\n{victim}";
                File.WriteAllText(MuteFilePath, list);
                player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully muted {targetPlayer.Username}</color>");
            }
        }
    }
}
