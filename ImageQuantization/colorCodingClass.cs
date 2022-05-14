using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    internal class colorCodingClass
    {
        public int codeColors(RGBPixel pixl)
        {

            int enCodedColr = (pixl.red << 16) + (pixl.green << 8) + pixl.blue;
            return enCodedColr;
        }


        public RGBPixel decodeColors(int codedColor)
        {
            RGBPixel res;
            res.red = (byte)(codedColor >> 16);
            res.green = (byte)(codedColor >> 8);
            res.blue = (byte)(codedColor);
            return res;
        }
    }
}
