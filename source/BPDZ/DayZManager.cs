using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalUnityHooks;
using System.IO;
using BP_API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BPDZ.Commands;

namespace BPDZ
{
    public class DayZManager
    {
        public static string commandprefix = "/";
        public static void SvGlobalChatMessage(Player player, string message)
        {
            string groupsdisplayname = "";
            List<DayZGroups> groups = GroupGrabber(player).OrderByDescending(o => o.PermissionLevel).ToList();
            foreach (var group in groups)
            {
                groupsdisplayname = $"{groupsdisplayname} <color={group.Color}>{group.DisplayName} </color>";
            }
            
            if (message.StartsWith(commandprefix))
            {
                Debug.Log($"[BPDayZLogger] Command sent: [{player.svPlayer.player.ID}]  {player.svPlayer.player.username}: {message}");
                SearchCommand(player, message);
                return;
            }

            else if (!player.svPlayer.svManager.chatted.OverLimit(player.svPlayer.player))
            {                
                Debug.Log($"[BPDayZLogger] Message sent: [{player.svPlayer.player.ID}]  {player.svPlayer.player.username}: {message}");
                player.svPlayer.svManager.chatted.Add(player.svPlayer.player);
                player.svPlayer.Send(SvSendType.All, Channel.Unsequenced, ClPacket.GameMessage, $"[{player.ID}] {groupsdisplayname} {player.svPlayer.player.username}: {message}");
                return;
            }
        }

        public static void SearchCommand(Player player, string message)
        {
            List<string> newmessage = message.Split(' ').ToList();
            RunCommand(player, newmessage);
        }

        public static int GetPermissionLevel(Player player)
        {
            int result = 0;
            if(player.shPlayer.admin)
            {
                result = 10;
            }
            DayZGroups previous = new DayZGroups { Name = "Scavenger.txt", DisplayName = "[Scavenger] ", PermissionLevel = "0", Color = "lime"};
            List<DayZGroups> playerZgroups = GroupGrabber(player);
            foreach (var group in playerZgroups)
            {
                if(Convert.ToInt32(group.PermissionLevel) < Convert.ToInt32(previous.PermissionLevel))
                {
                    result = Convert.ToInt32(group.PermissionLevel);
                }
            }
            return result;
        }

        public static void RunCommand(Player player, List<string> args)
        {
            if (args[0] == $"{commandprefix}god")
            {
                if(GetPermissionLevel(player) <= GodMode.PermissionLevel)
                GodMode.Run(args[1]);
                SendMessage(player.svPlayer, $"<color=green>[BPDayZ]: </color><color=blue>Succesfully gave Godmode to {args[1]}</color>");
                Debug.Log($"[BPDayZ]: Successfully gave Godmode to player {args[1]}");
            }

            else if (args[0] == $"{commandprefix}help")
            {
                Help.SendHelpMenu(player);
            }

            else if (args[0] == $"{commandprefix}home")
            {
                player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>Work in progress</color>");
            }

            else if (args[0] == $"{commandprefix}safezone")
            {
                player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>Work in progress</color>");
            }

            else if (args[0] == $"{commandprefix}discord")
            {
                Discord.SendDiscordLink(player);
            }

            else if (args[0] == $"{commandprefix}mute")
            {
                Mute.MutePlayer(player, args[1]);
            }

            else if (args[0] == $"{commandprefix}kill")
            {
                Kill.KillPlayer(player, args[1]);
            }

            else if (args[0] == $"{commandprefix}tp")
            {
                Teleport.TeleportToTarget(player, args[1]);
            }

            else if (args[0] == $"{commandprefix}tphere")
            {
                Teleport.TeleportToSender(player, args[1]);
            }

            else if (args[0] == $"{commandprefix}ban")
            {
                Ban.BanPlayer(player, args[1].TryParseInt(), args.ToString());
            }

            else if (args[0] == $"{commandprefix}kick")
            {
                Kick.KickPlayer(player, args[1], args.ToString());
            }

            else if (args[0] == $"{commandprefix}spawnhoard")
            {
                
            }

            else
            {
                SendMessage(player.svPlayer, $"<color=green>[BPDayZ]: </color><color=red>Unknown Command</color> <color=yellow>\"{args[0]}\"</color><color=red>. Do /help for the list of commands</color>");
            }
        }

        public static void SendMessage(SvPlayer instance, string message)
        {
            instance.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"{message}");
        }

        public static List<DayZGroups> GroupGrabber(Player player)
        {
            string filepath = @"BPDayZ/Groups/";
            List<DayZGroups> ListOfGroups = new List<DayZGroups>();
            List<DayZGroups> playersGroups = new List<DayZGroups>();
            DirectoryInfo directory = new DirectoryInfo(filepath);

            foreach (var files in directory.GetFiles("*.txt"))
            {
                string[] Contents = File.ReadAllLines(files.FullName);
                ListOfGroups.Add(new DayZGroups { Name = files.Name, DisplayName = $"[{Path.GetFileNameWithoutExtension(files.FullName)}]", Color = Contents[0], PermissionLevel = Contents[1], Players = Contents});
            }

            foreach(var group in ListOfGroups)
            {
                var Players = new List<string>(group.Players);
                if (Players.Find(x => x == player.Username) != null)
                {
                    playersGroups.Add(group);
                }
            }
            return playersGroups;
        }
    }
}
