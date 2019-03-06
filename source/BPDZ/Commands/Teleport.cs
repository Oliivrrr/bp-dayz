using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPDZ.Commands
{
    class Teleport
    {
        public static void TeleportToTarget(Player player, string target)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            player.Location = targetPlayer.Location;
        }

        public static void TeleportToSender(Player player, string target)
        {
            Players.GetPlayerByUsername(target).Location = player.Location;
        }

        public static void TeleportToLocation(Player player, Player._Location location)
        {
            player.Location = location;
        }
    }
}
