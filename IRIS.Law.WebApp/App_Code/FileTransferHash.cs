using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace IRIS.Law.WebApp.App_Code
{
    public class FileTransferHash
    {
        /// <summary>
        /// Create a 16 byte MD5 hash of a file
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static byte[] CreateFileMD5Hash(string FileName)
        {
            using (FileStream Stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return CreateStreamMD5Hash(Stream);
            }
        }

        /// <summary>
        /// Create a 16 byte MD5 hash of a stream
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        public static byte[] CreateStreamMD5Hash(Stream Stream)
        {
            return new MD5CryptoServiceProvider().ComputeHash(Stream);
        }

        public static bool CheckHash(byte[] Hash1, byte[] Hash2)
        {
            if (Hash1.Length != Hash2.Length)
                return false;

            for (int i = 0; i < Hash1.Length; i++)
                if (Hash1[i] != Hash2[i])
                    return false;

            return true;
        }
    }
}
