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
            Rarity = 10;
            Health = 200;
            DamageMultiplier = 2.5f;
            RunSpeed = 6f;
            Wearables = new[]
            {
                880705339, //Head
                1089711634, //Face
                1258964252, //Body
                -1626497894, //Armour
                1174688158, //Hands
                134344360, //Legs
                -471928380, //Feet
                673780802 //Back
            };
        }
    }
}
