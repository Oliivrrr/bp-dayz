/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using BP_API;
using static BPDZ.Variables;
using System.Collections;

namespace BPDZ
{
    public static class Manager
    {
        public static string GroupGrabber(Player player)
        {
            var ListOfGroups = new List<DayZGroups>();
            var playersGroups = new List<DayZGroups>();
            var directory = new DirectoryInfo(GroupsFolder);

            foreach (var files in directory.GetFiles("*.txt"))
            {
                var contents = File.ReadAllLines(files.FullName).ToList();
                ListOfGroups.Add(new DayZGroups { Name = files.Name, DisplayName = $"[{Path.GetFileNameWithoutExtension(files.FullName)}]", Color = contents[0], PermissionLevel = contents[1], Players = contents });
            }

            foreach (var group in ListOfGroups)
            {
                var Players = new List<string>(group.Players);
                if (Players.Find(x => x == player.Username) != null)
                {
                    playersGroups.Add(group);
                }
            }
            var groupsdisplayname = "";
            List<DayZGroups> groups = playersGroups.OrderByDescending(o => o.PermissionLevel).ToList();
            foreach (var group in groups)
            {
                groupsdisplayname = $"{groupsdisplayname} <color={group.Color}>{group.DisplayName} </color>";
            }
            return groupsdisplayname;
        }
        public static void SvGlobalChatMessage(Player player, string message)
        {
            if (FileData.MutedPlayers.Contains(player.Username))
            {
                player.SendChatMessage($"&cYou are currently muted.");
                return;
            }

            if (!player.svPlayer.svManager.chatted.OverLimit(player.svPlayer.player))
            {
                player.svPlayer.svManager.chatted.Add(player.svPlayer.player);
                player.svPlayer.Send(SvSendType.All, Channel.Unsequenced, ClPacket.GameMessage, $"<color=#C4C4C4>[{player.ID}]</color> {GroupGrabber(player)} {player.svPlayer.player.username}: {message}");
            }
        }
    }
}
