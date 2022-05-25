using System;
using System.Collections.Generic;
using System.Drawing;
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
        static Dictionary<int, int> palette;
        static int ind;

        public Dictionary<int,int> generatePalette(List<int> dis,List<Edge> mst, int k)
        {
            TreeEdges = mst;
           // MessageBox.Show(mst.Count.ToString());
            int x = k;
            int ind;
            while (x > 1)
            {
                ind = getInxMaxEdge(TreeEdges);
                if(ind != 0)
                     TreeEdges[ind] = removeEdge(TreeEdges[ind]);
                x--;
            }
            List<List<int>> c = getClusters(dis, TreeEdges);
            getCentroid();
            return palette;
        }

        public int getInxMaxEdge(List<Edge> mst)
        {
            int ind = 0;
            float max = 0;

            for (int i = 0; i < mst.Count; i++)
            {
                if (mst[i].Priority > max)
                {
                    max = mst[i].Priority;
                    ind = i;
                }
            }

            return ind;
        }

        public Edge removeEdge(Edge e)
        {
            return new Edge(e.vert,e.parent,-1);
 
        }

        public List<List<int>> getClusters(List<int> vertices, List<Edge> mst)
        {

            adj = new Dictionary<int, List<int>>();
            color = new Dictionary<int, char>();
            clusters = new List<List<int>>();
            ind = -1;
            for (int i = 0; i < vertices.Count; i++)
            {
                color.Add(vertices[i], 'w');
                adj.Add(vertices[i], new List<int>());
            }

            for (int i = 0; i < mst.Count; i++)
            {
                if (mst[i].Priority != -1) //isn't a removed edge
                {
                    adj[mst[i].vert].Add(mst[i].parent);
                    adj[mst[i].parent].Add(mst[i].vert);
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                if (color[vertices[i]] == 'w')
                {
                    ind++;
                    clusters.Add(new List<int>());
                    Dfs(vertices[i]);
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
            palette = new Dictionary<int, int>();
            for (int i = 0; i < clusters.Count; i++)
            {
                int red = 0, green = 0, blue = 0;
                
                for (int j = 0; j < clusters[i].Count; j++)
                {
                    RGBPixel rGB =  codingClass.decodeColors(clusters[i][j]);
                    red += rGB.red;
                    green += rGB.green;
                    blue += rGB.blue;
                }
                red /= clusters[i].Count;
                green /= clusters[i].Count;
                blue /= clusters[i].Count;

                RGBPixel p;
                p.red = (byte)red;
                p.green = (byte)green;
                p.blue = (byte)blue;

                int mean = codingClass.codeColors(p);

                foreach (var l in clusters[i])
                {
                    palette.Add(l, mean);
                }
            }
        }
        public static void fillPalette(ListView list)
        {
            colorCodingClass c = new colorCodingClass();
            String red,blue,green;
            String keyRed, keyBlue, keyGreen;
            list.Items.Clear();
            ListViewItem listItem;
            foreach (var p in palette)
            {
                RGBPixel newColor=c.decodeColors(p.Value);
                red = newColor.red.ToString();
                green = newColor.green.ToString();
                blue = newColor.blue.ToString();

                RGBPixel KeyColor = c.decodeColors(p.Key);
                keyRed = KeyColor.red.ToString();
                keyGreen = KeyColor.green.ToString();
                keyBlue = KeyColor.blue.ToString();
                
                listItem = new ListViewItem(keyRed);
                listItem.SubItems.Add(keyGreen);
                listItem.SubItems.Add(keyBlue);
                listItem.SubItems.Add(red);
                listItem.SubItems.Add(green);
                listItem.SubItems.Add(blue);

                list.Items.Add(listItem);
            }
        }
    }
}
