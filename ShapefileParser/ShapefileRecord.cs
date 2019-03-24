namespace ShapefileParser
{
    public class ShapefileRecord
    {
        public ShapefileRecord(int recordNumber, ShapeType shapeType, Shape[] shapes, Shape boundingBox)
        {
            RecordNumber = recordNumber;
            ShapeType = shapeType;
            Shapes = shapes;
            BoundingBox = boundingBox;
        }

        public int RecordNumber { get; }
        public ShapeType ShapeType { get; }
        public Shape[] Shapes { get; }
        public Shape BoundingBox { get; }
    }

    internal class RecordBuilder
    {
        public RecordBuilder(int recordNumber)
        {
            RecordNumber = recordNumber;
        }

        public int RecordNumber { get; }
        public ShapeType ShapeType { get; set; }
        public Shape[] Shapes { get; set; }
        public Shape BoundingBox { get; set; }

        public ShapefileRecord Build()
        {
            return new ShapefileRecord(RecordNumber, ShapeType, Shapes, BoundingBox);
        }
    }
}