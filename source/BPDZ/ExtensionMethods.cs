﻿/*
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
    }
}