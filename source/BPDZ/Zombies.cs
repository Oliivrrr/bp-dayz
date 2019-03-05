using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniversalUnityHooks;
using Newtonsoft.Json;
using System.IO;
using BPDZ;
using BP_API;
using Newtonsoft.Json.Linq;

namespace BPDayZ
{
    static class Zombies
    {
        public static void GiveItems(SvPlayer instance, int DropType)
        {
            foreach (InventoryItem inventoryItem in instance.player.myItems.Values.ToList<InventoryItem>())
            {
                instance.player.TransferItem(2, inventoryItem.item.index, instance.player.MyItemCount(inventoryItem.item.index), true);
            }
            if (DropType < 80)
            {
                instance.player.AddToMyItems(Food[DayZCore.RNG(6, Food.Length - 1)], DayZCore.RNG(10, 35));
                instance.player.AddToMyItems(Ammo[DayZCore.RNG(0, 3)], DayZCore.RNG(35, 60));
                instance.player.AddToMyItems(Gun[DayZCore.RNG(0, 3)], 6);
            }
            if (DropType < 85)
            {
                instance.player.AddToMyItems(Food[DayZCore.RNG(8, Food.Length - 1)], DayZCore.RNG(10, 35));
                instance.player.AddToMyItems(Ammo[DayZCore.RNG(2, 5)], DayZCore.RNG(35, 60));
                instance.player.AddToMyItems(Gun[DayZCore.RNG(0, 8)], 6);
                instance.player.AddToMyItems(Armour[DayZCore.RNG(0, 6)], 1);
            }
            if (DropType < 92)
            {
                instance.player.AddToMyItems(Food[Food.Length - 2], DayZCore.RNG(10, 35));
                instance.player.AddToMyItems(Ammo[DayZCore.RNG(4, 5)], DayZCore.RNG(35, 60));
                instance.player.AddToMyItems(Gun[DayZCore.RNG(9, 13)], 6);
                instance.player.AddToMyItems(Armour[DayZCore.RNG(7, 15)], 1);
            }
            if (DropType < 98)
            {
                instance.player.AddToMyItems(Food[Food.Length - 1], DayZCore.RNG(10, 35));
                instance.player.AddToMyItems(Ammo[Ammo.Length - 1], DayZCore.RNG(6, 10));
                instance.player.AddToMyItems(Gun[Gun.Length - 1], 6);
                instance.player.AddToMyItems(Armour[DayZCore.RNG(16, Armour.Length - 1)], 1);
            }
            else
            {
                instance.player.AddToMyItems(Food[DayZCore.RNG(0, Food.Length - 1)], DayZCore.RNG(10, 35));
                instance.player.AddToMyItems(Ammo[DayZCore.RNG(0, 1)], DayZCore.RNG(35, 60));
            }
        }

        public static bool SetZombie(Player instance, ref Vector3 position, ref Quaternion rotation, ref Place place, ref Waypoint node, ref ShPlayer spawner, ref ShEntity mount, ref ShPlayer enemy)
        {
            if (DayZCore.RNG(1, 150) == 1)
            {
                
                GiveItems(instance.svPlayer, DayZCore.RNG(1, 100));
                instance.svPlayer.player.ShDie();
                instance.svPlayer.player.gameObject.SetActive(false);
            }

            if (DayZCore.RNG(1, 100) == 1)
            {
                
                instance.svPlayer.SvSetJob(instance.svPlayer.player.jobs[JobIndex.Paramedic], true, false);
                foreach (InventoryItem inventoryItem in instance.svPlayer.player.myItems.Values.ToList<InventoryItem>())
                {
                    instance.svPlayer.player.TransferItem(2, inventoryItem.item.index, instance.svPlayer.player.MyItemCount(inventoryItem.item.index), true);
                }
                int SpawnType = DayZCore.RNG(1, 100);
                if (SpawnType < 30)
                {
                    instance.svPlayer.player.AddToMyItems(-828647977, DayZCore.RNG(20, 30));
                    instance.svPlayer.player.AddToMyItems(-1477501700, 1);
                    instance.svPlayer.player.curEquipable.moveSpeed = 1.25f;
                    instance.svPlayer.player.health = 80;
                }

                else if (SpawnType < 50)
                {
                    instance.svPlayer.player.AddToMyItems(-828647977, DayZCore.RNG(30, 40));
                    instance.svPlayer.player.AddToMyItems(-484090981, 1);
                    instance.svPlayer.player.curEquipable.moveSpeed = 3;
                    instance.svPlayer.player.health = 150;
                }

                else if (SpawnType < 70)
                {
                    instance.svPlayer.player.AddToMyItems(-828647977, DayZCore.RNG(40, 50));
                    instance.svPlayer.player.AddToMyItems(-406179965, 1);
                    instance.svPlayer.player.curEquipable.moveSpeed = 2;
                    instance.svPlayer.player.health = 200;
                }

                else if (SpawnType < 80)
                {
                    instance.svPlayer.player.AddToMyItems(-828647977, DayZCore.RNG(60, 70));
                    instance.svPlayer.player.AddToMyItems(1670374823, 1);
                    instance.svPlayer.player.curEquipable.moveSpeed = 1;
                    instance.svPlayer.player.health = 275;
                }

                else if (SpawnType < 95)
                {
                    instance.svPlayer.player.AddToMyItems(-828647977, DayZCore.RNG(100, 175));
                    instance.svPlayer.player.AddToMyItems(-648442112, 1);
                    instance.svPlayer.player.curEquipable.moveSpeed = 2;
                    instance.svPlayer.player.health = 500;
                }
            }
            return true;
        }

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
            -872807194 //T3 end
            -921321944 //T4 end
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
            -2011341759,//T3 end
            -1572858027 //T4 end
        };

        public static int[] Armour = new int[]
        {
            -1627168389, //T1 start
            -633509750,
            880705339,
            -766353867,
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
    }
}
