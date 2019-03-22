/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
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
        }

        static bool OnPlayerDamage(Player player, Player attacker, ref DamageIndex type, ref float amount, ref Collider collider)
        {
			foreach (var name in Lists.GoddedPlayers)
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
            Loggers.Misc.Log($"[{player.ID}] {player.Username} Joined the server ({player.UserData.GetIpV4()})");
            if (player.svPlayer.playerData.username != null)
                return;
            player.Inventory.AddItem(-1975896234, 1);
            player.Inventory.AddItem(-1627168389, 1);
            player.Inventory.AddItem(493970259, 35);
        }

        static void OnPlayerDisconnected(Player player)
        {
			Loggers.Misc.Log($"[{player.ID}] {player.Username} Left the server");
        }

        public static int GenerateRandom(int min, int max) => Variables.Random.Next(min, max);



		[Command("Godmode", "Prevents the player from taking damage.", "Usage: /god [username]", new string[] { "godmode", "god" }, true)]
        public static void Godmode(Player player, string target)
        {
            var targetPlayer = Players.GetPlayerByUsername(target);
            var list = "";
            var allusers = File.ReadAllLines(GodListFile);

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
                player.SendChatMessage($"<color=green>[BPDZ]</color> <color=red>Successfully removed godmode from {targetPlayer.Username}</color>");
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
			var targetPlayer = Players.GetPlayerByUsername(target);
            player.Location = targetPlayer.Location;
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully teleported to {targetPlayer.Username}</color>");
        }

        [Command("Teleport here", "Teleports a player to you.", "Usage: /tphere [username]", new string[] { "tph", "tphere" }, true, true)]
        public static void TeleportToSender(Player player, string target)
        {
			var targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.Location = player.Location;
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Successfully teleported {targetPlayer.Username} to yourself</color>");
        }

        [Command("Kick", "Disconnects a player from the server for 10mins", "Usage: /kick [playerID] [reason]", new string[] { "kick" }, true, true)]
        public static void KickPlayer(Player player, string target, string reason)
        {
			var targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were kicked by {player.Username} for {reason}");
            targetPlayer.Kick(reason);
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Kicked {targetPlayer.Username} for {reason}</color>");
        }

        [Command("Ban", "Bans a player from the server", "Usage: /ban [playerID] [reason]", new string[] { "ban" }, true, true)]
        public static void BanPlayer(Player player, string target, string reason)
        {
			var targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were banned by {player.Username} for {reason}");
            targetPlayer.Ban(reason);
            player.SendChatMessage(SvSendType.Self, $"<color=green>[BPDZ]</color> <color=red>Banned {targetPlayer.Username} for {reason}</color>");
        }

        [Command("Discord Link", "Sends you the link to the discord server", "Usage: /discord", new string[] { "discord", "discordlink" }, true, true)]
        public static void SendDiscordLink(Player player)
        {
			var discordlink = File.ReadAllText(@"BPDayZ/DiscordLink.txt");
            player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]:</color><color=yellow> {discordlink} </color>");
        }

   // todo
   //     [Command("Help", "Opens a menu containing all commands", "Usage: /help", new string[] { "help" })]
   //     public static void SendHelpMenu(Player player)
   //     {
			//var text = "";
   //         foreach (var item in ListOfCommands)
   //         {
   //             text = text + "\n" + item;
   //         }
   //         player.svPlayer.Send(SvSendType.Self, Channel.Fragmented, ClPacket.ServerInfo, $"<color=green>[BPDayZ] List of commands </color> " + text);
   //     }

        [Command("Mute", "Mutes a player on the server", "Usage: /mute [username] [reason]", new string[] { "mute" }, true)]
        public static void MutePlayer(Player player, string victim)
        {
			var data = File.ReadAllLines(MuteFilePath);
			var targetPlayer = Players.GetPlayerByUsername(victim);
			var list = "";
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
