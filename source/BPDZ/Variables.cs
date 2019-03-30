/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BPDZ
{
	public static class Variables
	{
		public const string resourceName = "BPDayZ";
		public const string prefix = "BPDayZ";

		public static string RootFolder { get; } = "BPDayZ";
		public static string GroupsFolder { get; } = Path.Combine(RootFolder, "groups/");
		public static string GodListFile { get; } = Path.Combine(RootFolder, "GodList.txt");
		public static string MuteFilePath { get; } = Path.Combine(RootFolder, "MuteList.txt");
		public static Random Random { get; } = new Random();

        public static string Version { get; private set; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
		public static bool IsPreRelease => Version.Contains("pre");
		public static bool IsDevelopmentBuild => Version.Contains("dev");



		public static class FileData
		{
			public static List<string> GoddedPlayers = new List<string>();
			public static List<string> MutedPlayers = new List<string>();
		}
	}
}
