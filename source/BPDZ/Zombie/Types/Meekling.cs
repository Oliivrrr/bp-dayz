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
            DisplayName = "Meekling";
            Rarity = 0; //65
            Health = 120;
            DamageMultiplier = 1.5f;
            RunSpeed = 8.5f;
        }
    }
}
