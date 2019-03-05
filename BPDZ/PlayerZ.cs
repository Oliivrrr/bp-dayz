using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalUnityHooks;
using UnityEngine;
using Unity;
using ENet;

namespace BPDZ
{
    public class PlayerZ
    {
        [Hook("ShPlayer.RemoveItemsDeath")]
        public static bool RemoveItemsDeath(ShPlayer instance)
        {
            foreach (InventoryItem inventoryItem in instance.myItems.Values.ToList<InventoryItem>())
            {
                instance.TransferItem(2, inventoryItem.item.index, instance.MyItemCount(inventoryItem.item.index), true);
            }
            return false;
        }
    }
}
