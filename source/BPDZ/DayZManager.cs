using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using BP_API;

namespace BPDZ
{
    public class DayZManager
    {
        public static void SvGlobalChatMessage(Player player, string message)
        {
            if (!File.ReadAllLines(DayZCore.MuteFilePath).Contains(player.Username))
            {
				var groupsdisplayname = "";
				var groups = GroupGrabber(player).OrderByDescending(o => o.PermissionLevel).ToList();
                foreach (var group in groups)
                {
                    groupsdisplayname = $"{groupsdisplayname} <color={group.Color}>{group.DisplayName} </color>";
                }

                if (!player.svPlayer.svManager.chatted.OverLimit(player.svPlayer.player))
                {
                    player.svPlayer.svManager.chatted.Add(player.svPlayer.player);
                    player.svPlayer.Send(SvSendType.All, Channel.Unsequenced, ClPacket.GameMessage, $"[{player.ID}] {groupsdisplayname} {player.svPlayer.player.username}: {message}");
                }
            }
        }

        public static List<DayZGroups> GroupGrabber(Player player)
        {
			var filepath = @"BPDayZ/Groups/";
			var ListOfGroups = new List<DayZGroups>();
			var playersGroups = new List<DayZGroups>();
			var directory = new DirectoryInfo(filepath);

            foreach (var files in directory.GetFiles("*.txt"))
            {
				var Contents = File.ReadAllLines(files.FullName);
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
