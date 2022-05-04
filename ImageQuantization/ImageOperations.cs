using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using Priority_Queue;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    public struct  Edge
    {
        public int src;      // source node
        public int dst;      // destination node
        public float Weight;  // weight of edge
    }
    class VertexParent : FastPriorityQueueNode
    {
        // 
        public VertexParent()
        { }
        public VertexParent(int vertex, int? parent)
        {
            V = vertex;
            P = parent;
        }
        public int V { get; set; }          // current vertix
        public int? P { get; set; }         // parent vertix

    }
    
  
    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }
        
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        public static List<int> GetDistinctColors(RGBPixel[,] ImageMatrix)
        {
            // in this fuction i used the set to make sure that all the colors are distinct 
            // but there is a problem i cannot make sure that all the r, g & b are distict at the same time so 
            // I compresed all the r & g & b of each color into a one integer by representing it in a format that looks like the 
            // hex format since the r or g or b colors are integers between  0 -> 255 so take 4 byte each so..

            int width = ImageMatrix.GetLength(1);
            int height = ImageMatrix.GetLength(0);
            int r, g, b;
            HashSet<int> S = new HashSet<int>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Reading the r and g and b from the color 
                    r = ImageMatrix[y, x].red;
                    g = ImageMatrix[y, x].green;
                    b = ImageMatrix[y, x].blue;
                    // the R will be Shifted to the left by 16 bits ( 8 for G and 8 for B)
                    // the G will be Shifted to the left by 8 bits ( 8 for B)
                    // the B will not be shifted since it is the last one
                    S.Add((r << 16) + (g << 8) + b);
                    // after all of this the color of the set becomes distinct 

                }
            }
            List<int> L = S.ToList();
            return L;
        }
        private static float CalcWeight(VertexParent V1, VertexParent V2)
        {
            byte red1, red2;
            byte green1, green2;
            byte blue1, blue2;
            //decrpting the colors from the set before 
            red1 = (byte)(V1.V >> 16);
            red2 = (byte)(V2.V >> 16);
            green1 = (byte)(V1.V >> 8);
            green2 = (byte)(V2.V >> 8);
            blue1 = (byte)(V1.V);
            blue2 = (byte)(V2.V);
            return (float)Math.Sqrt((red2 - red1) * (red2 - red1) + (green2 - green1) * (green2 - green1) + (blue2 - blue1) * (blue2 - blue1));
        }
        public static List<Edge> BuildingMST(List<int> listOfDisticteColors)
        {
            List<Edge> MSTList = new List<Edge>();
            // final list contains the MST 

            FastPriorityQueue<VertexParent> FP = new FastPriorityQueue<VertexParent>(listOfDisticteColors.Count);
            // Priority queue sorts the priority of edges' weights each time .

            VertexParent[] VP = new VertexParent[listOfDisticteColors.Count];
            // array holding each node and it's parent node.

            VP[0] = new VertexParent(listOfDisticteColors[0], null); // initializing the first node in the MST.

            FP.Enqueue(VP[0], 0);  // inserting the first node into the priority queue.

            float w;
            for (int i = 1; i < listOfDisticteColors.Count; i++)
            {
                // intializing all the weights with OO value.
                VP[i] = new VertexParent(listOfDisticteColors[i], null);
                FP.Enqueue(VP[i], int.MaxValue);
            }
            while (FP.Count != 0)
            {
                VertexParent Top = FP.Dequeue();   // get the minimum priority 
                if (Top.P != null)        // if it is not the starting node.
                {
                    Edge E;
                    E.src = Top.V;
                    E.dst = (int)(Top.P);
                    E.Weight = (float)(Top.Priority);
                    MSTList.Add(E);     // add the minimum weight to the MST.
                }
                foreach (var unit in FP)     // modify the priority each time .
                {
                    w = CalcWeight(unit, Top);  // calculates the weight between the current node and the top node.
                    if (w < unit.Priority)
                    {
                        unit.P = Top.V;
                        FP.UpdatePriority(unit, w);
                    }
                }
            }

            return MSTList;
        }
       


        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


       /// <summary>
       /// Apply Gaussian smoothing filter to enhance the edge detection 
       /// </summary>
       /// <param name="ImageMatrix">Colored image matrix</param>
       /// <param name="filterSize">Gaussian mask size</param>
       /// <param name="sigma">Gaussian sigma</param>
       /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

           
            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }


    }
}
