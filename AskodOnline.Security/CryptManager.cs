using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AskodOnline.Security
{
    public static class CryptManager
    {
        private const string Password = "17412025005304011223321124717915510818711705524602602104" +
                                        "62272071030041742021772501370100280212101791300870131192" +
                                        "01038051013254028105193043134160097013025037194020174018" +
                                        "14919508009219115014610300914114720218912100109308106505" +
                                        "10101980742161071881281742471451540300880361542410700640" +
                                        "87123167104217218144043233154176";

        public static List<string> Keys = new List<string>();

        /// <summary>
        /// Шифровать данные.
        /// </summary>
        /// <param name="data">Данные для шифровки.</param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data)));
        }

        /// <summary>
        /// Шифровать данные.
        /// </summary>
        /// <param name="data">Данные для шифровки.</param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] data)
        {
            using (var sa = Rijndael.Create())
            {
                var ct = sa.CreateEncryptor((new PasswordDeriveBytes(Password, null)).GetBytes(16), new byte[16]);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Расшифровать данные.
        /// </summary>
        /// <param name="data">Данные для расшифровки.</param>
        /// <returns></returns>
        public static string Decrypt(string data)
        {
            using (var sa = Rijndael.Create())
            {
                var ct = sa.CreateDecryptor((new PasswordDeriveBytes(Password, null)).GetBytes(16), new byte[16]);
                using (var ms = new MemoryStream(Convert.FromBase64String(data)))
                {
                    using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
