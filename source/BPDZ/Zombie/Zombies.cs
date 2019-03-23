/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace BPDZ
{
    public static class Zombies
    {
        public static List<Zombie> List { get; private set; } = new List<Zombie>();

        public static IEnumerable<Zombie> GetZombies() => List;
        public static IEnumerable<Zombie> GetAliveZombies() => List.Where(x=>x.Alive);
    }
}
