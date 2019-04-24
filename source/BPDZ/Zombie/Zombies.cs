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

        public static bool SpawnZombie(Player player, ref Vector3 position, ref Quaternion rotation, ref Place place,
            ref Waypoint node, ref ShPlayer spawner, ref ShMountable mount, ref ShPlayer enemy)
        {
            var zombie = new Zombie(player.svPlayer);
            int TypeChance = Core.GenerateRandom(1, 100);
            if (TypeChance == new ZombieKing(player.svPlayer).Rarity)
            {
                zombie.Type = new ZombieKing(player.svPlayer);
            }
            else if (TypeChance <= new Runner(player.svPlayer).Rarity)
            {
                zombie.Type = new Runner(player.svPlayer);
            }
            else if (TypeChance <= new Slug(player.svPlayer).Rarity)
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
            player.svPlayer.SvTrySetJob(JobIndex.Criminal, false, false);
            player.svPlayer.player.Spawn(position, rotation, place.transform);
            player.Stats.SetHealth(zombie.Type.Health, true);
            List.Add(zombie);
            player.svPlayer.svManager.StartCoroutine(ZombieLoop(player, zombie.Type));
            player.svPlayer.svManager.StartCoroutine(Core.LookForPlayers(player.svPlayer));
            return true;
        }

        static IEnumerator ZombieLoop(Player player, ZombieType zombieType)
        {
            while (!player.shPlayer.IsDead())
            {
                foreach (InventoryItem inventoryItem in player.shPlayer.myItems.Values.ToArray())
                {
                    if (!zombieType.Wearables.Contains(inventoryItem.item.index))
                    {
                        player.shPlayer.TransferItem(DeltaInv.RemoveFromMe, inventoryItem.item.index, inventoryItem.count, true);
                    }
                }

                foreach (var WearbleID in zombieType.Wearables)
                {
                    player.shPlayer.SetWearable(WearbleID);
                }
                
                player.svPlayer.SvTrySetJob(JobIndex.Criminal, false, false);
                player.svPlayer.SvAddCrime(CrimeIndex.Trespassing, player.shPlayer);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
