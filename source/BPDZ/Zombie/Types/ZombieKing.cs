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
            Rarity = 1;
            Health = 500;
            DamageMultiplier = 10f;
            RunSpeed = 4f;
            Wearables = new[]
            {
                880705339, //Head
                1089711634, //Face
                2065462452, //Body
                112674745, //Armour
                1174688158, //Hands
                -1527091616, //Legs
                837068077, //Feet
                673780802 //Back
            };
        }
    }
}
