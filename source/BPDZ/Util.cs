/*
 * BPDayZ
 *   A custom plugin for "Broke Protocol", with zombies.
 * (c) Unlucky 2019
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
			Lists.GoddedPlayers = File.ReadAllLines(GodListFile).ToList();
			Lists.MutedPlayers = File.ReadAllLines(MuteFilePath).ToList();
		}
	}
}
