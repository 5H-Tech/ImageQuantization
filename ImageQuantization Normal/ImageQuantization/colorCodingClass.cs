using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    internal class colorCodingClass
    {
        public int codeColors(RGBPixel pixel)
        {

            int enCodedColor = (pixel.red << 16) + (pixel.green << 8) + pixel.blue;
            return enCodedColor;
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
