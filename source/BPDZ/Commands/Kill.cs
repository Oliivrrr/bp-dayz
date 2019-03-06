using BP_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ.Commands
{
    class Kill
    {
        public static void KillPlayer(Player player, string victim)
        {
            Players.GetPlayerByUsername(victim).shPlayer.ShDie();
        }
    }
}
