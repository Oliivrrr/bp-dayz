
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

        public byte Rarity
        {
            get
            {
                return Player.player.job.jobIndex;
            }
            set
            {
                Player.player.job.jobIndex = value;
            }
        }

        public float Health
        {
            get
            {
                return Player.player.health;
            }
            set
            {
                Player.player.health = value;
            }
        }
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
    }
}
