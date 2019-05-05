/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using BP_API;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UniversalUnityHooks;
using static BPDZ.Variables;

namespace BPDZ
{
    public static class PlayerZ
    {
        [Hook("ShPlayer.RemoveItemsDeath")]
        public static bool RemoveItemsDeath(ShPlayer player)
        {
            if (!player.svPlayer.serverside)
            {
                foreach (InventoryItem inventoryItem in player.myItems.Values.ToList())
                {
                    player.TransferItem(DeltaInv.RemoveFromMe, inventoryItem.item.index,
                        player.MyItemCount(inventoryItem.item.index), true);
                }
                return false;
            }
            return true;
        }

        static public IEnumerator ContaminationLoop(Player player)
        {
            if (player.shPlayer.GetPosition().x < -999999 && !player.IsServerSide())
            {
                player.SendSuccessMessage("You have entered a Contaminated Area! You will take damage if you are not wearing a gas mask!");
                while (player.shPlayer.GetPosition().x < -252 && !player.IsServerSide())
                {
                    yield return new WaitForSeconds(4f);
                    if (player.shPlayer.GetWearable(WearableType.Head) != null)
                    {
                        if (player.shPlayer.GetWearable(WearableType.Head).index != -1627168389)
                        {
                            player.svPlayer.Damage(DamageIndex.Null, 5f, player.shPlayer, player.shPlayer.headCollider);
                        }
                    }
                    else if (player.shPlayer.GetWearable(WearableType.Head) == null)
                    {
                        player.svPlayer.Damage(DamageIndex.Null, 5f, player.shPlayer, player.shPlayer.headCollider);
                    }
                }
                player.SendSuccessMessage("You left the Contaminated Area");
            }
        }

        static public IEnumerator LookForPlayers(SvPlayer player)
        {
            while (!player.player.IsDead())
            {
                yield return new WaitForSeconds(5f);
                foreach (Sector sector in player.localSectors)
                {
                    foreach (ShEntity shEntity in sector.centered)
                    {
                        if (!(shEntity == player.entity))
                        {
                            ShPlayer shPlayer = shEntity as ShPlayer;
                            if (shPlayer && player.player.CanSeeEntity(shPlayer) && shPlayer.job.jobIndex != player.player.job.jobIndex)
                            {
                                player.targetEntity = shPlayer;
                                player.SetState(10);
                            }
                        }
                    }
                }
            }
        }

        static public IEnumerator PlayerBleed(Player player, float time)
        {
            player.shPlayer.StartEffect(EffectIndex.Intoxicated);
            while (!player.shPlayer.IsDead() && time != 0)
            {
                yield return new WaitForSeconds(10f);
                time = time - 10f;
                player.svPlayer.Damage(DamageIndex.Null, Core.GenerateRandomF(7, 13), player.shPlayer, player.shPlayer.headCollider);
                player.SendSuccessMessage("You are bleeding. Heal yourself or get another player to heal you");
            }
        }

        static public IEnumerator PlayerInfected(Player player, float time)
        {
            player.shPlayer.StartEffect(EffectIndex.Intoxicated);
            while (!player.shPlayer.IsDead() && FileData.InfectedPlayers.Contains(player.Username))
            {
                yield return new WaitForSeconds(20f);
                player.svPlayer.Damage(DamageIndex.Null, Core.GenerateRandomF(15, 20), player.shPlayer, player.shPlayer.headCollider);
                player.SendSuccessMessage("You are infected. Consume pills to cure it");
            }
        }

        static public IEnumerator CarLoop(Player player)
        {
            yield return new WaitForSeconds(1f);
            while (player.shPlayer.curMount)
            {
                if (!player.Inventory.HasItem(1699387113))
                {
                    player.SendSuccessMessage("You ran out of fuel!");
                }
                yield return new WaitForSeconds(1);
                player.Inventory.RemoveItem(1699387113, 1);
            }
        }
    }
}
