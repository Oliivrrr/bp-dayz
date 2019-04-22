/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */


namespace BPDZ
{
    public class Slug : ZombieType
    {
        SvPlayer Player { get; set; }
        public Slug(SvPlayer player) : base(player)
        {
            Player = player;
            DisplayName = "Slug";
            Rarity = 2; //9
            Health = 200;
            DamageMultiplier = 2.5f;
            RunSpeed = 6f;
        }
    }
}
