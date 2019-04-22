/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System.Collections.Generic;

namespace BPDZ
{
    public class Zombie
    {
        public SvPlayer Player { get; private set; }
        public Zombie(SvPlayer player)
        {
            Player = player;
        }
        public bool Alive => !Player.destroyable.IsDead();
        public ZombieType Type { get; set; }
    }
}
