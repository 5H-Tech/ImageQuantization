using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    internal class MappingClass
    {
        List<RGBPixel> palate;
        public RGBPixel[,] ImageMatrix;
        colorCodingClass colorCodingClass;
   


        public  MappingClass(List<RGBPixel>palate, RGBPixel[,] ImageMatrix)
        {
            this.palate = palate;
            this.ImageMatrix = ImageMatrix;
            colorCodingClass = new colorCodingClass();
        }

      
        public RGBPixel[,]  map()
        {        
            for (int x = 0; x < ImageMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < ImageMatrix.GetLength(1); y++)
                {
                    float min = int.MaxValue;
                    int minInd = 0;
                    for (int i = 0; i < palate.Count; i++)
                    {
                        int f = (int)getDistanceClass.getEqldeanDistancee(ImageMatrix[x,y] ,palate[i]);
                        if (f<min)
                        {
                            min = f;
                            minInd = i;
                        }
                    }
                    ImageMatrix[x, y]=palate[minInd];
                }
            }
            return ImageMatrix;
        }

    }
}
