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
    class Edge : FastPriorityQueueNode
    {
        public int vert;
        public int parant;
        
        public Edge(int vertex, int parent)
        {
            this.vert = vertex;
            this.parant = parent;
        }
        public Edge(int vertex, int parent,float weight)
        {
            this.vert = vertex;
            this.parant = parent;
            this.Priority = weight;

        }
 
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
           // MessageBox.Show(minSpanningTreeEdges.Count.ToString());
            Dictionary<int,int> p = cluster.generatePalette(listOfDistinct, minSpanningTreeEdges, k);
            MappingClass = new MappingClass(p, aimImage);
            RGBPixel[,] y = MappingClass.map();
            return y;
        }



        public int getK(List<Edge> temp)
        {
            List<Edge> auto = new List<Edge>();

            foreach (Edge e in temp)
            {
                auto.Add(e);
            }
            int r = 0;
            double oldStv = 0;
            double newStv = 0;
            double stv = 0;
            double mean = 0;
    

            oldStv = standardDeviation(auto);
            mean = (double)getMean(auto);

            while (auto.Count > 0)
            {
                double max = 0;
                int index = 0;
              
                for (int j = 0; j < auto.Count; j++)
                {
                    if (Math.Abs(auto[j].Priority - mean) > max)
                    {
                        max = Math.Abs(auto[j].Priority - mean);
                        index = j;
                    }
                }
                auto.RemoveAt(index);
                r += 1;
                newStv = standardDeviation(auto);
                mean = (double)getMean(auto);
              
              
                if (Math.Abs(oldStv - newStv) < 0.0001)
                {
                    break;
                }
                oldStv = newStv;

            }
            MessageBox.Show("num of clusters is "+(r + 1).ToString());
            return r;
        }


        public double standardDeviation(List<Edge> tmp)
        {
            double result = 0;
            double sum = 0;



            double mean = getMean(tmp);
            for (int i = 0; i < tmp.Count; i++)
            {
                sum += (Math.Pow((tmp[i].Priority - mean),2));
            }
            sum = sum/(tmp.Count-1);
                
                result = Math.Sqrt(sum);



            return result;
        }

        public double getMean(List<Edge> tmp)
        {
            double result = 0;


            for (int i = 0; i < tmp.Count; i++)
            {
                result += tmp[i].Priority;
            }
            result =result/ (tmp.Count()-1);

            return result;
        }


        public RGBPixel[,] quintize(int k)
        {
            getDistinctColors();
            buildingMST();
            return makeCluster(k);
        }

        public RGBPixel[,] autoClustring()
        {
         
            getDistinctColors();
            buildingMST();         
            int k = getK(minSpanningTreeEdges);
            return makeCluster(k);
        }

    }
}