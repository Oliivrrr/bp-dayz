/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Collections.Generic;

namespace BPDZ
{
    public class DayZGroups
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string PermissionLevel { get; set; }
        public string Color { get; set; }
		public List<string> Players { get; set; } = new List<string>();
    }
}
