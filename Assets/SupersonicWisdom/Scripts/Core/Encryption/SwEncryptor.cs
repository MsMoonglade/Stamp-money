using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwEncryptor
    {
        #region --- Private Methods ---

        internal static string Encrypt(string textToEncrypt)
        {
            try
            {
                var key = "50p3r50n1cw15d0mORyDrAaMmY";
                var iv = "abc@98797hjkas$&asd(*$%";
                var encryptedText = "";
                var ivBytes = System.Text.Encoding.UTF8.GetBytes(iv.Substring(0, 8));
                var keyBytes = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                MemoryStream ms = null;
                CryptoStream cs = null;
                var inBytesArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);

                using (var des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(keyBytes, ivBytes), CryptoStreamMode.Write);
                    cs.Write(inBytesArray, 0, inBytesArray.Length);
                    cs.FlushFinalBlock();
                    encryptedText = Convert.ToBase64String(ms.ToArray());
                }

                return encryptedText;
            }
            catch (Exception ex)
            {
                //TODO add logs in future
                return "";
            }
        }

        #endregion
    }
}