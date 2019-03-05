using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ.Commands
{
    class Help
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

        public static void SendHelpMenu(Player player)
        {
            string text = "";
            foreach (var item in ListOfCommands)
            {
                text = text + "\n" + item;
            }
            player.svPlayer.Send(SvSendType.Self, Channel.Fragmented, ClPacket.ServerInfo, $"<color=green>[BPDayZ] List of commands </color> " + text);
        }
    }
}
