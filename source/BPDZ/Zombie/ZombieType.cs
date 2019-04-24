
/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


using System.CodeDom;

namespace BPDZ
{
    public abstract class ZombieType
    {
        SvPlayer Player { get; set; }
        protected ZombieType(SvPlayer player)
        {
            Player = player;
        }

        public string DisplayName
        {
            get
            {
                return Player.player.username;
            }
            set
            {
                Player.player.username = value;
            }
        }

        public int Rarity { get; set; }

        public int Health { get; set; }

        public float DamageMultiplier { get; set; }

        public float RunSpeed
        {
            get
            {
                return Player.player.maxSpeed;
            }
            set
            {
                Player.player.maxSpeed = value;
            }
        }

        public int[] Wearables { get; set; }
    }
}
