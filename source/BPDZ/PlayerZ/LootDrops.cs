﻿/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using BP_API;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BPDZ
{
    class LootDrops
    {
        public static float LootDropDespawnTime = 45f;

        public static int[] Armour = new int[]
        {
            -1627168389, //T1 start
            -633509750,
            880705339,
            -766353867,
            -552154137,
            226920179,
            -1310874884,
            -1888685880, //T1 end
            134344360, //T2 start
            1561055750,
            1998080865,
            -1527091616,
            -295837114,
            2065462452, //T2 end
            1258964252, //T3 start
            -849689176,
            602133489, //T3 end
            388702693, //T4 start
            -645285295, //T4 end
            112674745 //T5
        };

        public static int[] Food = new int[]
        {
            123684252,
            1702175260,
            1205979388, //T1 end 
            1877954167,
            1201374976,
            -1220321196, //T2 end
            1734556914,
            811305019,
            1897785191, //T3 end
            467765943,
            -2098668787 //T4 end
        };

        public static int[] Ammo = new int[]
        {
            493970259,
            -906852676, //T1 end
            168336396,
            869043502, //T2 end
            1086364132,
            -872807194, //T3 end
            -921321944, //T4 end
            1699387113
        };

        public static int[] Gun = new int[]
        {
            -1975896234,
            -162370066,
            1968059740,
            2015188875, //T1 end
            1053582733,
            794733957,
            1075047858,
            1875626018,
            554920573, //T2 end
            238752591,
            2109002909,
            633001730,
            -1384309123,
            -2011341759, //T3 end
            -1572858027 //T4 end
        };

        public static void Initialize(ShPlayer player, int tier)
        {
            int foodItemID;
            int ammoItemID;
            int armourItemID;
            int gunItemID;
            Vector3 spawnerPos = player.GetPosition();
            Vector3 armourPos = new Vector3(spawnerPos.x, spawnerPos.y, spawnerPos.z);
            Vector3 ammoPos = new Vector3(spawnerPos.x + spawnerPos.y, 0f, spawnerPos.z - 2f);
            Vector3 gunPos = new Vector3(spawnerPos.x - 2, spawnerPos.y, spawnerPos.z);

            if (tier == 1)
            {
                InventoryItem foodItem;
                foodItemID = Food[Core.GenerateRandom(0, 10)];
                player.AddToMyItems(foodItemID, 1);
                player.myItems.TryGetValue(foodItemID, out foodItem);
                ShEntity FoodEntity = player.svEntity.svManager.AddNewEntity(foodItem.item, player.GetPlace(), spawnerPos, new Quaternion(0f, 0f, 0f, 0), false);
                FoodEntity.svEntity.svManager.StartCoroutine(Core.KillDelay(FoodEntity, LootDropDespawnTime));
            }

            if (tier == 2)
            {
                InventoryItem caseItem;
                ammoItemID = Ammo[Core.GenerateRandom(0, 6)];
                player.AddToMyItems(-667273670, 1);
                player.myItems.TryGetValue(-667273670, out caseItem);
                ShEntity ammoCase = player.svEntity.svManager.AddNewEntity(caseItem.item, player.GetPlace(), ammoPos, new Quaternion(0f, 0f, 0f, 0), false);
                ammoCase.AddToMyItems(ammoItemID, Core.GenerateRandom(20, 35));
                ammoCase.svEntity.destroyEmpty = true;
                ammoCase.svEntity.svManager.StartCoroutine(Core.KillDelay(ammoCase, LootDropDespawnTime));
            }

            if (tier == 3)
            {
                InventoryItem armourItem;
                armourItemID = Armour[Core.GenerateRandom(0, 19)];
                player.AddToMyItems(armourItemID, 1);
                player.myItems.TryGetValue(armourItemID, out armourItem);
                ShEntity armourEntity = player.svEntity.svManager.AddNewEntity(armourItem.item, player.GetPlace(), armourPos, new Quaternion(-0.7071068f, 0f, 0f, 0.7071068f), false);
                armourEntity.svEntity.svManager.StartCoroutine(Core.KillDelay(armourEntity, LootDropDespawnTime));
            }

            if (tier >= 4)
            {
                InventoryItem gunItem;
                gunItemID = Gun[Core.GenerateRandom(0, 14)];
                player.TransferItem(DeltaInv.AddToMe, gunItemID, 1, true);
                player.myItems.TryGetValue(gunItemID, out gunItem);
                ShEntity gunEntity = player.svEntity.svManager.AddNewEntity(gunItem.item, player.GetPlace(), gunPos, new Quaternion(0f, 0f, 0.7071068f, 0.7071068f), false);
                gunEntity.svEntity.svManager.StartCoroutine(Core.KillDelay(gunEntity, LootDropDespawnTime));
            }
            player.svPlayer.Despawn();
        }
    }
}
