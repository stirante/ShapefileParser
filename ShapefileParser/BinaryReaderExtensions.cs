using System;
using System.Collections.Generic;
using System.IO;

namespace ShapefileParser
{
    internal static class BinaryReaderExtensions
    {
        public static int ReadBigEndianInt32(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static string ReadZeroDelimitedString(this BinaryReader reader)
        {
            var chars = new List<char>();
            var current = reader.ReadChar();
            while (current != '\0')
            {
                chars.Add(current);
                current = reader.ReadChar();
            }

            return new string(chars.ToArray());
        }
    }
}