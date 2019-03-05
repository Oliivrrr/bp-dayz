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
        public static void SvGlobalChatMessage(Player player, string message)
        {
            string groupscolor = "";
            string groupsdisplayname = "";
            List<DayZGroups> groups = GroupGrabber(player, false).OrderByDescending(o => o.PermissionLevel).ToList();
            foreach (var c in groups)
            {
                groupsdisplayname = $"{groupsdisplayname} {c.DisplayName}";
                groupscolor = $"{groupscolor} {c.Color}";
            }
            
            if (message.StartsWith("/"))
            {
                Debug.Log($"[BPDayZLogger] Command sent: [{player.svPlayer.player.ID}]  {player.svPlayer.player.username}: {message}");
                SearchCommand(player, message);
                return;
            }

            else if (!player.svPlayer.svManager.chatted.OverLimit(player.svPlayer.player))
            {                
                Debug.Log($"[BPDayZLogger] Message sent: [{player.svPlayer.player.ID}]  {player.svPlayer.player.username}: {message}");
                player.svPlayer.svManager.chatted.Add(player.svPlayer.player);
                player.svPlayer.Send(SvSendType.All, Channel.Unsequenced, ClPacket.GameMessage, $" {player.svPlayer.player.username}: {message}");
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
                result = 5;
            }
            DayZGroups previous = new DayZGroups { Name = "null", DisplayName = "null", PermissionLevel = "0", Color = "null"};
            foreach (var re in GroupGrabber(player, false))
            {
                if(Convert.ToInt32(re.PermissionLevel) < Convert.ToInt32(previous.PermissionLevel))
                {
                    result = Convert.ToInt32(re.PermissionLevel);
                }
            }

            return result;
        }

        public static void RunCommand(Player player, List<string> args)
        {
            if (args[0] == @"/god")
            {
                
                if(GetPermissionLevel(player) <= GodMode.PermissionLevel)
                GodMode.Run(args[1]);
                SendMessage(player.svPlayer, $"<color=green>[BPDayZ]: </color><color=blue>Succesfully gave Godmode to {args[1]}</color>");
                Debug.Log($"[BPDayZ]: Successfully gave Godmode to player {args[1]}");
            }

            else if (args[0] == @"/help")
            {
                Help.SendHelpMenu(player);
            }

            else if (args[0] == @"/home")
            {
                player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>Work in progress</color>");
            }

            else if (args[0] == @"/safezone")
            {
                player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>Work in progress</color>");
            }

            else if (args[0] == @"/discord")
            {
                Discord.SendDiscordLink(player);
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

        public static List<DayZGroups> GroupGrabber(Player player, bool withColor)
        {
            List<DayZGroups> ListOfGroups = new List<DayZGroups>();
            List<DayZGroups> playersGroups = new List<DayZGroups>();
            string filepath = @"BPDayZ/Groups/";
            DirectoryInfo directory = new DirectoryInfo(filepath);

            foreach (var files in directory.GetFiles("*.txt"))
            {
                string [] Contents = File.ReadAllLines($"BPDayZ/Groups/{files.Name}");
                ListOfGroups.Add(new DayZGroups { Name = files.Name, DisplayName = $"[{files.Name}]", Color = $"{Contents[0]}", PermissionLevel = Contents[1] });
            }

            foreach (var groups in ListOfGroups)
            {
                string [] Contents = File.ReadAllLines($"BPDayZ/Groups/{groups.Name}");
                if (Contents.Contains(player.Username))
                {
                    playersGroups.Add(groups);
                }
            }
            return playersGroups;
        }
    }
}
