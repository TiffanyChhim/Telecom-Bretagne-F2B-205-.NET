
// TP2 Threads - Maxime MONIN et Tiffany CHHIM

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using BallonPremierLibrary;
using System.Windows.Forms;

namespace TP2_Threads
{
    // Cette application permet le lancement, la suppression ainsi que la suspension
    // de threads de type 'Ballon' et 'Premier'.
    public partial class MainWindow : Window
    {
        private List<Thread> listThreads = new List<Thread>();
        private int nbBallon = 0;
        private int nbPremier = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Déclenche la création d'un thread 'Ballon'.
        private void Ballon_Click(object sender, RoutedEventArgs e)
        {
            Ballon b = new Ballon();
            b.FormClosed += (s, ev) => App.Current.Dispatcher.BeginInvoke(
                new Action(() => Ballon_CloseEvent(s, ev)));
            
            Thread t = new Thread(() => b.Go());
            b.threadId = t.ManagedThreadId;
            t.Name = "Ballon";
            t.Start();
            AddThread(t);
        }

        // Déclenche la création d'un thread 'Premier'.
        private void Premier_Click(object sender, RoutedEventArgs e)
        {
            NombrePremier np = new NombrePremier();
            np.FormClosed += (s, ev) => App.Current.Dispatcher.BeginInvoke(
                new Action(() => Premier_CloseEvent(s, ev)));

            Thread t = new Thread(() => np.ThreadFunction());
            np.threadId = t.ManagedThreadId;
            t.Name = "Premier";
            t.Start();
            AddThread(t);          
        }

        // Suspend ou reprend l'exécution des threads.
        // Empêche toute autre action si les threads sont en pause.
        private void PauseResume_Click(object sender, RoutedEventArgs e)
        {
            // Pause
            if (boutonPause.IsChecked)
            {
                boutonLancerThread.IsEnabled = false;
                boutonSupprimer.IsEnabled = false;
                boutonQuitter.IsEnabled = false;
                foreach (Thread t in listThreads)
                    t.Suspend();
            }

            // Reprise
            else {
                boutonLancerThread.IsEnabled = true;
                boutonSupprimer.IsEnabled = true;
                boutonQuitter.IsEnabled = true;
                foreach (Thread t in listThreads)
                    t.Resume();
            }
        }

        // Déclenche la suppression du dernier thread créé.
        private void DeleteLastThread_Click(object sender, RoutedEventArgs e)
        {
            Thread t = listThreads[listThreads.Count - 1];
            RemoveThread(t.Name, listThreads.Count - 1);
            t.Abort();
        }

        // Déclenche la suppression du dernier thread 'Ballon' créé.
        private void DeleteLastBallon_Click(object sender, RoutedEventArgs e)
        {
            // Recherche du dernier 'Ballon' créé
            int i;
            for (i = listThreads.Count - 1; i >= 0; i--)
            {
                if (listThreads[i].Name == "Ballon")
                    break;
            }

            // Suppression du thread
            Thread t = listThreads[i];
            RemoveThread(t.Name, i);
            t.Abort();
        }

        // Déclenche la suppression du dernier thread 'Premier' créé.
        private void DeleteLastPremier_Click(object sender, RoutedEventArgs e)
        {
            // Recherche du dernier 'Premier' créé
            int i;
            for (i = listThreads.Count - 1; i >= 0; i--)
            {
                if (listThreads[i].Name == "Premier")
                    break;
            }

            // Suppression du thread
            Thread t = listThreads[i];
            RemoveThread(t.Name, i);
            t.Abort();
        }

        // Déclenche la suppression de tous les threads en cours.
        private void DeleteAllThreads_Click(object sender, RoutedEventArgs e)
        {
            for (int i = listThreads.Count - 1; i >= 0; i--)
            {
                Thread t = listThreads[i];
                RemoveThread(t.Name, i);
                t.Abort();
            }
        }

        // Supprime tous les threads en cours, puis quitte l'application.
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllThreads_Click(sender, e);
            System.Windows.MessageBox.Show("Au revoir !");
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Quit_Click(sender, null);
        }

        // Récupération de l'ID du thread afin de le supprimer de la liste d'affichage.
        private void Ballon_CloseEvent(object sender, EventArgs e)
        {
            Ballon b = (Ballon)sender;
            FindDeadThread(b.threadId);
        }

        // Récupération de l'ID du thread afin de le supprimer de la liste d'affichage.
        private void Premier_CloseEvent(object sender, EventArgs e)
        {
            NombrePremier np = (NombrePremier)sender;
            FindDeadThread(np.threadId);
        }

        // Ajoute le thread à la liste des threads et met à jour l'affichage des threads.
        private void AddThread(Thread t)
        {
            switch (t.Name)
            {
                case "Ballon":
                    listThreads.Add(t);
                    nbBallon++;
                    nombreThreadsBox.Items[0] = nbBallon + " thread(s) 'Ballon'";
                    listeThreadsBox.Items.Add("Thread 'Ballon' d'ID " + t.ManagedThreadId);

                    //(Ré)activation du bouton 'Supprimer dernier 'Ballon''
                    boutonSupprDernierBallon.IsEnabled = true;
                    break;

                case "Premier":
                    listThreads.Add(t);
                    nbPremier++;
                    nombreThreadsBox.Items[1] = nbPremier + " thread(s) 'Premier'";
                    listeThreadsBox.Items.Add("Thread 'Premier' d'ID " + t.ManagedThreadId);

                    //(Ré)activation du bouton 'Supprimer dernier 'Premier''
                    boutonSupprDernierPremier.IsEnabled = true;
                    break;

                default:
                    Console.WriteLine("Thread non supporté");
                    break;
            }

            // (Ré)activation des boutons de suppression de tous les threads et du dernier thread
            boutonSupprDernierThread.IsEnabled = true;
            boutonSupprTousThreads.IsEnabled = true;
        }

        // Retire le thread de la liste des threads et met à jour l'affichage des threads.
        private void RemoveThread(String threadType, int index)
        {
            switch (threadType)
            {
                case "Ballon":
                    listThreads.RemoveAt(index);
                    nbBallon--;
                    nombreThreadsBox.Items[0] = nbBallon + " thread(s) 'Ballon'";
                    listeThreadsBox.Items.Remove(listeThreadsBox.Items[index]);

                    // Désactivation du bouton 'Supprimer dernier 'Ballon''
                    if (nbBallon == 0)
                        boutonSupprDernierBallon.IsEnabled = false;
                    break;

                case "Premier":
                    listThreads.RemoveAt(index);
                    nbPremier--;
                    nombreThreadsBox.Items[1] = nbPremier + " thread(s) 'Premier'";
                    listeThreadsBox.Items.Remove(listeThreadsBox.Items[index]);

                    // Désactivation du bouton 'Supprimer dernier 'Premier''
                    if (nbPremier == 0)
                        boutonSupprDernierPremier.IsEnabled = false;
                    break;

                default:
                    Console.WriteLine("Thread non supporté");
                    break;
            }

            // Désactivation des boutons de suppression de tous les threads et du dernier thread
            if (listThreads.Count == 0)
            {
                boutonSupprDernierThread.IsEnabled = false;
                boutonSupprTousThreads.IsEnabled = false;
            }
        }

        // Cherche le thread mort selon son ID pour le retirer de la liste.
        private void FindDeadThread(int threadId)
        {
            for (int i = 0; i < listThreads.Count; i++)
            {
                Console.WriteLine(threadId.ToString());
                if (listThreads[i].ManagedThreadId == threadId)
                {                 
                    RemoveThread(listThreads[i].Name, i);
                    break;
                }
            }
        }
    }
}
