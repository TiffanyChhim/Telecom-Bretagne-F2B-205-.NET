using System;
using System.Threading;

namespace BallonPremierLibrary
{
    public class NombrePremier : System.Windows.Forms.Form
    {
        // Stockage de l'ID du thread
        public int threadId;

        public void ThreadFunction()
        {
            Console.SetWindowSize(20, 30);

            for (int p = 1; p < 1000000; p++)
            {
                int i = 2;
                while ((p % i) != 0 && i < p)
                {
                    i++;
                }
                if (i == p)
                    Console.WriteLine("thread(" + Thread.CurrentThread.ManagedThreadId + ") = " + p.ToString());
                Thread.Sleep(50);
            }
        }
    }
}
