using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ShapefileParser
{
    public class DbfParser
    {
        public static DataTable Parse(FileStream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                var table = new DataTable();
                var version = reader.ReadByte();
                Console.WriteLine($"Version: {version}");
                var year = (reader.ReadByte() & 0xff) + 1900;
                var month = (int) reader.ReadByte();
                var day = (int) reader.ReadByte();
                Console.WriteLine($"Last update: {year}/{month}/{day}");
                var rowCount = reader.ReadInt32();
                Console.WriteLine($"Number of rows: {rowCount}");
                var headerLength = reader.ReadInt16();
                Console.WriteLine($"Header length: {headerLength}");
                var rowSize = reader.ReadInt16();
                Console.WriteLine($"Row size: {rowSize}");
                reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
                var fieldSizes = new Dictionary<string, int>();
                while (true)
                {
                    if (reader.ReadByte() == 0x0D)
                    {
                        break;
                    }

                    reader.BaseStream.Position -= 1;
                    var fieldName = reader.ReadZeroDelimitedString();
                    var code = reader.ReadChar();
                    while (code == '\0')
                    {
                        code = reader.ReadChar();
                    }

                    reader.BaseStream.Position += 4;
                    var fieldSize = reader.ReadByte();
                    reader.BaseStream.Position += 15;
                    table.Columns.Add(fieldName, typeof(string));
                    fieldSizes[fieldName] = fieldSize;
                }

                for (var i = 0; i < rowCount; i++)
                {
                    //row status
                    reader.ReadByte();
                    var row = table.NewRow();
                    foreach (var fieldSize in fieldSizes)
                    {
                        row[fieldSize.Key] = Encoding.Default.GetString(reader.ReadBytes(fieldSize.Value)).Trim();
                    }

                    table.Rows.Add(row);
                }

                return table;
            }
        }
    }
}