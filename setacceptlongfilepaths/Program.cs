using System;
using Microsoft.Win32;
namespace setacceptlongfilepaths
{
    class Program
    {
        static void Main()
        {
            if (!ThrowsException(() => Ignore(Environment.GetCommandLineArgs()[2])))
            {
                ShowHelp();
                return;
            }
            var lm = RegistryHive.LocalMachine;
            var lmk = RegistryKey.OpenBaseKey(lm, RegistryView.Default);
            if (ThrowsException(() => Ignore(Environment.GetCommandLineArgs()[1])))
            {
                var fsk = lmk.OpenSubKey("SYSTEM").OpenSubKey("CurrentControlSet").OpenSubKey("Control").OpenSubKey("FileSystem");
                var v = fsk.GetValue("LongPathsEnabled");
                Console.WriteLine((((int)v) == 1) ? "Long paths allowed." : "Long paths blocked.");
            }
            else
            {
                var fsk = lmk.CreateSubKey("SYSTEM").CreateSubKey("CurrentControlSet").CreateSubKey("Control").CreateSubKey("FileSystem");
                if (bool.TryParse(Environment.GetCommandLineArgs()[1], out bool x))
                {
                    fsk.SetValue("LongPathsEnabled", x ? 1 : 0);
                    return;
                }
                ShowHelp();
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
        private static void ShowHelp()
        {
            Console.WriteLine("setacceptlongfilepaths [true | false]");
            Console.WriteLine("┌──────────────────────────────────────────────────────────────────────┐");
            Console.WriteLine("│ true       Allows long file paths on the machine.                    │");
            Console.WriteLine("│ false      Blocks long file paths on the machine.                    │");
            Console.WriteLine("│ No args    Shows whether long file paths are allowed on the machine. │");
            Console.WriteLine("└──────────────────────────────────────────────────────────────────────┘");
        }
#pragma warning disable IDE0060
        static void Ignore(object v)
#pragma warning restore IDE0060
        {
        }
        static bool ThrowsException(Action p)
        {
            var te = false;
            try
            {
                p();
            }
            catch
            {
                te = true;
            }
            return te;
        }
    }
}