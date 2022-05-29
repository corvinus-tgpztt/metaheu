using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Statistics;

namespace Harmonia_kereses
{
    // A metaheurisztika.cs fájl tartalmazza az algoritmust.
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Itt adom meg a függvényt. Most egy egyszerű negált parabola.
        Func<double, double> fx = (x => (-Math.Pow(x,2)+2*x+3));
        Func<double, double, double> fx2 =((x, y) => (-Math.Pow(x,2)-Math.Pow(y,2)+3*x+3*y));
        Func<double, double, double> fx3 = ((x, y) => -(4 * Math.Pow(x, 2) - 2.1 * Math.Pow(x, 4) + Math.Pow(x, 6) / 3 + x * y - 4 * Math.Pow(y, 2) + 4 * Math.Pow(y, 4)));
       // Func<double, double, double> fx3 = ((x, y) => -100 * Math.Pow(y - (Math.Pow(x, 2)), 2) + Math.Pow(1 - x, 2));
        double vegeredmeny = 1.0316285;
        int iteracio = 0;
        double x1 = 0;
        double x2 = 0;
        double fxlegjobb = 0;
        double szazalek = 0;
        TimeSpan futásidő = DateTime.Now - DateTime.Now;



        private void button1_Click(object sender, EventArgs e)
        {
            //Itt letörlöm a listbox-ban lévő elemeket és csak beolvasom az adatokat az űrlapról.
            listBox1.Items.Clear();
            

            //Egy listát ad eredményül az algoritmus. Ezt a listát írom most ki.

            if (radioButton1.Checked)
            {
                textBox2.Text = "1";
                Metaheurisztika a = new Metaheurisztika(
                Convert.ToInt32(textBox1.Text),
                Convert.ToInt32(textBox2.Text),
                Convert.ToDouble(textBox3.Text),
                Convert.ToDouble(textBox4.Text),
                Convert.ToDouble(textBox5.Text),
                fx);
                int i = 1;
                foreach (var x in a.Szamol1())
                {

                    listBox1.Items.Add("sorszám: " + i + ", X= " + Math.Round(x.X, 4) + " F(x)= " + Math.Round(x.Fx, 4));
                    i++;
                }
            }
            else if (radioButton2.Checked)
            {
                
                Metaheurisztika a = new Metaheurisztika(
                Convert.ToInt32(textBox1.Text),
                Convert.ToInt32(textBox2.Text),
                Convert.ToDouble(textBox3.Text),
                Convert.ToDouble(textBox4.Text),
                Convert.ToDouble(textBox5.Text),
                fx2);
                int i = 1;
                foreach (var x in a.Szamol2())
                {

                    listBox1.Items.Add("sorszám: " + i + ", X= " + Math.Round(x.X, 4) + ", Y= " + Math.Round(x.Y, 4) + " F(x)= " + Math.Round(x.Fx, 4));
                    i++;
                }
            }
            else
            {
               
                Metaheurisztika a = new Metaheurisztika(
                Convert.ToInt32(textBox1.Text),
                Convert.ToInt32(textBox2.Text),
                Convert.ToDouble(textBox3.Text),
                Convert.ToDouble(textBox4.Text),
                Convert.ToDouble(textBox5.Text),
                fx3);
                int i = 1;
                DateTime Start = DateTime.Now;
                foreach (var x in a.Szamol2())
                {

                    listBox1.Items.Add("sorszám: " + i + ", X= " + Math.Round(x.X, 4) + ", Y= " + Math.Round(x.Y, 4) + " F(x)= " + Math.Round(x.Fx, 4) +".  " +(Math.Round((x.Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal.");
                    i++;
                    x1 = Math.Round(x.X, 4);
                    x2 = Math.Round(x.Y, 4);
                    fxlegjobb = Math.Round(x.Fx, 4);
                    szazalek = (Math.Round((x.Fx / vegeredmeny * 100), 5));
                    

                }
                DateTime Vége = DateTime.Now;
                futásidő = Vége - Start;
                iteracio = i;

            }

           

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Text = "1";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Text = "2";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Text = "2";
            textBox5.Text = "5";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
            /*   using (StreamWriter sw = new StreamWriter("Output.csv"))
               {
                   sw.WriteLine("Iteracio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum)");
                   for (int j = 0; j < 30; j++)
                   {
                       button1.PerformClick();
                       sw.WriteLine(iteracio + ";" + x1 + ";" + x2 + ";" + fxlegjobb + ";" + szazalek + ";" + futásidő.Milliseconds );



                   }
               }
            */
            var lst = new List<double> { 1 };
            var pontos = new List<double>();
            var lepes = new List<int>();
            var ido = new List<double>();
            using (StreamWriter sw = new StreamWriter("Output.csv"))
            {
                for (int i = 0; i < lst.Count; i++)
                {

                    textBox2.Text = lst[i].ToString();
                    for (int j = 0; j < 50; j++)
                    {
                        button1.PerformClick();
                        pontos.Add(szazalek);
                        ido.Add(futásidő.Milliseconds);
                        lepes.Add(iteracio);

                    }
                    sw.WriteLine(lst[i] + ";" + lepes.Average() + ";" + pontos.Average() + ";" + pontos.Median() + ";" + Math.Sqrt(pontos.Variance()) + ";" + ido.Average());
                    pontos.Clear();
                    ido.Clear();
                    lepes.Clear();

                }
            }



        }
    }
}
