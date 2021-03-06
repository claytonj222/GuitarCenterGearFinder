using System.Diagnostics;

namespace GuitarCenterGearFinder.Classes
{
    public static class Tracer
    {
        public static bool DoLog { get; set; }
        public static string FilePath { get; set; }

        public static void TryEmptyTraceFile()
        {
            try
            {
                FileInfo f = new FileInfo(FilePath);
                if (f.Length > 2147483648)
                {
                    ClearTraceFile();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

        }

        private static void ClearTraceFile()
        {
            using (FileStream fs = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                lock (fs)
                {
                    fs.SetLength(0);
                }
            }
        }

        public static void PrintDetailedTrace(string callingMethod, string output)
        {
            if (DoLog)
            {
                Trace.WriteLine(string.Format("{0}: - {1}: {2}", DateTime.Now, callingMethod, output));
            }
        }

        public static void PrintDetailedException(Exception ex)
        {
            Trace.WriteLine(GetDetailedException(ex));
        }

        public static string GetDetailedException(Exception ex)
        {
            return string.Format("{0}: - {1}", DateTime.Now, ex.ToString());

        }
    }
}
