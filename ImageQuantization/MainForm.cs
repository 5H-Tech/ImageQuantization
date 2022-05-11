using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
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

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            
            //creating an object form the imge class 
            Image im = new Image(ImageMatrix);

            List<int> ListOfDistinctColors = im.getDistinctColors();
            float w = im.getMSTsum();
            MessageBox.Show("Distinct colors= " + ListOfDistinctColors.Count.ToString() + "\nTotal weight= " + w);
            ImageMatrix = im.Quantize(int.Parse(textBox1.Text));

            //List<int> L = ImageOperations.GetDistinctPixels(ImageMatrix);
            //List<Edge> MSTList = ImageOperations.PrimMST(L);
            //float w = 0;
            //foreach (var unit in MSTList)
            //{
            //    w = w + unit.Weight;
            //}
            //MessageBox.Show("Distinct colors= " + L.Count.ToString() + "\nTotal weight= " + w);
            
           
            
           
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }




        
    }
}