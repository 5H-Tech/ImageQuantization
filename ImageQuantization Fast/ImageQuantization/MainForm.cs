using ImageQuantization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        RGBPixel[,] ImageMatrix;

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;

            //creating an object form the imge class 
            Stopwatch stopwatch = new Stopwatch();
            Image im = new Image(ImageMatrix);
            stopwatch.Start();

            ImageMatrix = im.quantize(int.Parse(noClusters.Text));
            

            stopwatch.Stop();
            RunningTime.Text = "" + stopwatch.ElapsedMilliseconds / 1000.0 + " Sec";
            ClusteringClass.fillPalette(listView1);


            distincet_txt.Text = im.noColors.ToString();
            mst_sum_txt.Text = im.totalWeight.ToString();
            clusters_no_txt.Text = "Same";

             //ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            im = null;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //image filters
            openFileDialog1.Filter= "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

    
        private void button1_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;

            //creating an object form the imge class 
            Stopwatch stopwatch = new Stopwatch();
            Image im = new Image(ImageMatrix);
            stopwatch.Start();
         
            ImageMatrix = im.autoClustering();
            
            stopwatch.Stop();
            RunningTime.Text = "" + stopwatch.ElapsedMilliseconds / 1000.0 + " Sec";
            ClusteringClass.fillPalette(listView1);

            distincet_txt.Text = im.noColors.ToString();
            mst_sum_txt.Text = im.totalWeight.ToString();
            clusters_no_txt.Text = im.k.ToString();

            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            im = null;
        }
    }
}
