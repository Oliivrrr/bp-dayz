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
            if (File.ReadAllText(GodListFile).Contains(username))
            {
                List<string> allusers = File.ReadAllText(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                allusers.Add(username);

                File.WriteAllText(GodListFile, allusers.ToString());
            }

            else
            {
                List<string> allusers = File.ReadAllText(GodListFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                allusers.Remove(username);
                foreach (var user in allusers)
                {
                    if (!user.Contains(" "))
                    {
                        File.WriteAllText(GodListFile, user);
                    }
                }
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
