using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using MathNet.Numerics.Statistics;

namespace genetikus
{
    public partial class Form1 : Form
    {

        int popsize = 10;
        int maxlepes = 100;
        double elitek = 0.1;
        double mutacio = 0.25;
        int iteracio = 0;
        double vegeredmeny = 1.0316285;
        int csoportsize = 0;
        Egyed [] populacio ;
        Egyed uj;
        Egyed[] ujpopulacio;
        TimeSpan futásidő = DateTime.Now - DateTime.Now;
        DateTime Vége = DateTime.Now;
        List<double> eredmeny = new List<double>();
        int feltetel;
        Egyed legjobb = new Egyed();

        int max = 10; //abs értéke
        
        //Func<double, double, double> fx2 = ((x, y) => (-Math.Pow(x, 2) - Math.Pow(y, 2) + 3 * x + 3 * y));

        Func<double, double, double> fx2 = ((x, y) => -(4 * Math.Pow(x, 2) - 2.1 * Math.Pow(x, 4) + Math.Pow(x, 6) / 3 + x* y - 4 * Math.Pow(y, 2) + 4 * Math.Pow(y, 4)));
        //Func<double, double, double> fx2 = ((x, y) => -100 * Math.Pow(y - (Math.Pow(x, 2)), 2) + Math.Pow(1 - x, 2));


        public Form1()
        {
            InitializeComponent();
             feltetel = Convert.ToInt32(textBox6.Text);


        }
        public struct Egyed
        {
            public double X1;
            public double X2;
            public double Fx;

        }


        Random r = new Random();

        private void elitista_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DateTime Start = DateTime.Now;
            popsize = Convert.ToInt32(textBox1.Text);
            maxlepes = Convert.ToInt32(textBox2.Text);
            elitek = Convert.ToDouble(textBox3.Text) / 100;
            mutacio = Convert.ToDouble(textBox4.Text) / 100;
           
            populacio = new Egyed[popsize];



            //kezdeti populáció betöltés

            for (int i = 0; i < popsize; i++)
            {
                uj = new Egyed();
                uj.X1 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X1 = uj.X1 * -1;
                }

                uj.X2 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X2 = uj.X2 * -1;
                }

