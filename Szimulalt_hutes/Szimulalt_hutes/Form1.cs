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


namespace Szimulalt_hutes
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

        double max = 10; // A maximum érték  abs értéke. (negatívan is rá fogjuk mérni)
        double homerseklet =0;
        double delta = 0;
        double lehules = 0.9999;
        double minhom = 0.00001; // Itt all le
        double time=0; // ennyi ideig marad a homersekleten
        List<double> eredmeny = new List<double>();
        Random r = new Random();

        Egyed regi = new Egyed();
        Egyed uj = new Egyed();
        
        TimeSpan futásidő = DateTime.Now - DateTime.Now;
        double vegeredmeny = 1.0316285;
        DateTime Vége;
        int iteracio;

        private void button1_Click_1(object sender, EventArgs e)
        {
            DateTime Start = DateTime.Now;
            homerseklet = Convert.ToDouble(textBox1.Text);
            time = Convert.ToDouble(textBox2.Text);
            int exit = 0;
            double referencia = 0;
           iteracio = 0;




            
            

            //maga a tesztfüggvény

            Func<double, double, double> fx2 = ((x, y) => -(4 * Math.Pow(x, 2) - 2.1 * Math.Pow(x, 4) + Math.Pow(x, 6) / 3 + x * y - 4 * Math.Pow(y, 2) + 4 * Math.Pow(y, 4)));
           // Func<double, double, double> fx2 = ((x, y) => -100 * Math.Pow(y - (Math.Pow(x, 2)), 2) + Math.Pow(1 - x, 2));

            //első megoldás generálása


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


            
            

            while (homerseklet > minhom)
            {
               
                for (int i = 0; i < time; i++)
                {
                    uj = regi;

                    referencia = regi.Fx;


                    //Módosulás

                    //Első paraméter
                    if (r.Next(2) == 1)
                    {
                        //növelem
                        uj.X1 = uj.X1 + (max * r.NextDouble());
                    }
                    else
                    {
                        //növelem
                        uj.X1 = uj.X1 - (max * r.NextDouble());
                    }
                    //Második paraméter
                    if (r.Next(2) == 1)
                    {
                        //növelem
                        uj.X2 = uj.X2 + (max * r.NextDouble());
                    }
                    else
                    {
                        //növelem
                        uj.X2 = uj.X2 - (max * r.NextDouble());
                    }

                    uj.Fx = fx2(uj.X1, uj.X2);

                    //Kiszámoljuk a differenciát az új és a régi között.

                    delta = uj.Fx - regi.Fx;
                    

                    if (delta > 0)
                    {
                        //Közelebb kerültünk a maximumhoz.
                        regi = uj;

                    }                    
                    else if (Math.Exp(-delta / homerseklet) < r.NextDouble())
                    {
                        regi = uj;
                    }


                     double diff = Math.Abs(referencia - regi.Fx);
                     if ( diff < 0.01)
                     {
                         exit++;
                     }
                     else exit = 0;
                    
                     if (exit == 5000)
                     {
                         break;
                     }
                    

                  
                    iteracio++;

                
                }
                if (exit == 5000) break;

                
                



                
                homerseklet = homerseklet * lehules;
                time = time * lehules;
              }
             Vége = DateTime.Now;
             futásidő = Vége - Start;




            listBox1.Items.Add((iteracio).ToString() + " lépésből az optimum: X1= " + Math.Round(regi.X1, 4) + ", X2= " + Math.Round(regi.X2, 4) + " F(x): " + Math.Round(regi.Fx, 5) + ". " + (Math.Round((regi.Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal." + "Futásidő: "+ futásidő.Milliseconds + " milliszekundum");
        
    }

        private void button2_Click(object sender, EventArgs e)
        {

            var lst = new List<double> { 1,5,7,10,12,15,20,100,1000,10000 };
            var pontos = new List<double>();
            var lepes = new List<int>();
            var ido = new List<double>();
            using (StreamWriter sw = new StreamWriter("Output.csv"))
            {
                for (int i = 0; i < lst.Count; i++)
                {

                    textBox2.Text = lst[i].ToString();
                    for (int j = 0; j < 100; j++)
                    {
                        button1.PerformClick();
                        pontos.Add(Math.Round((regi.Fx / vegeredmeny * 100), 5));
                        ido.Add(futásidő.Milliseconds);
                        lepes.Add(iteracio);

                    }
                    sw.WriteLine(lst[i] + ";" + lepes.Average() + ";" + pontos.Average() + ";" + pontos.Median() + ";" + Math.Sqrt(pontos.Variance()) + ";" + ido.Average());
                    pontos.Clear();
                    ido.Clear();
                    lepes.Clear();

                }
            }

            /* using (StreamWriter sw = new StreamWriter("Output.csv"))
            {
                sw.WriteLine("Iteracio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum)");
                for (int i = 0; i < 30; i++)
                {
                    button1.PerformClick();
                    sw.WriteLine(((iteracio).ToString() + ";" + Math.Round(regi.X1, 4) + ";" + Math.Round(regi.X2, 4) + ";" + Math.Round(regi.Fx, 5) + ";" + (Math.Round((regi.Fx / vegeredmeny * 100), 5)) + ";" + futásidő.Milliseconds));
                    
                }
            }*/


        }
    }
}
