using Portalum.Zvt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Portalum.Zvt.Models
{
    /// <summary>
    /// The TLV-Container containig all the TLV-Tags
    /// </summary>
    public class TlvContainerParameter
    {
        /// <summary>
        /// Pseudo-Tag declaring the start of the TLV-Container
        /// </summary>
        public const byte BMP = 0x06;

        /// <summary>
        /// All data-objects
        /// </summary>
        public List<TlvItem> TlvItems = new List<TlvItem>();

        /// <summary>
        /// The length of the <see cref="TlvContainerParameter"/> and all <see cref="TlvItems"/> combined
        /// </summary>
        public int Length => GetBytes().Length;

        /// <summary>
        /// TLV-ContainerParameter
        /// </summary>
        public TlvContainerParameter() : this(null)
        {

        }

        /// <summary>
        /// TLV-ContainerParameter
        /// </summary>
        /// <param name="tlvItems"></param>
        public TlvContainerParameter(List<TlvItem> tlvItems)
        {
            TlvItems = new List<TlvItem>();

            if (tlvItems != null)
            {
                TlvItems = tlvItems;
            }
        }

        /// <summary>
        /// Gets all the bytes of the <see cref="TlvContainerParameter"/> and all <see cref="TlvItems"/>
        /// </summary>
        /// <returns>A byte-array with all the data of the <see cref="TlvContainerParameter"/></returns>
        public byte[] GetBytes()
        {
            var subItemData = new List<byte>();
            foreach(var item in TlvItems)
            {
                subItemData.AddRange(item.GetBytes());
            }

            var data = new List<byte>();
            data.Add(BMP);
            data.AddRange(TlvItem.GetLengthData(subItemData.Count));
            data.AddRange(subItemData);

            return data.ToArray();
        }
    }

    /// <summary>
    /// A TLV data-object
    /// </summary>
    public class TlvItem
    {
        /// <summary>
        /// The tag of the data-object
        /// </summary>
        public TlvTag Tag { get; }

        /// <summary>
        /// The data-element of the data-object
        /// </summary>
        public List<byte> Data = new List<byte>();

        /// <summary>
        /// If the data-object is constructed it can have sub-items
        /// </summary>
        public List<TlvItem> SubItems { get; set; }

        /// <summary>
        /// TLV-Item
        /// </summary>
        /// <param name="data">data-element</param>
        public TlvItem(byte[] data)
        {
            Data.AddRange(data);
        }

        /// <summary>
        /// TLV-Item
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="data">data-element</param>
        public TlvItem(TlvTag tag, List<byte> data) : this(tag, data, null)
        {

        }

        /// <summary>
        /// TLV-Item
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="subItems">sub-items</param>
        public TlvItem(TlvTag tag, List<TlvItem> subItems = null) : this(tag, null, subItems)
        {

        }

        private TlvItem(TlvTag tag, List<byte> data = null, List<TlvItem> subItems = null)
        {
            Tag = tag;

            Data = new List<byte>();
            if (data != null)
            {
                Data.AddRange(data);
            }

            SubItems = new List<TlvItem>();
            if (subItems != null)
            {
                SubItems.AddRange(subItems);
            }
        }

        /// <summary>
        /// Gets the bytes of this data-object and if constructed also all the <see cref="SubItems"/>
        /// </summary>
        /// <returns>A byte-array containing all the data</returns>
        public byte[] GetBytes()
        {
            var data = new List<byte>();

            data.AddRange(Tag.Data);

            // Primitive data objects only contain custom Data
            if (Tag.IsPrimitive && Data.Any())
            {
                data.AddRange(GetLengthData(Data.Count));
                data.AddRange(Data);
            }

            // Constructed data objects only contain sub-data-objects
            if (!Tag.IsPrimitive && SubItems.Any())
            {
                var subItemData = new List<byte>();

                foreach (var subItem in SubItems)
                {
                    subItemData.AddRange(subItem.GetBytes());
                }

                data.AddRange(GetLengthData(subItemData.Count));
                data.AddRange(subItemData);
            }

            return data.ToArray();
        }

        /// <summary>
        /// Gets the length part of the data-object
        /// </summary>
        /// <param name="length">The length of the to be converted</param>
        /// <returns>A byte-array with 1, 2 or 3 bytes depending on <paramref name="length"/></returns>
        /// <remarks>
        /// See 9.3.2
        /// </remarks>
        public static byte[] GetLengthData(int length)
        {
            var data = new List<byte>();

            if (length <= sbyte.MaxValue)
            {
                // only first length byte required
                data.Add((byte)length);
            }
            else if (length <= byte.MaxValue)
            {
                // indicate additional length byte use second byte for actual length value
                data.Add(0b1000_0001);
                data.Add((byte)length);
            }
            else if (length <= ushort.MaxValue)
            {
                // indicate two additional length bytes use second and third byte for actual length value
                data.Add(0b1000_0010);
                data.Add((byte)(length >> 8));
                data.Add((byte)length);
            }
            else
            {
                throw new NotSupportedException($"TLV length of more than {ushort.MaxValue} bytes is not supported.");
            }

            return data.ToArray();
        }
    }

    /// <summary>
    /// Tag Part of the data-object
    /// </summary>
    /// <remarks>
    /// See 9.3.1
    /// </remarks>
    public class TlvTag
    {
        /// <summary>
        /// Actual byte data of the tag
        /// </summary>
        public byte[] Data { get; private set; }
        /// <summary>
        /// The class of the tag
        /// </summary>
        public TlvTagFieldClassType Class { get; private set; }
        /// <summary>
        /// If true; the <see cref="TlvItem"/> is primitive; else constructed
        /// </summary>
        public bool IsPrimitive { get; private set; }
        /// <summary>
        /// The number of the Tag
        /// </summary>
        public ushort Number { get; private set; }

        /// <summary>
        /// TLV-Tag
        /// </summary>
        /// <param name="data">The tag data</param>
        public TlvTag(byte[] data)
        {
            Init(data);
        }

        private void Init(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Tag data is empty.");
            }

            if (data.Length > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Implemented support for one or two byte tags only.");
            }

            var tagByte1 = data[0];
            var tagByte2 = data.Length > 1 ? data[1] : 0;

            if ((tagByte2 & 0b1000_0000) != 0)
            {
                throw new NotImplementedException($"Tag-field 0x{data:X4} indicates a third byte following but implemented support for one or two byte tags only.");
            }

            Data = data;

            // Check the b8/b7 to determine the class, according to ZVT chapter 9.3.1		
            var bits = BitHelper.GetBits(tagByte1);
            if (bits.Bit7 && bits.Bit6)
            {
                Class = TlvTagFieldClassType.PrivateClass;
            }
            else if (bits.Bit7 && !bits.Bit6)
            {
                Class = TlvTagFieldClassType.ContextSpecificClass;
            }
            else if (!bits.Bit7 && bits.Bit6)
            {
                Class = TlvTagFieldClassType.ApplicationClass;
            }
            else if (!bits.Bit7 && !bits.Bit6)
            {
                Class = TlvTagFieldClassType.UniversalClass;
            }

            // Check the b6 to determine if primitive or constructed, according to ZVT chapter 9.3.1												
            IsPrimitive = (tagByte1 & 0b0010_0000) == 0;  
            
            Number = (tagByte1 & 0b11111) != 0b11111 ? (ushort)(tagByte1 & 0b11111) : (ushort)tagByte2;
        }
    }
}
