﻿/*
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
