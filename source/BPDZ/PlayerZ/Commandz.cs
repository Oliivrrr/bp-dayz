using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using BP_API;
using System.Reflection;

namespace BPDZ
{
    class Commandz
    {
        public static List<string> ListOfCommands = new List<string>
        {
            @"/god [username] - toggles godmode for the player",
            @"/help - displays the list of commands",
            @"/home - teleports you to your house",
            @"/safezone - teleports you to the nearest safezone",
            @"/discord - shows the link to the discord server",
            @"",
            @"<color=yellow>More to come</color>"
        };
        public static string GodListFile = @"BPDayZ/GodList.txt";
        public static string MuteFilePath = @"BPDayZ/MuteList.txt";


        [Command("Godmode", "Prevents the player from taking damage.", "Usage: /god [username]", new string[] { "godmode", "god" }, true)]
        public static void Godmode(Player player, string message)
        {
            if (File.ReadAllText(GodListFile).Contains(player.Username))
            {
                List<string> allusers = File.ReadAllLines(GodListFile).ToList();
                allusers.Remove(player.Username);
                File.WriteAllLines(GodListFile, allusers);
            }

            else
            {
                List<string> allusers = File.ReadAllText(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                allusers.Add(player.Username);
                File.WriteAllText(GodListFile, allusers.ToString());
            }
        }

        [Command("Teleport", "Teleports you to another player.", "Usage: /tp [username]", new string[] { "tp", "teleport" }, true, true)]
        public static void TeleportToTarget(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            player.Location = targetPlayer.Location;
        }

        [Command("Teleport here", "Teleports a player to you.", "Usage: /tphere [username]", new string[] { "tph", "tphere" }, true, true)]
        public static void TeleportToSender(Player player, string target)
        {
            Players.GetPlayerByUsername(target).Location = player.Location;
        }

        [Command("Kick", "Disconnects a player from the server for 10mins", "Usage: /kick [playerID] [reason]", new string[] { "kick" }, true, true)]
        public static void KickPlayer(Player player, string message, string target, string reason)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were kicked by {player.Username} for {reason}");
            targetPlayer.Kick(reason);
        }

        [Command("Ban", "Bans a player from the server", "Usage: /ban [playerID] [reason]", new string[] { "ban" }, true, true)]
        public static void BanPlayer(Player player, string message, string target, string reason)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were banned by {player.Username} for {reason}");
            targetPlayer.Ban(reason);
        }

        [Command("Discord Link", "Sends you the link to the discord server", "Usage: /discord", new string[] { "discord", "discordlink" }, true, true)]
        public static void SendDiscordLink(Player player)
        {
            string discordlink = File.ReadAllText(@"BPDayZ/DiscordLink.txt");
            player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]:</color><color=yellow> {discordlink} </color>");
        }

        [Command("Help", "Opens a menu containing all commands", "Usage: /help", new string[]{"help"})]
        public static void SendHelpMenu(Player player)
        {
            string text = "";
            foreach (var item in ListOfCommands)
            {
                text = text + "\n" + item;
            }
            player.svPlayer.Send(SvSendType.Self, Channel.Fragmented, ClPacket.ServerInfo, $"<color=green>[BPDayZ] List of commands </color> " + text);
        }

        [Command("Mute", "Mutes a player on the server", "Usage: /mute [username] [reason]", new string[] { "ban" }, true, true)]
        public static void MutePlayer(Player sender, string victim)
        {
            List<string> data = File.ReadAllLines(MuteFilePath).ToList();
            if (data.Contains(victim))
            {
                data.Remove(victim);
                File.WriteAllLines(MuteFilePath, data.ToArray());
            }

            else
            {
                data.Add(victim);
                File.WriteAllLines(MuteFilePath, data.ToArray());
            }
        }
    }
}
