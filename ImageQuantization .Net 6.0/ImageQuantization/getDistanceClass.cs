using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    internal class getDistanceClass
    {
        static colorCodingClass colorCodingClass;
        public static double getEculideanDistance(Vertex src, Vertex dst)
        {
            colorCodingClass =new colorCodingClass();
            double res;
            RGBPixel srcRGB = colorCodingClass.decodeColors(src.getSalf());
            RGBPixel dstRGB = colorCodingClass.decodeColors(dst.getSalf());


            float X = dstRGB.red - srcRGB.red;
            float Y = dstRGB.green - srcRGB.green;
            float Z = dstRGB.blue - srcRGB.blue;
            res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            return res;
        }


        //public static double getEqldeanDistancee(RGBPixel srcRGB, RGBPixel dstRGB)
        //{
        //    colorCodingClass = new colorCodingClass();
        //    double res;
           

        //    float X = dstRGB.red - srcRGB.red;
        //    float Y = dstRGB.green - srcRGB.green;
        //    float Z = dstRGB.blue - srcRGB.blue;
        //    res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        //    return res;
        //}
    }
}
