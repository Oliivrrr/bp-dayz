/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using BP_API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace BPDZ
{
    public static class Zombies
    {
        public static List<Zombie> List { get; private set; } = new List<Zombie>();

        public static IEnumerable<Zombie> GetZombies() => List;
        public static IEnumerable<Zombie> GetAliveZombies() => List.Where(x=>x.Alive);

        public static bool SpawnZombie(Player player, ref Vector3 position, ref Quaternion rotation, ref Place place, ref Waypoint node, ref ShPlayer spawner, ref ShMountable mount, ref ShPlayer enemy)
        {
            var zombie = new Zombie(player.svPlayer);
            int f = Core.GenerateRandom(1, 100);
            if (f == 100)
                zombie.Type = new ZombieKing(player.svPlayer);
            else if (f >= 75)
            {
                zombie.Type = new Runner(player.svPlayer);
            }
            else if (f > 81)
            {
                zombie.Type = new Slug(player.svPlayer);
            }
            else
            {
                zombie.Type = new Meekling(player.svPlayer);
            }
            player.svPlayer.preFrame = true;
            if (node)
            {
                player.svPlayer.SetNextWaypoint(node);
                player.svPlayer.onWaypoints = true;
            }
            else
            {
                player.svPlayer.onWaypoints = false;
            }
            player.svPlayer.player.Spawn(position, rotation, place.transform);
            List.Add(zombie);
            player.shPlayer.job.jobIndex = JobIndex.Criminal;
            player.svPlayer.svManager.StartCoroutine(ZombieLoop(player));
            player.svPlayer.svManager.StartCoroutine(Core.LookForPlayers(player.svPlayer));
            player.svPlayer.SvAddCrime(CrimeIndex.Trespassing, player.shPlayer);
            player.svPlayer.SvAddCrime(CrimeIndex.Trespassing, player.shPlayer);
            return true;
        }

        static IEnumerator ZombieLoop(Player player)
        {
            while (!player.shPlayer.IsDead())
            {
                player.svPlayer.SvSetWearable(673780802);
                player.svPlayer.SvSetWearable(-1638932793);
                player.svPlayer.SvSetWearable(1089711634);
                player.svPlayer.SvSetWearable(2064679354);
                player.svPlayer.SvSetWearable(-501996567);
                player.svPlayer.SvSetWearable(-1191209217);
                foreach (var item in player.shPlayer.myItems)
                {
                    player.Inventory.RemoveItem(item.Value.item.ID, item.Value.count);
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
