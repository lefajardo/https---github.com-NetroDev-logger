using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace NetroLogger_WorkerRole
{
    public static class crypt
    {
        #region utilities
        public static string GetMD5HashFromFile(MemoryStream blobcontents)
        {

            //2012-10-28 changed to use sha512 hashing

            SHA512 sha512 = new SHA512CryptoServiceProvider();
            byte[] retVal = sha512.ComputeHash(blobcontents.ToArray());

            return System.BitConverter.ToString(retVal).Replace("-", "").Replace(" ", "");

        }

        #endregion utilities
    }
}
