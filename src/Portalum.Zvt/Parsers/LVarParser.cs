using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portalum.Zvt.Parsers
{
    /// <summary>
    /// LVarParser
    /// </summary>
    public static class LVarParser
    {
        /// <summary>
        /// Extracts the length out of bytes with the following format
        /// <br/>
        /// [Fx1] [Fx2] [Fx3]...[FxlengthBytes]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="lengthBytes"></param>
        /// <returns></returns>
        /// <remarks>
        /// ATTENTION: LengthBytes is not added by this generic function
        /// </remarks>
        public static int ExtractLVarLength(byte[] buffer, int offset, int lengthBytes)
        {
            int length = 0;

            for (int i = 0; i < lengthBytes; i++)
            {
                byte currentByte = buffer[offset + lengthBytes - i - 1];

                //Consistency chek
                if ((currentByte & 0xF0) != 0xF0)
                    throw new ArgumentException("Could not extract length out of variable-length-field");

                length += (int)((currentByte & 0x0F) * Math.Pow(10, i));

            }

            return length;
        }

        /// <summary>
        /// Composes the length of the <paramref name="rawData"/> in a <paramref name="lengthBytes"/>-long array
        /// </summary>
        /// <param name="rawData">The data of which the length is to be composed</param>
        /// <param name="lengthBytes">In how many bytes should the length be composed</param>
        /// <returns>Returns an array with the length of <paramref name="lengthBytes"/> containing the length of <paramref name="rawData"/></returns>
        public static byte[] ComposeLVarLength(byte[] rawData, int lengthBytes)
        {
            byte[] data = new byte[lengthBytes];

            int myLength = rawData.Length;

            for (int i = 0; i < lengthBytes; i++)
            {
                //                          Modulo      bitwise OR
                byte currentByte = (byte)((myLength % 10) | 0xF0);
                myLength /= 10;

                data[lengthBytes - i - 1] = currentByte;
            }

            if (myLength > 0)
            {
                throw new ArgumentException(string.Format("#{0}-Bytes cannot be compressed in L-{1}-Var value", rawData.Length, lengthBytes));
            }

            return data;
        }

        /// <summary>
        /// Composes a LVarDatat byte[]
        /// </summary>
        /// <param name="rawData">The var-data</param>
        /// <param name="lengthBytes">In how many bytes should the length of the <paramref name="rawData"/> be composed</param>
        /// <returns>A byte[] containing the length and <paramref name="rawData"/></returns>
        public static byte[] ComposeLVarData(byte[] rawData, int lengthBytes)
        {
            byte[] data = new byte[lengthBytes + rawData.Length];
            Array.Copy(ComposeLVarLength(rawData, lengthBytes), data, lengthBytes);
            Array.Copy(rawData, 0, data, lengthBytes, rawData.Length);
            return data;
        }

        public static int ExtractLLVarLength(byte[] buffer, int offset)
        {
            if (buffer.Length - offset < 2)
                throw new ArgumentException("For LL-Var at least 2 bytes are required");

            return ExtractLVarLength(buffer, offset, 2) + 2;
        }

        public static int ExtractLLLVarLength(byte[] buffer, int offset)
        {
            if (buffer.Length - offset < 3)
                throw new ArgumentException("For LLL-Var at least 3 bytes are required");

            return ExtractLVarLength(buffer, offset, 3) + 3;
        }


        public static byte[] ReadLVarData(byte[] buffer, int offset, int lengthBytes)
        {
            int dataLength = ExtractLVarLength(buffer, offset, lengthBytes);

            byte[] data = new byte[dataLength];
            Array.Copy(buffer, offset + lengthBytes, data, 0, dataLength);

            return data;
        }


        public static byte[] ReadLLVarData(byte[] buffer, int offset)
        {
            return ReadLVarData(buffer, offset, 2);
        }

        public static byte[] ReadLLLVarData(byte[] buffer, int offset)
        {
            return ReadLVarData(buffer, offset, 3);
        }

        /// <summary>
        /// Composes a LVarDatat byte[] with two length bytes
        /// </summary>
        /// <param name="rawData">The var-data</param>
        /// <returns>A byte[] containing the length and <paramref name="rawData"/></returns>
        public static byte[] ComposeLLVarData(byte[] rawData)
        {
            return ComposeLVarData(rawData, 2);
        }

        /// <summary>
        /// Composes a LVarDatat byte[] with three length bytes
        /// </summary>
        /// <param name="rawData">The var-data</param>
        /// <returns>A byte[] containing the length and <paramref name="rawData"/></returns>
        public static byte[] ComposeLLLVarData(byte[] rawData)
        {
            return ComposeLVarData(rawData, 3);
        }

        public static byte[] EncodeString(string s)
        {
            if (s.Length % 2 != 0)
            {
                s += "F";
            }

            byte[] data = new byte[s.Length / 2];
            
            for (int i = 0; i < s.Length; i += 2)
            {
                byte num = Convert.ToByte(s.Substring(i, 2), 16);
                data[i / 2] = num;
            }

            return data;
        }

        public static string DecodeString(byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.AppendFormat("{0:X2}", data[i]);
            }

            return stringBuilder.ToString();
        }
    }
}
