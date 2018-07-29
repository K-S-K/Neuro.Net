using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace UnitTests.Common
{
    public static class FileComparator
    {
        private static string GetFileHash(string filename)
        {
            var hash = new SHA1Managed();
            var clearBytes = File.ReadAllBytes(filename);
            var hashedBytes = hash.ComputeHash(clearBytes);
            return ConvertBytesToHex(hashedBytes);
        }

        private static string ConvertBytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x"));
            }
            return sb.ToString();
        }

        public static void CompareFiles(string fnActual, string fnExpctd, string errMsg)
        {
            string HashStockActual = GetFileHash(fnActual);
            string HashStockExpctd = GetFileHash(fnExpctd);
            if (HashStockActual != HashStockExpctd)
                throw new Exception(Environment.NewLine + errMsg);
        }
    }
}
