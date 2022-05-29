using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            double referencia = 0;

            double max = 10; // A maximum érték  abs értéke. (negatívan is rá fogjuk mérni)
            double homerseklet = Convert.ToDouble(textBox1.Text);
            double delta = 0;
            double lehules = 0.9999;
            double minhom = 0.00001; // Itt all le
            double time = Convert.ToDouble(textBox2.Text); // ennyi ideig marad a homersekleten
           
            List<double> atlagok = new List<double>();
            List<double> populacio = new List<double>();
            int exit = 0;


            double vegeredmeny = 1.0316285;

            Random r = new Random();

            //maga a tesztfüggvény

            Func<double, double, double> fx2 = ((x, y) => -(4 * Math.Pow(x, 2) - 2.1 * Math.Pow(x, 4) + Math.Pow(x, 6) / 3 + x * y - 4 * Math.Pow(y, 2) + 4 * Math.Pow(y, 4)));


            //első megoldás generálása

            Egyed regi = new Egyed();
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


            Egyed uj = new Egyed();
            int iteracio = 0;
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

                    if ((iteracio + 1) % 1000 != 0)
                   {
                       populacio.Add(referencia);
                   }
                   else
                   {
                       atlagok.Add(populacio.Average());
                       populacio.Clear();

                        int hossz = atlagok.Count;
                        if (hossz>1)
                        {
                            
                            if (atlagok[hossz-1]-atlagok[hossz-2]<0.1)
                            {
                                exit++;
                            }
                            else
                            {
                                exit = 0;
                            }
                        }
                        

                   }
                    iteracio++;
                    if (exit == 10)
                    {
                        break;
                    }
                }

                if (exit == 10)
                {
                    break;
                }
                
                homerseklet = homerseklet * lehules;
                time = time * lehules;
            }


            listBox1.Items.Add((iteracio).ToString() + " lépésből az optimum: X1= " + Math.Round(regi.X1, 4) + ", X2= " + Math.Round(regi.X2, 4) + " F(x): " + Math.Round(regi.Fx, 5) + ". " + (Math.Round((regi.Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal.");
        
    }
    }
}
