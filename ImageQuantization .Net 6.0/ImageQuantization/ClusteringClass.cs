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
        static Dictionary<int, int> palate;
        static int ind;

        public Dictionary<int,int> generatePalette(List<int> dis,List<Edge> mst, int k)
        {
            TreeEdges = mst;
            int x = k;
            int ind;
            while (x > 1)
            {
                ind = getInxMaxEdge(TreeEdges);
                mst[ind] = removeEdge(TreeEdges[ind]);
                x--;
            }
            List<List<int>> c = getClusters(dis, TreeEdges);
            getCentroid();
            return palate;
        }

        public int getInxMaxEdge(List<Edge> mst)
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
            Edge e2 = new Edge();
            e2.src = e.src;
            e2.dst = e.dst;

            e2.Weight = -1;
            return e2;
        }

        public List<List<int>> getClusters(List<int> vertecies,List<Edge> mst)
        {
           
            adj = new Dictionary<int, List<int>>();
            color = new Dictionary<int, char>();
            clusters = new List<List<int>>();
            ind = -1;
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
                    adj[mst[i].dst].Add(mst[i].src);
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

            return clusters;
        }

        public static void Dfs(int vertex)
        {
            clusters[ind].Add(vertex);
            color[vertex] = 'g'; //visited
            for (int j = 0; j < adj[vertex].Count; j++)
            {
                if (color[adj[vertex][j]] == 'w') //new
                {
                    Dfs(adj[vertex][j]);
                }
            }
            color[vertex] = 'b'; //explored
           
        }

        public void getCentroid()
        {
            colorCodingClass codingClass = new colorCodingClass();
            palate = new Dictionary<int, int>();
            for (int i = 0; i < clusters.Count; i++)
            {
                int red = 0, gree = 0, blue = 0;
                
                for (int j = 0; j < clusters[i].Count; j++)
                {
                    RGBPixel rGB =  codingClass.decodeColors(clusters[i][j]);
                    red += rGB.red;
                    gree += rGB.green;
                    blue += rGB.blue;
                }
                red /= clusters[i].Count;
                gree /= clusters[i].Count;
                blue /= clusters[i].Count;

                RGBPixel p;
                p.red = (byte)red;
                p.green = (byte)gree;
                p.blue = (byte)blue;

                int mean = codingClass.codeColors(p);

                foreach (var l in clusters[i])
                {
                    palate.Add(l, mean);
                }
            }
        }
    }
}
