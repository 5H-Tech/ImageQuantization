using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageQuantization
{
    internal class ClusteringClass
    {
        List<Edge> TreeEdges = new List<Edge>();


        static Dictionary<int, List<int>> adj;
        static Dictionary<int, char> color;
        static List<List<int>> clusters;
        static List<RGBPixel> palate;
        static int ind;
        public List<RGBPixel> generatePalete(List<int> dis,List<Edge> mst, int k)
        {
            TreeEdges = mst;
            int x = k;
            int ind;

            while (x > 1)
            {
                ind = getMaxEdge(TreeEdges);
                mst[ind] = removeEdge(TreeEdges[ind]);
                x--;
            }
            List<List<int>> c = getClusters(dis, TreeEdges, k);
            getCentroid();

            //for (int i = 0; i < palate.Count; i++)
            //{
            //    MessageBox.Show(palate[i].ToString());
            //}

            return palate;
        }


        public int getMaxEdge(List<Edge> mst)
        {
            int ind = 0;
            float max = 0;

            for (int i = 0; i < mst.Count; i++)
            {
                if (mst[i].Weight > max)
                {
                    max = mst[i].Weight;
                    ind = i;
                }
            }

            return ind;
        }

        public Edge removeEdge(Edge e)
        {
            Edge ee = new Edge();
            e.src = e.src;
            ee.dst = e.dst;

            ee.Weight = -1;
            return ee;
        }

        public List<List<int>> getClusters(List<int> vertecies,List<Edge> mst,int k)
        {
           
            adj = new Dictionary<int, List<int>>();
            color = new Dictionary<int, char>();
            clusters = new List<List<int>>();
            ind = -1;
            //for (int i = 0; i < k; i++)
            //{
            //    clusters.Add(new List<int>());
            //}


            for (int i = 0; i < vertecies.Count; i++)
            {
                color.Add(vertecies[i], 'w');
                adj.Add(vertecies[i],new List<int>());
            }

            for (int i = 0; i < mst.Count; i++)
            {
                if (mst[i].Weight != -1)
                {
                    adj[mst[i].src].Add(mst[i].dst);
                }
            }


            for (int i = 0; i < vertecies.Count ; i++)
            {
                if(color[vertecies[i]]=='w')
                {
                    ind++;
                    clusters.Add(new List<int>());
                    Dfs(vertecies[i]);                
                }
            }






           // MessageBox.Show(clusters.Count.ToString());

            //for (int i = 0; i < clusters.Count; i++)
            //{
            //    for (int j = 0; j < clusters[i].Count; j++)
            //    {
            //        MessageBox.Show(clusters[i][j].ToString());
            //    }
            //    MessageBox.Show("done");
            //}

            return clusters;
        }

        public static void Dfs(int vertice)
        {
            clusters[ind].Add(vertice);
            color[vertice] = 'g'; 
            for (int j = 0; j < adj[vertice].Count; j++)
            {
                if (adj[vertice][j] != null)
                {                   
                    if (color[adj[vertice][j]] == 'w')
                    {                    
                        Dfs(adj[vertice][j]);
                    }
                }
            }
           
            color[vertice] = 'b';
           
        }

        public void getCentroid()
        {
            colorCodingClass codingClass = new colorCodingClass();
            palate = new List<RGBPixel>();
            for (int i = 0; i < clusters.Count; i++)
            {
                RGBPixel rGBPixel = new RGBPixel(); 
                
                for (int j = 0; j < clusters[i].Count; j++)
                {
                    RGBPixel rGB =  codingClass.decodeColors(clusters[i][j]);
                    rGBPixel.red += rGB.red;
                    rGBPixel.green += rGB.green;
                    rGBPixel.blue += rGB.blue;
                }
                rGBPixel.red /= (byte)clusters[i].Count;
                rGBPixel.green /= (byte)clusters[i].Count;
                rGBPixel.blue /= (byte)clusters[i].Count;
                palate.Add(rGBPixel);
            }
        }
    }
}
