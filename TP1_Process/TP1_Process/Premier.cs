using System;
using System.Threading;

namespace ConsoleANbPremier
{
    class NombrePremier
    {
        [STAThread]
        static void Main(string[] args)
        {
            // L'instence de ThreadStart demande un delegate en paramètre ce qui est plus
            // ou moins (mais pas vraiment) l'équivalent d'un pointeur de fonction
           
            Console.SetWindowSize(20, 30);  // la taille de la fenêtre

            Thread t = new Thread(new ThreadStart(ThreadFunction));

            t.Start();
        }

        private static void ThreadFunction()
        {
            for (int p = 1; p < 1000000; p++)
            {
                int i = 2;
                while ((p % i) != 0 && i < p)
                {
                    i++;
                }
                if (i == p)
                    Console.WriteLine(p.ToString());
                Thread.Sleep(50);

            }
        }
    }
}
