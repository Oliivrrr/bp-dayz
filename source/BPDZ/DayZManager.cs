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

namespace BPDZ
{
    public class DayZManager
    {



        public static List<string> ListOfCommands = new List<string>
        {
            @"/god [username] - toggles godmode for the player",
            @"/help - displays the list of commands",
            @"/home - teleports you to your house",
            @"/safezone - teleports you to the nearest safezone",
            @"/discord - shows the discord link for PPServers",
            @"",
            @"<color=yellow>More to come</color>"
        };

        
        public static void SvGlobalChatMessage(Player instance, string message)
        {

            string groupscolor = "";
            string groupsdisplayname = "";
            List<DayZGroups> groups = GroupGrabber(instance.svPlayer, false).OrderByDescending(o => o.PermissionLevel).ToList();
            foreach (var c in groups)
            {
                groupsdisplayname = $"{groupsdisplayname} {c.DisplayName}";
                groupscolor = $"{groupscolor} {c.Color}";
            }
            
            if (message.StartsWith("/"))
            {
                Debug.Log($"[BPDayZLogger] Command sent: [{instance.svPlayer.player.ID}]  {instance.svPlayer.player.username}: {message}");
                SearchCommand(instance.svPlayer, message);
                return;
            }

            else if (!instance.svPlayer.svManager.chatted.OverLimit(instance.svPlayer.player))
            {                
                Debug.Log($"[BPDayZLogger] Message sent: [{instance.svPlayer.player.ID}]  {instance.svPlayer.player.username}: {message}");
                instance.svPlayer.svManager.chatted.Add(instance.svPlayer.player);
                instance.svPlayer.Send(SvSendType.All, Channel.Unsequenced, ClPacket.GameMessage, $" {instance.svPlayer.player.username}: {message}");
                return;
            }
        }

        public static void SearchCommand(SvPlayer instance, string message)
        {
            List<string> newmessage = message.Split(' ').ToList();
            RunCommand(instance, newmessage);
        }

        public static int GetPermissionLevel(SvPlayer instance)
        {
            int result = 0;
            if(instance.player.admin)
            {
                result = 5;
            }
            DayZGroups previous = new DayZGroups { Name = "null", DisplayName = "null", PermissionLevel = "0", Color = "null"};
            foreach (var re in GroupGrabber(instance, false))
            {
                if(Convert.ToInt32(re.PermissionLevel) < Convert.ToInt32(previous.PermissionLevel))
                {
                    result = Convert.ToInt32(re.PermissionLevel);
                }
            }

            return result;
        }

        public static void RunCommand(SvPlayer instance, List<string> args)
        {
            if (args[0] == @"/god")
            {
                
                if(GetPermissionLevel(instance) <= GodMode.PermissionLevel)
                GodMode.Run(args[1]);
                SendMessage(instance, $"<color=green>[BPDayZ]: </color><color=blue>Succesfully gave Godmode to {args[1]}</color>");
                Debug.Log($"[BPDayZ]: Successfully gave Godmode to player {args[1]}");
            }

            else if (args[0] == @"/help")
            {
                string text = "";
                foreach (var item in ListOfCommands)
                {
                    text = text + "\n" + item;
                }
                instance.Send(SvSendType.Self, Channel.Fragmented, ClPacket.ServerInfo, $"<color=green>[BPDayZ] List of commands </color> " + text);
                return;
            }

            else if (args[0] == @"/home")
            {
                instance.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>You do not have permission to use this command</color>");
                return;
            }

            else if (args[0] == @"/safezone")
            {
                instance.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]: </color><color=red>You do not have permission to use this command</color>");
                return;
            }

            else if (args[0] == @"/discord")
            {
                StreamReader sr = new StreamReader(@"BPDayZ/DiscordLink.txt");
                string discordlink = sr.ReadToEnd();
                sr.Close();
                instance.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]:</color><color=yellow> {discordlink} </color>");
                return;
            }

            else
            {
                SendMessage(instance, $"<color=green>[BPDayZ]: </color><color=red>Unknown Command</color> <color=yellow>\"{args[0]}\"</color><color=red>. Do /help for the list of commands</color>");
                return;
            }
        }

        public static void SendMessage(SvPlayer instance, string message)
        {
            instance.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"{message}");
        }

        public static string ReadData(string location)
        {
            StreamReader sr = new StreamReader(location);
            string data = sr.ReadToEnd();
            sr.Close();
            return data;
        }

        public static void WriteData(string location, string text)
        {
            StreamReader sr = new StreamReader(location);
            string alldata = sr.ReadToEnd();
            sr.Close();

            StreamWriter sw = new StreamWriter(location);
            
            foreach(var c in alldata)
            {
                sw.WriteLine(c);
            }
            sw.WriteLine(text);
            sw.Close();
        }

        public static List<DayZGroups> GroupGrabber(SvPlayer instance, bool withColor)
        {
            List<DayZGroups> ListOfGroups = new List<DayZGroups>();
            List<DayZGroups> playersGroups = new List<DayZGroups>();
            string filepath = @"BPDayZ/Groups/";
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.txt"))
            {
                List<string> Contents = ReadData($"BPDayZ/Groups/{file.Name}").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                ListOfGroups.Add(new DayZGroups { Name = file.Name, DisplayName = $"[{file.Name}]", Color = $"{Contents[0]}", PermissionLevel = Contents[1] });
            }

            foreach (var re in ListOfGroups)
            {
                List<string> Contents = ReadData($"BPDayZ/Groups/{re.Name}").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                if (Contents.Contains(instance.player.username))
                {
                    playersGroups.Add(re);
                }
            }
            return playersGroups;
        }
    }
}
