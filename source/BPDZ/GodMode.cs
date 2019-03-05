using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using BP_API;

namespace BPDZ
{
    class GodMode
    {
        public static string GodListFile = @"BPDayZ/Godlist.txt";
        public static int PermissionLevel = 1;

        public static void Run(string username)
        {
            if (DayZManager.ReadData(GodListFile).Contains(username))
            {
                Remove(username);
            }

            else
            {
                Add(username);
            }
        }

        public static void Add(string username)
        { 

            List<string> allusers = DayZManager.ReadData(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            foreach (var user in allusers)
            {
                DayZManager.WriteData(GodListFile, user);
            }
            DayZManager.WriteData(GodListFile, username);
        }

        public static void Remove(string username)
        {
            List<string> allusers = DayZManager.ReadData(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            allusers.Remove(username);
            foreach (var user in allusers)
            {
                if (!user.Contains(" "))
                {
                    DayZManager.WriteData(GodListFile, user);
                }
            }
        }

        public static bool HasGodmode(Player instance)
        {
            string playerlist = DayZManager.ReadData(GodListFile);
            if (playerlist.Contains(instance.Username))
            {
                return true;
            }
            return false;
        }
    }
}
