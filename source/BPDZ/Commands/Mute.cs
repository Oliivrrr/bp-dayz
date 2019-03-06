using BP_API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ.Commands
{
    class Mute
    {
        public static string MuteFilePath = @"BPDayZ/MuteList.txt";

        public static void MutePlayer(Player sender, string victim)
        {
            List<string> data = File.ReadAllLines(MuteFilePath).ToList();
            if (data.Contains(victim))
            {
                data.Remove(victim);
                File.WriteAllLines(MuteFilePath, data.ToArray());
            }

            else
            {
                data.Add(victim);
                File.WriteAllLines(MuteFilePath, data.ToArray());
            }
        }
    }
}
