using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonia_kereses
{
   public class Metaheurisztika
    {
        //változók deklarálása
        private int Maxpop { get; set; }
        private int HMS { get; set; }
        private double HMCR { get; set; }
        private double  PAR { get; set; }
        private double BW { get; set; }
        private Func<double, double> egyvaltozos { get; set; }
        private Func<double, double, double> ketvaltozos { get; set; }
        private int pop = 0;
        private Random r = new Random();
        private int min = -10;
        private int max = 10;
        private double X_aktualis;
        private double X_uj;
        private double Y_aktualis;
        private double Y_uj;
        private TimeSpan futásidő { get; set; }


        public List<elem1> lista1 = new List<elem1>();
        public List<elem2> lista2 = new List<elem2>();
        public List<double> eredmeny = new List<double>();
        

        //Az elem egy összetett adatszerkezet. Az elemben fogom tárolni az X és az F(x) értékeket.
        public struct elem1
        {
            public double X;
            public double Fx;
        }
        public struct elem2
        {
            public double X;
            public double Y;
            public double Fx;
        }

        //Konstruktor
        public Metaheurisztika(int maxpop, int hMS, double hMCR, double pAR, double bW, Func<double, double> egyvaltozos)
        {
            Maxpop = maxpop;
            HMS = hMS;
            HMCR = hMCR;
            PAR = pAR;
            BW = bW;
            this.egyvaltozos = egyvaltozos;
            futásidő = DateTime.Now - DateTime.Now;
        }
        public Metaheurisztika(int maxpop, int hMS, double hMCR, double pAR, double bW, Func<double,double, double> ketvaltozos)
        {
            Maxpop = maxpop;
            HMS = hMS;
            HMCR = hMCR;
            PAR = pAR;
            BW = bW;
            this.ketvaltozos = ketvaltozos;
            futásidő = DateTime.Now - DateTime.Now;
        }

        //Mivel többször is előfordul vizsgálat, ezért függvényt írtam rá. 1db értéknél nem tudtam értelmezni az "eddigi legrosszabb" megfogalmazást. "Ha az új jobb az eddigi legrosszabbnál, akkor kiszorítja a jobb a legrosszabbat". 
        private void vizsgalat1()
        {
            if (egyvaltozos(X_aktualis) < egyvaltozos(X_uj)) 
            {
                X_aktualis = X_uj;
            }
        }

       

        //Itt kezdődik a számolás. A metódus egy elemekből (x és f(x)) álló listát fog visszaadni, aminek a végén a legjobb eredmény áll.
        public List<elem1> Szamol1()
        {
            X_aktualis = r.NextDouble() * max;
            if (r.Next(2)==0)
            {
                X_aktualis= X_aktualis* -1;
            }

            double referencia = 0;
            int exit = 0;

            for (int i = 0; i < Maxpop; i++)
            {

                X_uj = X_aktualis;
                referencia = egyvaltozos(X_aktualis);

                if (HMCR>r.NextDouble())
                {
                    if (PAR>r.NextDouble())
                    {
                        double rand = r.NextDouble();
                        switch (r.Next(2)) { 
                        case 0: X_uj += rand * BW; break;
                        case 1: X_uj -= rand * BW; break;
                        }

                        vizsgalat1();
                    }
                }
                else
                {
                    X_uj = r.NextDouble() * max - min;
                    vizsgalat1();
                }


                elem1 uj = new elem1();
                uj.X = X_aktualis;
                uj.Fx = egyvaltozos(X_aktualis);
                lista1.Add(uj);

                //kilépési feltétel

                double diff = Math.Abs(referencia - uj.Fx);
                if (diff < 0.001)
                {
                    exit++;
                }
                else exit = 0;

                if (exit == 20)
                {
                    break;
                }

            }
            

            return lista1;
        }

        //________________________________Kétismeretlenes egyenlet_______________________________

        public List<elem2> Szamol2()
        {
            List<double> atlagok = new List<double>();
            List<double> populacio = new List<double>();
            int exit = 0;
            double referencia = 0;

            elem2 aktualis = new elem2();
            elem2 uj = new elem2();
            aktualis.X = r.NextDouble() * max;
            if (r.Next(2) == 0)
            {
                aktualis.X = aktualis.X * -1;
            }

            aktualis.Y = r.NextDouble() * max;
            if (r.Next(2) == 0)
            {
                aktualis.Y = aktualis.Y * -1;
            }


            for (int i = 0; i < Maxpop; i++)
            { uj = aktualis;

                referencia = ketvaltozos(aktualis.X,aktualis.Y);

                if (HMCR> r.NextDouble())
                {
                    int modositando = r.Next(2);

                    if (PAR> r.NextDouble())
                    {
                        if (r.Next(2) ==0)
                        {
                            switch (modositando)
                            {
                                case 0: uj.X -= (BW * r.NextDouble()); break;
                                case 1: uj.Y -= (BW * r.NextDouble()); break;
                            }
                        }
                        else
                        {
                            switch (modositando)
                            {
                                case 0: uj.X += (BW * r.NextDouble()); break;
                                case 1: uj.Y += (BW * r.NextDouble()); break;
                            }
                        }

                        if (ketvaltozos(uj.X, uj.Y) > ketvaltozos(aktualis.X,aktualis.Y))
                        {
                            aktualis = uj;
                        }
                        
                    }

                }
                else
                {
                    
                    switch (r.Next(2))
                    {
                        case 0: uj.X = r.NextDouble() * max - min;  break;
                        case 1: uj.Y = r.NextDouble() * max - min;  break;
                    }
                    if (ketvaltozos(uj.X, uj.Y) > ketvaltozos(aktualis.X, aktualis.Y))
                    {
                        aktualis = uj;
                    }
                }

                elem2 uj_elem = new elem2();
                uj_elem.X = aktualis.X;
                uj_elem.Y = aktualis.Y;
                uj_elem.Fx = ketvaltozos(aktualis.X,aktualis.Y);
                lista2.Add(uj_elem);

                //kilépési feltétel

                if ((i + 1) % 50 != 0)
                {
                    populacio.Add(referencia);
                }
                else
                {
                    atlagok.Add(populacio.Average());
                    populacio.Clear();

                    int hossz = atlagok.Count;
                    if (hossz > 1)
                    {

                        if (atlagok[hossz - 1] - atlagok[hossz - 2] < 0.1)
                        {
                            exit++;
                        }
                        else
                        {
                            exit = 0;
                        }
                    }


                }
                if (exit == 10)
                {
                    break;
                }


            }

                return lista2;
        }






        }
}
