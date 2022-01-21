using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portalum.Zvt.Parsers
{
    public class LlvarParser
    {
        public byte[] GetLength(int length)
        {
            if (length < 0 || length > 99) throw new ArgumentOutOfRangeException(nameof(length));

            byte[] ll = new byte[2];

            if (length > 9)
            {
                ll[0] = Convert.ToByte(240 + GetNthDigit(length, 1));
                ll[1] = Convert.ToByte(240 + (length % 10));
            }
            else
            {
                ll[0] = 0xF0;
                ll[1] = Convert.ToByte(240 + length);
            }

            return ll;
        }

        private static int GetNthDigit(int value, int digits)
        {
            double mult = Math.Pow(10.0, digits);

            if (value >= mult)
            {
                while (value >= mult)
                    value /= 10;

                return value % 10;
            }

            else
            {
                throw new ArgumentOutOfRangeException("Digits greater than value");
            }
        }
    }
}
