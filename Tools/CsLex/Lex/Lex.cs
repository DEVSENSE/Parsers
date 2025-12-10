using System;
using System.IO;
namespace Lex
{
	public class Lex
	{
		public const int MAXBUF = 8192;
		public const int MAXSTR = 128;
		private static string inFile;
		private static string outFile;
		private static int version;
		public static bool ProcessArguments(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				if (arg[0] == '/')
				{
					int colon = arg.IndexOf(':');
					if (colon >= 0)
					{
						var currentOption = arg.Substring(1, colon - 1).Trim();
						var currentValue = arg.Substring(colon + 1).Trim();

						var optionNormalized = currentOption.ToLowerInvariant();
						if (optionNormalized == "v" || optionNormalized == "version")
						{
							if (int.TryParse(currentValue, out int version) && version >= 1 && version <= 2)
							{
								Lex.version = version;
                            }
							else
							{
								throw new ApplicationException("Invalid version number. Specify either '1' or '2'.");
							}
                        }
						else if (optionNormalized == "help")
						{
                            Lex.DisplayHelp();
                            return false;
                        }
						else
						{
                            throw new ApplicationException($"Unknown option '{arg}'.");
                        }
					}
					else
					{
                        throw new ApplicationException($"Option '{arg}' has no value.");
                    }
                }
				else
				{
					if (Lex.inFile == null)
					{
						Lex.inFile = arg;
					}
					else if (Lex.outFile != null)
					{
						throw new ApplicationException($"Invalid option '{arg}'.");
					}
					else
					{
						Lex.outFile = arg;
					}
				}
			}
			if (Lex.outFile == null)
			{
				Lex.outFile = Path.ChangeExtension(Lex.inFile, ".cs");
			}
			return true;
		}
		private static void DisplayHelp()
		{
			Console.WriteLine("lex <filename> [<outfile>] [<options>]");
			Console.WriteLine();
			Console.WriteLine("/help                   Displays this help.");
			Console.WriteLine("/v[ersion]:<version>    Version of C# to use for generated code.");
		}
		public static void Main(string[] args)
		{
			try
			{
				if (!Lex.ProcessArguments(args))
				{
					return;
				}
			}
			catch (ApplicationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine();
				Environment.ExitCode = 1;
			}
			try
			{
				Gen gen = new Gen(Lex.inFile, Lex.outFile, Lex.version);
				gen.Generate();
			}
			catch (ApplicationException ex2)
			{
				Console.WriteLine(ex2.Message);
				Console.WriteLine();
				Environment.ExitCode = 1;
			}
			catch (Exception ex3)
			{
				Console.WriteLine(ex3.Message);
				Console.WriteLine(ex3.StackTrace);
				Environment.ExitCode = 1;
			}
		}
	}
}
