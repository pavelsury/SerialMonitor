using System;
using System.IO;
using System.Text;

namespace SerialMonitor.Business.Helpers
{
    public static class FileHelper
    {
        public static string ReadAllText(string filename)
        {
            try
            {
                return File.ReadAllText(filename);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void WriteAllTextNoShare(string filename, string text)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            try
            {
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var streamWriter = new StreamWriter(fileStream, Encoding.Default))
                {
                    streamWriter.Write(text);
                }
            }
            catch (Exception)
            { }
        }
    }
}