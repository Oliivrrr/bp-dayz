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
            Wearables = new[]
            {
                -1962854266, //Head
                1089711634, //Face
                -299146069, //Body
                -1626497894, //Armour
                1174688158, //Hands
                1815065665, //Legs
                -1868436119, //Feet
                673780802 //Back
            };
        }
    }
}
