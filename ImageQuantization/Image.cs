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
        List<Edge> MSTList = new List<Edge>();     // final list contains the MST 
        List<HashSet<int>> clusters = new List<HashSet<int>>();
        Dictionary<int, int> resultPalette = new Dictionary<int, int>();

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


        public  List<int> getDistinctColors()
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
            listOfDistincet = destincetSet.ToList();
            return listOfDistincet;
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


        private List<Edge> buildingMST()
        {
            
            FastPriorityQueue<Vertex> FP = new FastPriorityQueue<Vertex>(listOfDistincet.Count);
            // Priority queue sorts the priority of edges' weights each time .

            Vertex[] VP = new Vertex[listOfDistincet.Count];
            // array holding each node and it's parent node.

            VP[0] = new Vertex(listOfDistincet[0], null); // initializing the first node in the MST.

            FP.Enqueue(VP[0], 0);  // inserting the first node into the priority queue.

            float w;
            for (int i = 1; i < listOfDistincet.Count; i++)
            {
                // intializing all the weights with OO value.
                VP[i] = new Vertex(listOfDistincet[i], null);
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

        public float getMSTsum()
        {
            List<Edge> tree = buildingMST();
            float wahit=0;
            foreach (var unit in tree)
            {
                wahit = wahit + unit.Weight;
            }


            return wahit;
        }




        private void DFS(int cur, ref HashSet<int> visited, ref Dictionary<int, List<int>> neighbours, ref HashSet<int> cluster)
        {
            visited.Add(cur);
            cluster.Add(cur);
            foreach (var neighbour in neighbours[cur])
            {
                if (!visited.Contains(neighbour))
                    DFS(neighbour, ref visited, ref neighbours, ref cluster);
            }
        }


        private void Cluster( int k)
        {
            HashSet<int> visited = new HashSet<int>();
            float maxweight = 0;
            int maxindex = 0;
            for (int j = 0; j < k - 1; j++)
            {
                maxweight = 0;
                maxindex = 0;
                for (int i = 0; i < MSTList.Count; i++)
                {
                    if (MSTList[i].Weight > maxweight)
                    {
                        maxweight = MSTList[i].Weight;
                        maxindex = i;
                    }
                }
                Edge e;
                e.src = MSTList[maxindex].src;
                e.dst = MSTList[maxindex].dst;
                e.Weight = 0;
                MSTList[maxindex] = e;
            }
            Dictionary<int, List<int>> neighbours = new Dictionary<int, List<int>>();
            foreach (var edge in MSTList)
            {
                if (edge.Weight != 0)
                {
                    if (neighbours.ContainsKey(edge.src))
                    {
                        neighbours[edge.src].Add(edge.dst);
                    }
                    else
                    {
                        List<int> l = new List<int>();
                        l.Add(edge.dst);
                        neighbours.Add(edge.src, l);
                    }// dst
                    if (neighbours.ContainsKey(edge.dst))
                    {
                        neighbours[edge.dst].Add(edge.src);
                    }
                    else
                    {
                        List<int> l = new List<int>();
                        l.Add(edge.src);
                        neighbours.Add(edge.dst, l);
                    }
                }
                else
                {
                    if (!neighbours.ContainsKey(edge.src))
                    {
                        List<int> l = new List<int>();
                        neighbours.Add(edge.src, l);
                    }
                    if (!neighbours.ContainsKey(edge.dst))
                    {
                        List<int> l = new List<int>();
                        neighbours.Add(edge.dst, l);
                    }
                }
            }
            int q = 0;
            foreach (var vertex in neighbours)
            {
                if (!visited.Contains(vertex.Key))
                {
                    HashSet<int> h = new HashSet<int>();
                    DFS(vertex.Key, ref visited, ref neighbours, ref h);
                    clusters.Add(h);
                    q++;
                }
            }
            int[] averages = new int[k];


            //return clusters;
        }

        public void Palette()
        {
            
            int r;
            int g;
            int b;
            int value = 0;
            foreach (var set in clusters)
            {
                r = 0;
                g = 0;
                b = 0;
                foreach (var unit in set)
                {
                    r = (r + (byte)(unit >> 16));
                    g = (g + (byte)(unit >> 8));
                    b = (b + (byte)(unit));
                }
                r = (r / set.Count);
                g = (g / set.Count);
                b = (b / set.Count);
                value = (r << 16) + (g << 8) + (b);
                foreach (var unit in set)
                {
                    resultPalette.Add(unit, value);
                }
            }
            //return resultPalette;
        }
        public RGBPixel[,] Quantize( int k)
        {
            Cluster(k);
            Palette();
            int width = aimImage.GetLength(1);
            int height = aimImage.GetLength(0);
            int r, g, b;
            int key = 0;
            int value = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    r = aimImage[y, x].red;
                    g = aimImage[y, x].green;
                    b = aimImage[y, x].blue;
                    key = (r << 16) + (g << 8) + b;
                    value = resultPalette[key];
                    aimImage[y, x].red = (byte)(value >> 16);
                    aimImage[y, x].green = (byte)(value >> 8);
                    aimImage[y, x].blue = (byte)(value);
                }
            }
            return aimImage;
        }
   
        
    }
}
