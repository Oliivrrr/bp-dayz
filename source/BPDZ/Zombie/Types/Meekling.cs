/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


namespace BPDZ
{
    public class Meekling : ZombieType
    {
        SvPlayer Player { get; set; }
        public Meekling(SvPlayer player) : base(player)
        {
            Player = player;
            Health = 120;
        }
    }
}
