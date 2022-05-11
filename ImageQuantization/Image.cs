using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using Priority_Queue;
namespace ImageQuantization
{

    public struct Edge
    {
        public int src;      // source node
        public int dst;      // destination node
        public float Weight;  // weight of edge
    }
    class Vertex : FastPriorityQueueNode
    {

        /// <summary>
        /// it is a class carry the node and it's parnt node
        /// extind the Fast priority Queue so can be used as node in the perouty quee
        /// </summary>
        public Vertex()
        { }
        public Vertex(int vertex, int? parent)
        {
            vert = vertex;
            parant = parent;
        }
        public int vert { get; set; }          // current vertix
        public int? parant { get; set; }         // parent vertix

    }
    class Image
    {
        RGBPixel[,] aimImage;
        public Image(RGBPixel[,] ImagePixels)
        {
            aimImage = ImagePixels;
        }

        private int codeColors(RGBPixel pixl){

            int enCodedColr = (pixl.red << 16) + (pixl.green << 8) + pixl.blue;
            return enCodedColr;
        }
        private RGBPixel decodeColors(int codedColor)
        {
            RGBPixel res ;
            res.red = (byte)(codedColor >> 16);
            res.green = (byte)(codedColor >> 8);
            res.blue = (byte)(codedColor);
            return res;
        }
        public  List<int> GetDistinctColors()
        {
            // in this fuction i used the set to make sure that all the colors are distinct 
            // but there is a problem i cannot make sure that all the r, g & b are distict at the same time so 
            // I compresed all the r & g & b of each color into a one integer by representing it in a format that looks like the 
            // hex format since the r or g or b colors are integers between  0 -> 255 so take 4 byte each so..

            
            HashSet<int> destincetSet = new HashSet<int>();
            for (int x = 0; x < aimImage.GetLength(0); x++)
            {
                for (int y = 0; y <  aimImage.GetLength(1); y++)
                {
                    int incodedColor = codeColors(aimImage[x, y]);
                    destincetSet.Add(incodedColor);
                }
            }
            List<int> L = destincetSet.ToList();
            return L;
        }
        private double GetWeight(Vertex src, Vertex dst)
        {
            double res;
            RGBPixel srcRGB = decodeColors(src.vert);
            RGBPixel dstRGB  = decodeColors(dst.vert);


            float X = dstRGB.red - srcRGB.red;
            float Y = dstRGB.green - srcRGB.green;
            float Z = dstRGB.blue - srcRGB.blue;
            res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            return res;
        }
        public  List<Edge> BuildingMST(List<int> listOfDisticteColors)
        {
            List<Edge> MSTList = new List<Edge>();
            // final list contains the MST 

            FastPriorityQueue<Vertex> FP = new FastPriorityQueue<Vertex>(listOfDisticteColors.Count);
            // Priority queue sorts the priority of edges' weights each time .

            Vertex[] VP = new Vertex[listOfDisticteColors.Count];
            // array holding each node and it's parent node.

            VP[0] = new Vertex(listOfDisticteColors[0], null); // initializing the first node in the MST.

            FP.Enqueue(VP[0], 0);  // inserting the first node into the priority queue.

            float w;
            for (int i = 1; i < listOfDisticteColors.Count; i++)
            {
                // intializing all the weights with OO value.
                VP[i] = new Vertex(listOfDisticteColors[i], null);
                FP.Enqueue(VP[i], int.MaxValue);
            }
            while (FP.Count != 0)
            {
                Vertex Top = FP.Dequeue();   // get the minimum priority 
                if (Top.parant != null)        // if it is not the starting node.
                {
                    Edge E;
                    E.src = Top.vert;
                    E.dst = (int)(Top.parant);
                    E.Weight = (float)(Top.Priority);
                    MSTList.Add(E);     // add the minimum weight to the MST.
                }
                foreach (var unit in FP)     // modify the priority each time .
                {
                    w = (float) GetWeight(unit, Top);  // calculates the weight between the current node and the top node.
                    if (w < unit.Priority)
                    {
                        unit.parant = Top.vert;
                        FP.UpdatePriority(unit, w);
                    }
                }
            }

            return MSTList;
        }
    }
}
