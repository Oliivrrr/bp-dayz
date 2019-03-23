/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


namespace BPDZ
{
    public abstract class ZombieType
    {
        SvPlayer Player { get; set; }
        protected ZombieType(SvPlayer player)
        {
            Player = player;
        }
        public float WalkSpeed { get; set; }
        public float RunSpeed { get; set; }
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
    }
}
