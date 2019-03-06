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
        public static string GodListFile = @"BPDayZ/GodList.txt";
        public static int PermissionLevel = 10;

        public static void Run(string username)
        {
            if (File.ReadAllText(GodListFile).Contains(username))
            {
                List<string> allusers = File.ReadAllLines(GodListFile).ToList();
                allusers.Remove(username);
                File.WriteAllLines(GodListFile, allusers);
            }

            else
            {
                List<string> allusers = File.ReadAllText(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                allusers.Add(username);
                File.WriteAllText(GodListFile, allusers.ToString());
            }
        }

        public static bool HasGodmode(Player instance)
        {
            string playerlist = File.ReadAllText(GodListFile);
            if (playerlist.Contains(instance.Username))
            {
                return true;
            }
            return false;
        }
    }
}
