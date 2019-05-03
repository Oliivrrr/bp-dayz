/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sends a message with a set prefix to the user.
        /// </summary>
        /// <param name="player">The user to send the message to.</param>
        /// <param name="prefix">The prefix that will be used.</param>
        /// <param name="str">The message that you want to send.</param>
        public static void SendPrefixedMessage(this Player player, string prefix, string str)
        {
            player.SendChatMessage(prefix + " " + str);
        }
        /// <summary>
        /// Sends a normal message prefixed with green [BPDZ] text.
        /// </summary>
        /// <param name="player">The user to send the message to</param>
        /// <param name="str">The message that you want to send. This text will be red.</param>
        public static void SendSuccessMessage(this Player player, string str)
        {
            SendPrefixedMessage(player, "<color=green>[BPDZ]</color><color=red>", str + "</color>");
        }
        /// <summary>
        /// Sends a normal message prefixed with green [BPDZ] text.
        /// </summary>
        /// <param name="player">The user to send the message to</param>
        /// <param name="str">The message that you want to send. This text will be yellow.</param>
        public static void SendInfoMessage(this Player player, string str)
        {
            SendPrefixedMessage(player, "<color=green>[BPDZ]</color><color=yellow>", str + "</color>");
        }
        /// <summary>
        /// Sends a message to all staff.
        /// </summary>
        /// <param name="player">The user to send the message to</param>
        /// <param name="str">The message that you want to send. This text will be mint coloured.</param>
        public static void SendStaffMessage(this Player sender, string str)
        {
            foreach (var player in sender.svPlayer.svManager.players.Where(x => x.Value.admin))
                Players.GetPlayerFromInternalList(player.Value).SendChatMessage(SvSendType.Self, $"<color=purple>[STAFF CHAT]</color><color=#74B999> {str}</color>");
        }
        /// <summary>
        /// Sends a message to local players.
        /// </summary>
        /// <param name="player">The user to send the message to</param>
        /// <param name="str">The message that you want to send.</param>
        public static void SendLocalMessage(this Player sender, string str)
        {
            sender.svPlayer.Send(SvSendType.Local, Channel.Unsequenced, SvPacket.LocalMessage, $"<color=green>[LOCAL]</color> {sender.Username}: {str}");
        }
    }
}
