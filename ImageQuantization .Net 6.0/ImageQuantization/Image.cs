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
        ClusteringClass cluster;
        MappingClass MappingClass;
        colorCodingClass colorCoding;
        RGBPixel[,] aimImage;
        List<int> listOfDistincet = new List<int>();
        List<Edge> minSpanningTreeEdges = new List<Edge>();
        List<Edge> minSpanningTreeEdgesForBultIN = new List<Edge>();
        Dictionary<int, int> palate;
        float totalW = 0;

        public Image(RGBPixel[,] ImagePixels)
        {
            aimImage = ImagePixels;
            cluster = new ClusteringClass();
            colorCoding = new colorCodingClass();
          
        }

        public int getDistinctColors()
        {

            HashSet<int> destincetSet = new HashSet<int>();
            for (int x = 0; x < aimImage.GetLength(0); x++)
            {
                for (int y = 0; y < aimImage.GetLength(1); y++)
                {
                    int incodedColor = colorCoding.codeColors(aimImage[x, y]);
                    destincetSet.Add(incodedColor);
                }
            }
            listOfDistincet = destincetSet.ToList();
            return listOfDistincet.Count;
        }
        
        
        private double getEqldeanDistance2(Edge src, Edge dst)
        {
            double res;
            RGBPixel srcRGB = colorCoding.decodeColors(src.src);
            RGBPixel dstRGB = colorCoding.decodeColors(dst.src);


            float X = dstRGB.red - srcRGB.red;
            float Y = dstRGB.green - srcRGB.green;
            float Z = dstRGB.blue - srcRGB.blue;
            res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            return res;
        }

        private void MSTWithBultInQueue()
        {
            //it is the fast Priorty queu (linked Lib)
            PriorityQueue<Edge, float> queue = new PriorityQueue<Edge, float>();


            //intializing the the queue valuses by infny and without parant node
            for (int i = 0; i < listOfDistincet.Count; i++)
            {
                Edge e;
                e.src = listOfDistincet[i];
                e.dst = 0;
                e.Weight = float.MaxValue;
                queue.Enqueue(e, float.MaxValue);
            }
            float tmp;
            while (queue.Count != 0)
            {
                Edge Top = queue.Dequeue();
                if (Top.dst != 0)       //if this not the root node 
                {
                    // we will take the top of the queue wher the smollest 
                    // edge whaite 
                    Edge E;
                    E.src = Top.src;
                    E.dst = (int)(Top.dst);
                    E.Weight = (float)(Top.Weight);
                    minSpanningTreeEdgesForBultIN.Add(E);
                }
                PriorityQueue<Edge, float> qtmp = new PriorityQueue<Edge, float>();
                while (queue.Count != 0)
                {
                    Edge e = queue.Dequeue();
                    tmp = (float)getEqldeanDistance2(e, Top);
                    if (tmp < e.Weight)
                    {
                        e.dst = Top.src;
                        e.Weight = tmp;
                        qtmp.Enqueue(e, tmp);
                    }
                    else
                    {
                        qtmp.Enqueue(e, e.Weight);
                    }
                }
                queue = qtmp;
            }
        }

        public float sumWithBultInQueue()
        {
            MSTWithBultInQueue();
            float wahit = 0;
            foreach (var item in minSpanningTreeEdges)
                wahit += item.Weight;

            return wahit;
        }

        public float buildingMST()
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
                    totalW += E.Weight;
                    minSpanningTreeEdges.Add(E);
                }
                //relaxing the edges 
                foreach (var unit in q)
                {
                    // @ when this loop done at firest time 
                    // the root node will be the parant off all nodes 
                    // and will the nearst node to the root will be in the top of the queue
                    // in the next iteration we will take the nearst node to the root and do the same then pop it form the q and so on...
                    tmp = (float)getDistanceClass.getEqldeanDistance(unit, Top);
                    if (tmp < unit.Priority)
                    {
                        unit.parant = Top.vert;
                        q.UpdatePriority(unit, tmp);
                    }
                }
            }
            return totalW;
        }

        public RGBPixel[,] makeClister(int k)
        {
            Dictionary<int, int> p = cluster.generatePalete(listOfDistincet, minSpanningTreeEdges, k);
            MappingClass = new MappingClass(p, aimImage);
            RGBPixel[,] y = MappingClass.map();
            return y;
        }

        public double standardDeviation(List<float> tmp)
        {
            double result = 0;
            double sum = 0;



            double mean = getMean(tmp);
            for (int i = 0; i < tmp.Count; i++)
            {
                sum += (Math.Pow((tmp[i] - mean),2));
            }
            sum = sum/tmp.Count;
                
                result = Math.Sqrt(sum);



            return result;
        }

        public double getMean(List<float> tmp)
        {
            double result = 0;


            for (int i = 0; i < tmp.Count; i++)
            {
                result += tmp[i];
            }
            result =result/ tmp.Count();

            return result;
        }


    }
}