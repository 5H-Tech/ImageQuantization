using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    internal class MappingClass
    {
        Dictionary<int,int> palate;
        public RGBPixel[,] ImageMatrix;
        colorCodingClass colorCodingClass;
   


        public  MappingClass(Dictionary<int, int> palate, RGBPixel[,] ImageMatrix)
        {
            this.palate = palate;
            this.ImageMatrix = ImageMatrix;
            colorCodingClass = new colorCodingClass();
        }

      
        public RGBPixel[,]  map()
        {
            int width = ImageMatrix.GetLength(1);
            int height = ImageMatrix.GetLength(0);
            int r, g, b;
            int key = 0;
            int value = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    r = ImageMatrix[y, x].red;
                    g = ImageMatrix[y, x].green;
                    b = ImageMatrix[y, x].blue;
                    RGBPixel p;
                    p.red = (byte)r;
                    p.green = (byte)g;
                    p.blue = (byte)b;
                    key = colorCodingClass.codeColors(p);
                    value = palate[key];

                    ImageMatrix[y, x] = colorCodingClass.decodeColors(value);
                  
                }
            }
          
            return ImageMatrix;
        }

    }
}
