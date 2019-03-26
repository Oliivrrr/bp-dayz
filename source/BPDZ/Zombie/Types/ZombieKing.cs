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
            Rarity = 0.01f;
            Health = 500;
            DamageMultiplier = 5f;
            RunSpeed = 1.45f;
            WalkSpeed = 1.25f;
        }
    }
}
