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
        List<int> listOfDistincet = new List<int>();
        List<Edge> minSpanningTreeEdges = new List<Edge>();

        public Image(RGBPixel[,] ImagePixels)
        {
            aimImage = ImagePixels;
        }

        private int codeColors(RGBPixel pixl)
        {

            int enCodedColr = (pixl.red << 16) + (pixl.green << 8) + pixl.blue;
            return enCodedColr;
        }


        private RGBPixel decodeColors(int codedColor)
        {
            RGBPixel res;
            res.red = (byte)(codedColor >> 16);
            res.green = (byte)(codedColor >> 8);
            res.blue = (byte)(codedColor);
            return res;
        }


        public int getDistinctColors()
        {

            HashSet<int> destincetSet = new HashSet<int>();
            for (int x = 0; x < aimImage.GetLength(0); x++)
            {
                for (int y = 0; y < aimImage.GetLength(1); y++)
                {
                    int incodedColor = codeColors(aimImage[x, y]);
                    destincetSet.Add(incodedColor);
                }
            }
            listOfDistincet = destincetSet.ToList();
            return listOfDistincet.Count;
        }

        private double getEqldeanDistance(Vertex src, Vertex dst)
        {
            double res;
            RGBPixel srcRGB = decodeColors(src.vert);
            RGBPixel dstRGB = decodeColors(dst.vert);


            float X = dstRGB.red - srcRGB.red;
            float Y = dstRGB.green - srcRGB.green;
            float Z = dstRGB.blue - srcRGB.blue;
            res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            return res;
        }


        private void buildingMST()
        {
            //it is the fast Priorty queu (linked Lib)
            FastPriorityQueue<Vertex> q = new FastPriorityQueue<Vertex>(listOfDistincet.Count);

            //intializing the the queue valuses by infny and without parant node
            for (int i = 0; i < listOfDistincet.Count; i++)
                q.Enqueue(new Vertex(listOfDistincet[i], null), int.MaxValue);

            float tmp;
            while (q.Count != 0)
            {
                Vertex Top = q.Dequeue();
                if (Top.parant != null)       //if this not the root node 
                {
                    // we will take the top of the queue wher the smollest 
                    // edge whaite 
                    Edge E;
                    E.src = Top.vert;
                    E.dst = (int)(Top.parant);
                    E.Weight = (float)(Top.Priority);
                    minSpanningTreeEdges.Add(E);
                }
                //relaxing the edges 
                foreach (var unit in q)
                {
                    // @ when this loop done at firest time 
                    // the root node will be the parant off all nodes 
                    // and will the nearst node to the root will be in the top of the queue
                    // in the next iteration we will take the nearst node to the root and do the same then pop it form the q and so on...
                    tmp = (float)getEqldeanDistance(unit, Top);
                    if (tmp < unit.Priority)
                    {
                        unit.parant = Top.vert;
                        q.UpdatePriority(unit, tmp);
                    }
                }
            }
        }

        public float getMSTsum()
        {
            //its just getting the mst sum for printing 
            buildingMST();
            float wahit = 0;
            foreach (var item in minSpanningTreeEdges)
                wahit = wahit + item.Weight;

            return wahit;
        }




    }
}