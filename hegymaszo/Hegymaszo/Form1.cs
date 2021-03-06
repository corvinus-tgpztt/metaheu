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


namespace Hegymaszo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public struct Egyed
        {
            public double X1;
            public double X2;
            public double Fx;

        }
        double delta = 0;
        double max = 10; // A maximum érték  abs értéke. (negatívan is rá fogjuk mérni)
        double szomszedhatar = 0; // milyen környezetben keressük a következő megoldást?
        int maxiteracio = 0; // meddig fut a program legfeljebb
        int i = 0;
        int iter = 0;

        double vegeredmeny = 1.0316285;
        Egyed regi = new Egyed();
        Egyed uj = new Egyed();

        TimeSpan futásidő = DateTime.Now - DateTime.Now;
        List<double> eredmeny = new List<double>();
        double arany = 0;
        Random r = new Random();


        private void button1_Click(object sender, EventArgs e)
        {
            iter = 0;
            DateTime Start = DateTime.Now;
            szomszedhatar = Convert.ToDouble(textBox1.Text);
            maxiteracio = Convert.ToInt32(textBox2.Text);
            int exit = 0;
            double referencia = 0;

            

            //maga a tesztfüggvény

            Func<double, double, double> fx2 = ((x, y) => -(4 * Math.Pow(x, 2) - 2.1 * Math.Pow(x, 4) + Math.Pow(x, 6) / 3 + x * y - 4 * Math.Pow(y, 2) + 4 * Math.Pow(y, 4)));
           // Func<double, double, double> fx2 = ((x, y) => -100* Math.Pow(y-(Math.Pow(x,2)),2) + Math.Pow(1-x,2) );


            //első megoldás generálása
            regi = new Egyed();

            regi.X1 = r.NextDouble() * max;
            if (r.Next(2) == 0)
            {
                regi.X1 = regi.X1 * -1;
            }

            regi.X2 = r.NextDouble() * max;
            if (r.Next(2) == 0)
            {
                regi.X2 = regi.X2 * -1;
            }

            // Fitness számítása - ez maga az Fx lesz. (szerintem)
            regi.Fx = fx2(regi.X1, regi.X2);
            for (i = 0; i < maxiteracio; i++)
            {
                referencia = regi.Fx;

                //A következő megoldás keresése a szomszédsági sugáron belül
                uj = new Egyed();
                
                //Első paraméter
                if (r.Next(2) == 1)
                {
                    //növelem
                    uj.X1 = regi.X1 + (szomszedhatar * r.NextDouble());
                }
                else
                {
                    //növelem
                    uj.X1 = regi.X1 - (szomszedhatar * r.NextDouble());
                }
                //Második paraméter
                if (r.Next(2) == 1)
                {
                    //növelem
                    uj.X2 = regi.X2 + (szomszedhatar * r.NextDouble());
                }
                else
                {
                    //növelem
                    uj.X2 = regi.X2 - (szomszedhatar * r.NextDouble());
                }

                uj.Fx = fx2(uj.X1, uj.X2);

                //Kiszámoljuk a differenciát az új és a régi között.

                delta = uj.Fx - regi.Fx;


                if (delta > 0)
                {
                    //Közelebb kerültünk a maximumhoz.
                    regi = uj;

                }

                double diff = Math.Abs(referencia - regi.Fx);
                if (diff < 0.01)
                {
                    exit++;
                }
                else exit = 0;

                if (exit == 5000)
                {
                    break;
                }

            }
            DateTime Vége = DateTime.Now;
            futásidő = Vége - Start;

            listBox1.Items.Add((i).ToString() + " lépésből az optimum: X1= " + Math.Round(regi.X1,4) + ", X2= " + Math.Round(regi.X2, 4) + " F(x): " + Math.Round(regi.Fx, 5) + ". ---  A sugár "+ szomszedhatar.ToString() + " volt  és " +(Math.Round((regi.Fx/vegeredmeny*100),5))+"%-os az egyezőség az optimummal.");
            iter = i;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var lst = new List<double> { 0.3,0.5,0.7,0.9,1,3,5,7,9,10,20,30,40,50,100,150 };
            var pontos = new List<double>();
            var lepes = new List<int>();
            var ido = new List<double>();
            using (StreamWriter sw = new StreamWriter("Output.csv"))
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    
                    textBox1.Text = lst[i].ToString();
                    for (int j = 0; j < 50; j++)
                    {
                        button1.PerformClick();
                        pontos.Add(Math.Round((regi.Fx / vegeredmeny * 100), 5));
                        ido.Add(futásidő.Milliseconds);
                        lepes.Add(iter);

                    }
                    sw.WriteLine(lst[i] + ";" +lepes.Average()+ ";" + pontos.Average() + ";"+pontos.Median() +";" + Math.Sqrt(pontos.Variance()) + ";" + ido.Average());
                    pontos.Clear();
                    ido.Clear();
                    lepes.Clear();

                }
            }


          /*  using (StreamWriter sw = new StreamWriter("Output.csv"))
            {
                sw.WriteLine("Iteracio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum); szomszedossagi szugar");
                for (int j = 0; j < 30; j++)
                {
                    button1.PerformClick();
                    sw.WriteLine(((i).ToString() + ";" + Math.Round(regi.X1, 4) + ";" + Math.Round(regi.X2, 4) + ";" + Math.Round(regi.Fx, 5) + ";" + (Math.Round((regi.Fx / vegeredmeny * 100), 5)) + ";" + futásidő.Milliseconds + ";" + szomszedhatar));

                   
                    
                }
            }*/
        }
    }
}
