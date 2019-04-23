/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


namespace BPDZ
{
    public class Runner : ZombieType
    {
        SvPlayer Player { get; set; }
        public Runner(SvPlayer player) : base(player)
        {
            Player = player;
            DisplayName = "Runner";
            Rarity = 24;
            Health = 135;
            DamageMultiplier = 1f;
            RunSpeed = 9.2f;
            Dif = -471928380;
        }
    }
}
