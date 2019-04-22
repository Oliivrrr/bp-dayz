/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


namespace BPDZ
{
    public class ZombieKing : ZombieType
    {
        SvPlayer Player { get; set; }
        public ZombieKing(SvPlayer player) : base(player)
        {
            Player = player;
            DisplayName = "Zombie King";
            Rarity = 3; //1
            Health = 500;
            DamageMultiplier = 10f;
            RunSpeed = 4f;
        }
    }
}
