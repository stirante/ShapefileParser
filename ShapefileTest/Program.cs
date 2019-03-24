using System;
using System.Data;
using System.IO;
using ShapefileParser;

namespace ShapefileTest
{
    internal class Program
    {
        //tested with files from https://www.naturalearthdata.com/downloads/50m-cultural-vectors/50m-admin-0-countries-2/
        public static void Main(string[] args)
        {
            //parse DBF file with countries shape description
            DataTable countries;
            using (var stream = File.Open("ne_50m_admin_0_countries.dbf", FileMode.Open))
            {
                countries = DbfParser.Parse(stream);
            }
            //parse shapefile
            using (var stream = File.Open("ne_50m_admin_0_countries.shp", FileMode.Open))
            {
                var shapefile = ShapefileParser.ShapefileParser.Parse(stream);

                //match description with shape
                for (var i = 0; i < countries.Rows.Count; i++)
                {
                    var country = countries.Rows[i];
                    //column no. 45 is ISO code
                    if ((string) country[45] == "PL")
                    {
                        Console.WriteLine(country[45] + ": " + shapefile.Records[i + 1].Shapes[0]);
                        //check if Lublin is inside Poland
                        Console.WriteLine(shapefile.Records[i + 1].Shapes[0].IsInside(new Point2D(22.566206, 51.248545)));
                    }
                }
            }
        }
    }
}