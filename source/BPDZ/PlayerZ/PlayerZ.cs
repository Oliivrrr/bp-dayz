/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Linq;
using UniversalUnityHooks;

namespace BPDZ
{
    public class PlayerZ
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
    }
}
