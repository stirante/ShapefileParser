using System;
using System.Collections.Generic;
using System.IO;

namespace ShapefileParser
{
    public class ShapefileParser
    {
        public static bool DEBUG = false;

        public static Shapefile Parse(FileStream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (reader.ReadBigEndianInt32() != 0x0000270a)
                {
                    throw new ParserException("Invalid magic value!");
                }

                // skipping 5 unused uint32
                for (var i = 0; i < 5; i++)
                {
                    reader.ReadBigEndianInt32();
                }

                var fileLength = reader.ReadBigEndianInt32();
                if (DEBUG)
                {
                    Console.WriteLine("File length: " + fileLength);
                }

                var version = reader.ReadInt32();
                if (DEBUG)
                {
                    Console.WriteLine("Version: " + version);
                }

                var shapeType = (ShapeType) reader.ReadInt32();
                if (DEBUG)
                {
                    Console.WriteLine("ShapeType: " + shapeType);
                }

                //values currently unsupported:
                //minimumBoundingRectangle
                var minimumBoundingBox = ReadBoundingBox(reader);
                //minZ
                reader.ReadDouble();
                //maxZ
                reader.ReadDouble();
                //minM
                reader.ReadDouble();
                //maxM
                reader.ReadDouble();

                // records
                var records = new Dictionary<int, ShapefileRecord>();
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var record = ReadRecord(reader);
                    records[record.RecordNumber] = record;
                }

                return new Shapefile(version, shapeType, minimumBoundingBox, records);
            }
        }

        private static ShapefileRecord ReadRecord(BinaryReader reader)
        {
            //record header
            var recordNumber = reader.ReadBigEndianInt32();
            //recordLength
            reader.ReadBigEndianInt32();
            //record content
            var builder = new RecordBuilder(recordNumber);
            var recordShapeType = (ShapeType) reader.ReadInt32();
            builder.ShapeType = recordShapeType;
            switch (recordShapeType)
            {
                case ShapeType.NullShape:
                    break;
                case ShapeType.Point:
                    builder.Shapes = new[] {new Shape(new[] {new Point2D(reader.ReadDouble(), reader.ReadDouble())})};
                    break;
                case ShapeType.Polygon:
                    builder.BoundingBox = ReadBoundingBox(reader);
                    var numParts = reader.ReadInt32();
                    var numPoints = reader.ReadInt32();
                    var parts = new int[numParts];
                    for (var i = 0; i < numParts; i++)
                    {
                        parts[i] = reader.ReadInt32();
                    }

                    var points = new Point2D[numPoints];
                    for (var i = 0; i < numPoints; i++)
                    {
                        points[i] = new Point2D(reader.ReadDouble(), reader.ReadDouble());
                    }

                    builder.Shapes = new Shape[numParts];
                    for (var i = 0; i < numParts; i++)
                    {
                        var shape = new List<Point2D>();
                        var j = parts[i];
                        var first = points[j];
                        while (!Equals(first, points[j]) || j == parts[i])
                        {
                            shape.Add(points[j]);
                            j++;
                        }

                        shape.Add(points[j]);
                        builder.Shapes[i] = new Shape(shape.ToArray());
                    }

                    break;
                default:
                    throw new Exception("Unsupported ShapeType: " + recordShapeType);
            }

            return builder.Build();
        }

        private static Shape ReadBoundingBox(BinaryReader reader)
        {
            var minX = reader.ReadDouble();
            var minY = reader.ReadDouble();
            var maxX = reader.ReadDouble();
            var maxY = reader.ReadDouble();
            return new Shape(new[]
            {
                new Point2D(minX, minY),
                new Point2D(minX, maxY),
                new Point2D(maxX, maxY),
                new Point2D(maxX, minY)
            });
        }
    }

    public enum ShapeType
    {
        NullShape = 0,
        Point = 1,
        Polyline = 3,
        Polygon = 5,
        MultiPoint = 8,
        PointZ = 11,
        PolylineZ = 13,
        PolygonZ = 15,
        MultiPointZ = 18,
        PointM = 21,
        PolylineM = 23,
        PolygonM = 25,
        MultiPointM = 28,
        MultiPatch = 31
    }
}