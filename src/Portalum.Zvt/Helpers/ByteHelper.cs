﻿using System;
using System.Linq;
using System.Text;

namespace Portalum.Zvt.Helpers
{
    /// <summary>
    /// ByteHelper
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Hex string to byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(string hex)
        {
            hex = hex.Replace("-", string.Empty);

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Byte array to hex string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHex(Span<byte> data)
        {
            var hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string text) => ByteHelper.StringToByteArray(text, Encoding.ASCII);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string text, Encoding encoding) => encoding.GetBytes(text);
    }
}
