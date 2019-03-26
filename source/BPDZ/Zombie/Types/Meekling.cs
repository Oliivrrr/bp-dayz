﻿/*
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
            Rarity = 0.65f;
            Health = 120;
            DamageMultiplier = 1f;
            RunSpeed = 1.35f;
            WalkSpeed = 1.25f;
        }
    }
}
