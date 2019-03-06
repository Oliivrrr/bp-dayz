using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ.Commands
{
    class Kick
    {
        public static void KickPlayer(Player player, string target, string reason)
        {
            Player targetPlayer = Players.GetPlayerByUsername(target);
            targetPlayer.SendServerInfoMessage($"You were kicked by {player.Username} for {reason}");
            targetPlayer.Kick(reason);
        }
    }
}