                // Fitness számítása - ez maga az Fx lesz. (szerintem)
                uj.Fx = fx2(uj.X1, uj.X2);
                populacio[i] = uj;

            }

            //sorbarendezés fitness (Fx) szerint

            for (iteracio = 0; iteracio < maxlepes; iteracio++)
            {

                var rendezett = (from x in populacio orderby x.Fx descending select x).ToArray();
                populacio = rendezett;
                listBox1.Items.Add("A " + (iteracio + 1) + ". populációban a legjobb egyed - X1: " + Math.Round( populacio[0].X1,5) + " X2: " + Math.Round(populacio[0].X2,5) + ", F(x) fitness: " + Math.Round(populacio[0].Fx,5) + ". " + (Math.Round((populacio[0].Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal.");
                legjobb = populacio[0];
                //kilépési feltétel
                if (eredmeny.Count < feltetel - 1)
                {
                    double popatlag = populacio.Average(x => x.Fx);
                    eredmeny.Add(popatlag);
                }
                else
                {
                    double mostani = populacio.Average(x => x.Fx);
                    double atlag = eredmeny.Average();
                    if (Math.Abs(mostani - atlag) < 0.01)
                    {
                        for (int p = 0; p < feltetel - 1; p++)
                        {
                            legjobb = populacio[0];
                            listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                        }
                        break;
                    }
                    eredmeny.Add(populacio.Average(x => x.Fx));
                    eredmeny.Reverse();
                    eredmeny.RemoveAt(feltetel - 1);
                    eredmeny.Reverse();
                }


                // Elitek kiválogatása - Az eddigi populáció legjobb bizonyos hányadát az új generációba beveszem
                ujpopulacio = new Egyed[popsize];
                int elitekszama = (int)(elitek * popsize);

                for (int i = 0; i < elitekszama; i++)
                {
                    ujpopulacio[i] = populacio[i];
                }

                //a többi helyre szelekció, egypontos keresztezéssel -> súlyozott átlagolással

                for (int i = elitekszama; i < popsize; i++)
                {
                    // mivel a fixpontos számábrázolás körülményes, ezért az apából és az anyából random súlyokkal állítom elő a gyereket.

                    Egyed gyerek = new Egyed();

                    double sulyX1 = r.NextDouble(); //0 és 1 közötti súly
                    int anyaX1 = r.Next(popsize);
                    int apaX1 = r.Next(popsize);

                    gyerek.X1 = (populacio[anyaX1].X1 * sulyX1) + (populacio[apaX1].X1 * (1 - sulyX1));

                    double sulyX2 = r.NextDouble();
                    int anyaX2 = r.Next(popsize);
                    int apaX2 = r.Next(popsize);

                    gyerek.X2 = (populacio[anyaX2].X2 * sulyX2) + (populacio[apaX2].X2 * (1 - sulyX2));
                    gyerek.Fx = fx2(gyerek.X1, gyerek.X2);
                    ujpopulacio[i] = gyerek;

                    /* 
                    Bites próbálkozás -> körülményes és erőforrásigényes lenne.

                     int probahossz = BitConverter.DoubleToInt64Bits(populacio[0].X2).ToString().Length;


                    int keresztpont = r.Next(probahossz);


                    string apafele = BitConverter.DoubleToInt64Bits(populacio[apa].X2).ToString().Substring(0,keresztpont);
                    string anyafele = BitConverter.DoubleToInt64Bits(populacio[anya].X2).ToString().Substring(keresztpont);

                     string eredmeny = apafele + anyafele;

                    double eredmeny2 = BitConverter.Int64BitsToDouble(Convert.ToInt64(eredmeny));
                     MessageBox.Show(eredmeny2.ToString());*/

                }

                //mutáció

                for (int i = 0; i < popsize; i++)
                {

                    //mutációnál csak az egyik vagy mindkét paramétert mutáljam? Mekkora mértékben mutáljam? Most teljesen új szám lesz. Laci 3 betűt módosított.
                    
                    if (r.NextDouble()<mutacio)
                    {
                        if (r.Next(2) == 0)
                        {
                            ujpopulacio[i].X1 = r.NextDouble();
                        }
                        else
                        {
                            ujpopulacio[i].X2 = r.NextDouble();
                        }
                        ujpopulacio[i].Fx = fx2(ujpopulacio[i].X1, ujpopulacio[i].X2);
                    }
                }

                //új generáció születése

                populacio = ujpopulacio;
                //fitnesst mindig számolok az új értékek létrejöttekor.

            }

            Vége = DateTime.Now;
            futásidő = Vége - Start;
        }

       

        private void roulette_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DateTime Start = DateTime.Now;
            popsize = Convert.ToInt32(textBox1.Text);
            maxlepes = Convert.ToInt32(textBox2.Text);
            elitek = Convert.ToDouble(textBox3.Text) / 100;
            mutacio = Convert.ToDouble(textBox4.Text) / 100;


            Egyed[] populacio = new Egyed[popsize];


            //kezdeti populáció betöltés

            for (int i = 0; i < popsize; i++)
            {
                Egyed uj = new Egyed();
                uj.X1 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X1 = uj.X1 * -1;
                }

                uj.X2 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X2 = uj.X2 * -1;
                }

                // Fitness számítása - ez maga az Fx lesz. (szerintem)
                uj.Fx = fx2(uj.X1, uj.X2);
                populacio[i] = uj;

            }

            //sorbarendezés fitness (Fx) szerint

            for (iteracio = 0; iteracio < maxlepes; iteracio++)
            {

                var rendezett = (from x in populacio orderby x.Fx descending select x).ToArray();



                legjobb = populacio[0];
                listBox1.Items.Add("A " + (iteracio + 1) + ". populációban a legjobb egyed - X1: " + Math.Round(rendezett[0].X1, 5) + " X2: " + Math.Round(rendezett[0].X2, 5) + ", F(x) fitness: " + Math.Round(rendezett[0].Fx, 5) + ". " + (Math.Round((rendezett[0].Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal.");

                //kilépési feltétel
                
                if (eredmeny.Count < feltetel - 1)
                {
                    double popatlag = populacio.Average(x => x.Fx);
                    eredmeny.Add(popatlag);
                }
                else
                {
                    double mostani = populacio.Average(x => x.Fx);
                    double atlageredmeny = eredmeny.Average();
                    if (Math.Abs(mostani - atlageredmeny) < 0.01)
                    {
                        for (int p = 0; p < feltetel - 1; p++)
                        {
                            legjobb = populacio[0];
                            listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                        }
                        break;
                    }
                    eredmeny.Add(populacio.Average(x => x.Fx));
                    eredmeny.Reverse();
                    eredmeny.RemoveAt(feltetel - 1);
                    eredmeny.Reverse();
                }

                // roulette
                Egyed[] ujpopulacio = new Egyed[popsize];
                double atlag =populacio.Average(x=>x.Fx);

                //Ennél az átlagnál a jobbakat beválogatom.

                var jok = (from x in populacio where (x.Fx>atlag) select x).ToArray();
                for (int i = 0; i < jok.Length; i++)
                {
                    ujpopulacio[i] = jok[i];
                }


                for (int i = jok.Length; i < popsize; i++)
                {
                    // mivel a fixpontos számábrázolás körülményes, ezért az apából és az anyából random súlyokkal állítom elő a gyereket.

                    Egyed gyerek = new Egyed();

                    double sulyX1 = r.NextDouble(); //0 és 1 közötti súly
                    int anyaX1 = r.Next(popsize);
                    int apaX1 = r.Next(popsize);

                    gyerek.X1 = (populacio[anyaX1].X1 * sulyX1) + (populacio[apaX1].X1 * (1 - sulyX1));

                    double sulyX2 = r.NextDouble();
                    int anyaX2 = r.Next(popsize);
                    int apaX2 = r.Next(popsize);

                    gyerek.X2 = (populacio[anyaX2].X2 * sulyX2) + (populacio[apaX2].X2 * (1 - sulyX2));
                    gyerek.Fx = fx2(gyerek.X1, gyerek.X2);
                    ujpopulacio[i] = gyerek;

                }


                //mutáció

                for (int i = 0; i < popsize; i++)
                {

                    //mutációnál csak az egyik vagy mindkét paramétert mutáljam? Mekkora mértékben mutáljam? Most teljesen új szám lesz. Laci 3 betűt módosított.

                    if (r.NextDouble() < mutacio)
                    {
                        if (r.Next(2) == 0)
                        {
                            ujpopulacio[i].X1 = r.NextDouble();
                        }
                        else
                        {
                            ujpopulacio[i].X2 = r.NextDouble();
                        }
                        ujpopulacio[i].Fx = fx2(ujpopulacio[i].X1, ujpopulacio[i].X2);
                    }
                }

                //új generáció születése

                populacio = ujpopulacio;
                //fitnesst mindig számolok az új értékek létrejöttekor.


            }
            Vége = DateTime.Now;
            futásidő = Vége - Start;
        }
              

        private void verseny(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DateTime Start = DateTime.Now;
            popsize = Convert.ToInt32(textBox1.Text);
            maxlepes = Convert.ToInt32(textBox2.Text);
           
            mutacio = Convert.ToDouble(textBox4.Text) / 100;
            csoportsize = Convert.ToInt32(textBox4.Text);


            Egyed[] populacio = new Egyed[popsize];


            //kezdeti populáció betöltés

            for (int i = 0; i < popsize; i++)
            {
                Egyed uj = new Egyed();
                uj.X1 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X1 = uj.X1 * -1;
                }

                uj.X2 = r.NextDouble() * max;
                if (r.Next(2) == 0)
                {
                    uj.X2 = uj.X2 * -1;
                }

                // Fitness számítása - ez maga az Fx lesz. (szerintem)
                uj.Fx = fx2(uj.X1, uj.X2);
                populacio[i] = uj;

            }

            //sorbarendezés fitness (Fx) szerint

            for (iteracio = 0; iteracio < maxlepes; iteracio++)
            {

                var rendezett = (from x in populacio orderby x.Fx descending select x).ToArray();


                var optimum = rendezett[0];
                legjobb = populacio[0];
                listBox1.Items.Add("A " + (iteracio + 1) + ". populációban a legjobb egyed - X1: " + Math.Round(optimum.X1, 5) + " X2: " + Math.Round(optimum.X2, 5) + ", F(x) fitness: " + Math.Round(optimum.Fx, 5) + ". " + ((Math.Round((optimum.Fx / vegeredmeny * 100), 5)) + "%-os az egyezőség az optimummal."));

                //kilépési feltétel
                if (eredmeny.Count < feltetel-1)
                {
                    double popatlag = populacio.Average(x => x.Fx);
                    eredmeny.Add(popatlag);
                }
                else
                {
                    double mostani = populacio.Average(x => x.Fx);
                    double atlag = eredmeny.Average();
                    if (Math.Abs(mostani - atlag)<0.01)
                    {
                        for (int p = 0; p < feltetel-1; p++)
                        {
                            legjobb = populacio[0];
                           listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                       }
                        break;
                    }
                    eredmeny.Add(populacio.Average(x => x.Fx));
                    eredmeny.Reverse();
                    eredmeny.RemoveAt(feltetel-1);
                    eredmeny.Reverse();
                }

                // csoport
                Egyed[] ujpopulacio = new Egyed[popsize];

                int counter = 0;
                int eleje = 0;
                int vege = csoportsize;
                double max = populacio[eleje].Fx;
                int maxhely = 0;
                while (vege < popsize)
                {
                    //egy csoport versenyzik, legjobb továbbjut
                    for (int i = eleje; i < vege; i++)
                    {
                        if (populacio[i].Fx > max)
                        {
                            max = populacio[i].Fx;
                            maxhely = i;
                        }
                    }

                    ujpopulacio[counter] = populacio[maxhely];

                    counter++;

                    eleje += csoportsize;
                    vege += csoportsize;
                    max = populacio[eleje].Fx;
                    maxhely = 0;
                }



                for (int i = counter; i < popsize; i++)
                {
                    // mivel a fixpontos számábrázolás körülményes, ezért az apából és az anyából random súlyokkal állítom elő a gyereket.

                    Egyed gyerek = new Egyed();

                    double sulyX1 = r.NextDouble(); //0 és 1 közötti súly
                    int anyaX1 = r.Next(popsize);
                    int apaX1 = r.Next(popsize);

                    gyerek.X1 = (populacio[anyaX1].X1 * sulyX1) + (populacio[apaX1].X1 * (1 - sulyX1));

                    double sulyX2 = r.NextDouble();
                    int anyaX2 = r.Next(popsize);
                    int apaX2 = r.Next(popsize);

                    gyerek.X2 = (populacio[anyaX2].X2 * sulyX2) + (populacio[apaX2].X2 * (1 - sulyX2));
                    gyerek.Fx = fx2(gyerek.X1, gyerek.X2);
                    ujpopulacio[i] = gyerek;

                }


                //mutáció

                for (int i = 0; i < popsize; i++)
                {

                    //mutációnál csak az egyik vagy mindkét paramétert mutáljam? Mekkora mértékben mutáljam? Most teljesen új szám lesz. Laci 3 betűt módosított.

                    if (r.NextDouble() < mutacio)
                    {
                        if (r.Next(2) == 0)
                        {
                            ujpopulacio[i].X1 = r.NextDouble();
                        }
                        else
                        {
                            ujpopulacio[i].X2 = r.NextDouble();
                        }
                        ujpopulacio[i].Fx = fx2(ujpopulacio[i].X1, ujpopulacio[i].X2);
                    }
                }

                //új generáció születése

                populacio = ujpopulacio;
                //fitnesst mindig számolok az új értékek létrejöttekor.


            }
            Vége = DateTime.Now;
            futásidő = Vége - Start;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /* using (StreamWriter sw = new StreamWriter("Output_elitista.csv"))
             {
                 sw.WriteLine("Populacio merete:;"+ popsize + ";Maximalis generacio:; "+ maxlepes + "; Elitek aranya:;"+elitek+";Mutacio valoszinusege:;"+mutacio+";Kilepesi feltetel:;"+feltetel);
                 sw.WriteLine("Populacio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum)");
                 for (int i = 0; i < Convert.ToInt32(textBox7.Text); i++)
                 {
                     elitista.PerformClick();
                     sw.WriteLine(((iteracio).ToString() + ";" + Math.Round(legjobb.X1, 5) + ";" + Math.Round(legjobb.X2, 5) + ";" + Math.Round(legjobb.Fx, 5) + ";" + (Math.Round((legjobb.Fx / vegeredmeny * 100), 5)) + ";" + futásidő.Milliseconds));

                 }
             }*/

            var lst = new List<double> { 25,50,75,100,125,150,200,300,400,500,1000,1500,2000,2500,3000 };
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
                        elitista.PerformClick();
                        pontos.Add(Math.Round((legjobb.Fx / vegeredmeny * 100), 5));
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

        private void button3_Click(object sender, EventArgs e)
        {
            /* using (StreamWriter sw = new StreamWriter("Output_roulette.csv"))
             {
                 sw.WriteLine("Populacio merete:;" + popsize + ";Maximalis generacio:; " + maxlepes + "; Elitek aranya:;" + elitek + ";Mutacio valoszinusege:;" + mutacio + ";Kilepesi feltetel:;" + feltetel);
                 sw.WriteLine("Populacio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum)");
                 for (int i = 0; i < Convert.ToInt32(textBox7.Text); i++)
                 {
                     roulette.PerformClick();
                     sw.WriteLine(((iteracio).ToString() + ";" + Math.Round(legjobb.X1, 5) + ";" + Math.Round(legjobb.X2, 5) + ";" + Math.Round(legjobb.Fx, 5) + ";" + (Math.Round((legjobb.Fx / vegeredmeny * 100), 5)) + ";" + futásidő.Milliseconds));

                 }
             }*/

            var lst = new List<double> { 25, 50, 75, 100, 125, 150, 200, 300, 400, 500, 1000, 1500, 2000, 2500, 3000 };
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
                        elitista.PerformClick();
                        pontos.Add(Math.Round((legjobb.Fx / vegeredmeny * 100), 5));
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

        private void button4_Click(object sender, EventArgs e)
        {
            /*using (StreamWriter sw = new StreamWriter("Output_verseny.csv"))
            {
                sw.WriteLine("Populacio merete:;" + popsize + ";Maximalis generacio:; " + maxlepes + "; Elitek aranya:;" + elitek + ";Mutacio valoszinusege:;" + mutacio + ";Kilepesi feltetel:;" + feltetel+";Csoport nagysaga:;"+csoportsize);
                sw.WriteLine("Populacio ; X1 ; X2 ; F(x) ; pontos egyezoseg; futas (milliszekundum)");
                for (int i = 0; i < Convert.ToInt32(textBox7.Text); i++)
                {
                    roulette.PerformClick();
                    sw.WriteLine(((iteracio).ToString() + ";" + Math.Round(legjobb.X1, 5) + ";" + Math.Round(legjobb.X2, 5) + ";" + Math.Round(legjobb.Fx, 5) + ";" + (Math.Round((legjobb.Fx / vegeredmeny * 100), 5)) + ";" + futásidő.Milliseconds));

                }
            }*/

            var lst = new List<double> { 25, 50, 75, 100, 125, 150, 200, 300, 400, 500, 1000, 1500, 2000, 2500, 3000 };
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
                        elitista.PerformClick();
                        pontos.Add(Math.Round((legjobb.Fx / vegeredmeny * 100), 5));
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
