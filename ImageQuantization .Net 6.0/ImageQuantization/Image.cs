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

    //public struct Edge:FastPriorityQueueNode
    //{
    //    public int src;      // source node
    //    public int? dst;      // destination node
    //    public float Weight;  // weight of edge
        
    //}
    class Edge : FastPriorityQueueNode
    {
        public int vert;
        public int parant;
        //public float Weight;
        public Edge(int vertex, int parent)
        {
            this.vert = vertex;
            this.parant = parent;
        }
        public Edge(int vertex, int parent,float weight)
        {
            this.vert = vertex;
            this.parant = parent;
            //this.Weight = weight;
            this.Priority = weight;

        }
        //public int parant
        //{
        //    return this.parant;
        //}
        //public void setParant(int input)
        //{
        //    parant = input;
        //}
        //public int getSalf()
        //{
        //    return vert;
        //}
        //public void setSalf(int input)
        //{
        //    vert = input;
        //}
        //public float Weight
        //{
        //    return this.Weight;
        //}
        //public void setWeight(float input)
        //{
        //    this.Weight = input;
        //}
    }
    class Image
    {
        ClusteringClass cluster;
        MappingClass MappingClass;
        colorCodingClass colorCoding;
        RGBPixel[,] aimImage;
        List<int> listOfDistinct = new List<int>();
        List<Edge> minSpanningTreeEdges = new List<Edge>();
        public float totalWahit = 0;
        public int noColors = 0;

        public Image(RGBPixel[,] ImagePixels)
        {
            aimImage = ImagePixels;
            cluster = new ClusteringClass();
            colorCoding = new colorCodingClass();
          
        }

        public void getDistinctColors()
        {

            HashSet<int> distinctSet = new HashSet<int>();
            for (int x = 0; x < aimImage.GetLength(0); x++)
            {
                for (int y = 0; y < aimImage.GetLength(1); y++)
                {
                    int encodedColor = colorCoding.codeColors(aimImage[x, y]);
                    distinctSet.Add(encodedColor);
                }
            }
            listOfDistinct = distinctSet.ToList();
            noColors = listOfDistinct.Count;
        }

        private void buildingMST()
        {
            FastPriorityQueue<Edge> fQueue = new FastPriorityQueue<Edge>(listOfDistinct.Count);
            for (int i = 0; i < listOfDistinct.Count; i++)
                fQueue.Enqueue(new Edge(listOfDistinct[i], -1), int.MaxValue);

            float tmp;
            while (fQueue.Count != 0)
            {
                Edge front = fQueue.Dequeue();
                if (front.parant != -1)
                {
                    totalWahit += front.Priority;
                    /*front.Weight=front.Priority;*/
                    minSpanningTreeEdges.Add(front);
                }
                foreach (var v in fQueue)
                {
                    tmp = (float)getDistanceClass.getEculideanDistance(v, front);
                    if (tmp < v.Priority)
                    {
                        v.parant=(front.vert);
                        fQueue.UpdatePriority(v, tmp);
                    }
                }
            }
        }

        public RGBPixel[,] makeCluster(int k)
        {
            Dictionary<int,int> p = cluster.generatePalette(listOfDistinct, minSpanningTreeEdges, k);
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


        public RGBPixel[,] quintize(int k)
        {
            getDistinctColors();
            buildingMST();
            return makeCluster(k);
        }


        #region MST With the Bultin Pirotry queue

        //List<Edge> minSpanningTreeEdgesForBultIN = new List<Edge>();
        //private double getEqldeanDistance(Edge src, Edge dst)
        //{
        //    double res;
        //    RGBPixel srcRGB = colorCoding.decodeColors(src.getSalf());
        //    RGBPixel dstRGB = colorCoding.decodeColors(dst.getSalf());


        //    float X = dstRGB.red - srcRGB.red;
        //    float Y = dstRGB.green - srcRGB.green;
        //    float Z = dstRGB.blue - srcRGB.blue;
        //    res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        //    return res;
        //}

        //private void MSTWithBultInQueue()
        //{

        //    PriorityQueue<Edge, float> queue = new PriorityQueue<Edge, float>();

        //    for (int i = 0; i < listOfDistinct.Count; i++)
        //    {
        //        Edge e;
        //        e.src = listOfDistinct[i];
        //        e.dst = 0;
        //        e.Weight = float.MaxValue;
        //        queue.Enqueue(e, float.MaxValue);
        //    }
        //    float tmp;
        //    while (queue.Count != 0)
        //    {
        //        Edge Top = queue.Dequeue();
        //        if (Top.dst != 0)       //if this not the root node 
        //        {
        //            // we will take the top of the queue wher the smollest 
        //            // edge whaite 
        //            Edge E;
        //            E.src = Top.src;
        //            E.dst = (int)(Top.dst);
        //            E.Weight = (float)(Top.Weight);
        //            minSpanningTreeEdgesForBultIN.Add(E);
        //        }
        //        PriorityQueue<Edge, float> qtmp = new PriorityQueue<Edge, float>();
        //        while (queue.Count != 0)
        //        {
        //            Edge e = queue.Dequeue();
        //            tmp = (float)getEqldeanDistance(e, Top);
        //            if (tmp < e.Weight)
        //            {
        //                e.dst = Top.src;
        //                e.Weight = tmp;
        //                qtmp.Enqueue(e, tmp);
        //            }
        //            else
        //            {
        //                qtmp.Enqueue(e, e.Weight);
        //            }
        //        }
        //        queue = qtmp;
        //    }
        //}

        //public float sumWithBultInQueue()
        //{
        //    MSTWithBultInQueue();
        //    float wahit = 0;
        //    foreach (var item in minSpanningTreeEdges)
        //        wahit += item.Weight;

        //    return wahit;
        //}
        #endregion

    }
}