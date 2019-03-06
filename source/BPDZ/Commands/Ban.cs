using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ.Commands
{
    class Ban
    {
        public static void BanPlayer(Player player, int targetID, string reason)
        {
            Player targetPlayer = Players.GetPlayerByID(targetID);
            targetPlayer.SendServerInfoMessage($"You were banned by {player.Username} for {reason}");
            targetPlayer.Ban(reason);
        }
    }
}
