﻿using System;
using NDesk.Options;
using System.IO;

namespace G00D1DEA.Piker
{
	class Program
	{
		static void Main(string[] args)
		{
			string inputFilename = null;
			string outputFilename = null; 
			string key = null;
			uint numKey = 0;
			bool showHelp = false;
			var p = new OptionSet() {
				{ "i|input=", "input file", v => inputFilename = v},
				{ "o|output=", "output file", v => outputFilename = v },
				{ "k|key=", "crypto key", v => key = v},
				{ "h|help", "show help", v => showHelp = v != null },
			};
			try
			{
				p.Parse(args);
			}
			catch (Exception e)
			{
				Console.Error.Write("Piker:");
				Console.Error.Write(e.Message);
				Console.Error.WriteLine();
				p.WriteOptionDescriptions(Console.Error);
				Environment.Exit(1);
			}

			if (string.IsNullOrEmpty(key))
			{
				Console.Error.WriteLine("crypto key not found.");
				p.WriteOptionDescriptions(Console.Error);
				Environment.Exit(1);
			}

			try
			{
				numKey = uint.Parse(key);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("key should be integer");
				Console.Error.WriteLine(e.Message);
				p.WriteOptionDescriptions(Console.Error);
				Environment.Exit(1);
			}

			if (showHelp)
			{
				p.WriteOptionDescriptions(Console.Out);
				return;
			}

		
			byte[] inputBytes = null;
			if (Console.IsInputRedirected)
			{
				var m = new MemoryStream();
				byte[] buf = new byte[4096];
				var s = Console.OpenStandardInput();
				while (true)
				{
					var n = s.Read(buf, 0, buf.Length);
					if (n > 0)
					{
						m.Write(buf, 0, n);
						continue;
					}
					break;
				}

				inputBytes = new byte[m.Length];
				Array.Copy(m.GetBuffer(), inputBytes, m.Length);
			}
			else
			{
				if (string.IsNullOrEmpty(inputFilename))
				{
					Console.Error.WriteLine("missing input filename.");
					p.WriteOptionDescriptions(Console.Error);
					Environment.Exit(1);
				}
				try
				{
					inputBytes = File.ReadAllBytes(inputFilename);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e.Message);
					p.WriteOptionDescriptions(Console.Error);
					Environment.Exit(1);
				}
			}


			var pike = new Pike.Pike(numKey);
			pike.Codec(ref inputBytes);


			if (Console.IsOutputRedirected)
			{
				var s = Console.OpenStandardOutput();
				s.Write(inputBytes, 0, inputBytes.Length);
			}
			else
			{
				if (string.IsNullOrEmpty(outputFilename))
				{
					Console.Error.WriteLine("missing output filename.");
					p.WriteOptionDescriptions(Console.Error);
					Environment.Exit(1);
				}
				File.WriteAllBytes(outputFilename, inputBytes);
			}
		}
	}
}
