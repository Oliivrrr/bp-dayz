using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BPDZ.Commands
{
    class Discord
    {
        public static void SendDiscordLink(Player player)
        {
            string discordlink = File.ReadAllText(@"BPDayZ/DiscordLink.txt");
            player.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color=green>[BPDayZ]:</color><color=yellow> {discordlink} </color>");
        }
    }
}
