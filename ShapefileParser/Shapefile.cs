using System.Collections.Generic;

namespace ShapefileParser
{
    public class Shapefile
    {
        public Shapefile(int version, ShapeType shapeType, Shape minimumBoundingBox,
            Dictionary<int, ShapefileRecord> records)
        {
            Version = version;
            ShapeType = shapeType;
            MinimumBoundingBox = minimumBoundingBox;
            Records = records;
        }

        public int Version { get; }
        public ShapeType ShapeType { get; }
        public Shape MinimumBoundingBox { get; }
        public Dictionary<int, ShapefileRecord> Records { get; }
    }
}