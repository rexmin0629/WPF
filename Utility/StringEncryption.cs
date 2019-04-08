using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Utility
{
    public class StringEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Encryption(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Decryption(string msg)
        {
            string result = msg;
            try
            {
                var data = Convert.FromBase64String(msg);
                result = Encoding.UTF8.GetString(data);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[Exception] Decryption : " + e.Message);
            }
            return result;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static string byteAryToStr(byte[] ba){
            return BitConverter.ToString(ba);
        }
        public static byte[] strToByteAry(string s)
        {
            return s.Split('-').Select(str => byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }
        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

    }
}
