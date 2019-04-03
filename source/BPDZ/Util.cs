/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BPDZ.Variables;

namespace BPDZ
{
	public static class Util
	{
		/// <summary>
		/// Logs a specific message with prefix.
		/// </summary>
		/// <param name="msg">The message to log.</param>
		public static void Log(string msg)
		{
			Debug.Log($"[{prefix}] {msg}");
		}
		/// <summary>
		/// Logs a specific message with prefix, but only when in development mode.
		/// </summary>
		/// <param name="msg">The message to log.</param>
		public static void LogDebug(string msg)
		{
			if (!Variables.IsDevelopmentBuild)
				return;
			Log(msg);
		}

		/// <summary>
		/// Checks if all files and directories exist.
		/// </summary>
		public static void ValidateFiles()
		{
			if (!Directory.Exists(@"BPDayZ"))
			{
				Log("Creating directory BPDayZ...");
				Directory.CreateDirectory("BPDayZ/Groups");
				Log("Successfully created directory");
				Log("Creating files...");
				File.Create("BPDayZ/GodList.txt");
				File.Create("BPDayZ/DiscordLink.txt");
				File.Create("BPDayZ/HelpMessage.txt");
				File.Create("BPDayZ/MuteList.txt");
                Log("Successfully created files");
			}
			Log("All resources exist and validated.");
		}
		/// <summary>
		/// Reads all required files into memory.
		/// </summary>
		public static void ReadFiles()
		{
			FileData.GoddedPlayers = File.ReadAllLines(GodListFile).ToList();
			FileData.MutedPlayers = File.ReadAllLines(MuteFilePath).ToList();
        }

        /// <summary>
        /// Writes an list to a file, every entry on a new line.
        /// </summary>
        /// <param name="list">The list that will be used.</param>
        /// <param name="fileName">The file that the list will be written to. If it does already exist, it'll be overwritten.</param>
        public static void ListToFile(IEnumerable<string> list, string fileName)
        {
            File.WriteAllLines(fileName, list);
        }
	}
}
