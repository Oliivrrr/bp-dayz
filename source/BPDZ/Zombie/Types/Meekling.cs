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
            Rarity = 65;
            Health = 120;
            DamageMultiplier = 1.5f;
            RunSpeed = 8.5f;
            Wearables = new[]
            {
                -1779484338,
                1089711634,
                -2145175385,
                -1626497894,
                1174688158,
                -1954781234,
                1629197558,
                673780802
            };
        }
    }
}
