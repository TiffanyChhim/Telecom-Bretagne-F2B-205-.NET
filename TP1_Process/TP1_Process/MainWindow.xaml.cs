
// TP1 Processus - Maxime MONIN et Tiffany CHHIM

using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TP1_Process
{
    // Cette application permet le lancement et la suppression de processus 
    // de type 'Ballon' et 'Premier' dans la limite d'un nombre fixé de processus.
    // La liste des processus en cours peut être affichée ou masquée.
    public partial class MainWindow : Window
    {
        private List<Process> listProcesses = new List<Process>();
        private int nbBallon = 0;
        private int nbPremier = 0;
        private int maxNbProcesses = 5;

        public MainWindow()
        {
            InitializeComponent();
            CheckAliveProcesses();
        }

        // Déclenche la création d'un processus 'Ballon' s'il y en a moins de maxNbProcesses.
        private void Ballon_Click(object sender, RoutedEventArgs e)
        {
            if (listProcesses.Count < maxNbProcesses)
            {                
                Process p = new Process();
                p.StartInfo.FileName = "Ballon.exe";
                p.Start();
                AddProcess(p);
            }
        }

        // Déclenche la création d'un processus 'Premier' s'il y en a moins de maxNbProcesses.
        private void Premier_Click(object sender, RoutedEventArgs e)
        {
            if (listProcesses.Count < maxNbProcesses)
            {
                Process p = new Process();
                p.StartInfo.FileName = "Premier.exe";
                p.Start();
                AddProcess(p);
            }
        }

        // Rend visible ou invisible l'affichage des processus en cours.
        private void ProcessusEnCours_Click(object sender, RoutedEventArgs e)
        {
            nombreProcBox.Visibility = boutonAfficheProcessus.IsChecked ? Visibility.Visible : Visibility.Hidden;
            listeProcBox.Visibility = boutonAfficheProcessus.IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        // Déclenche la suppression du dernier processus créé.
        private void DeleteLastProcessus_Click(object sender, RoutedEventArgs e)
        {
            Process p = listProcesses[listProcesses.Count - 1];
            RemoveProcess(p.StartInfo.FileName, listProcesses.Count - 1);
            p.Kill();
        }

        // Déclenche la suppression du dernier processus 'Ballon' créé.
        private void DeleteLastBallon_Click(object sender, RoutedEventArgs e)
        {
            // Recherche du dernier 'Ballon' créé
            int i;
            for (i = listProcesses.Count - 1; i >= 0; i--)
            {
                if (listProcesses[i].StartInfo.FileName == "Ballon.exe")
                    break;
            }

            // Suppression du processus
            Process p = listProcesses[i];
            RemoveProcess(p.StartInfo.FileName, i);
            p.Kill();
        }

        // Déclenche la suppression du dernier processus 'Premier' créé.
        private void DeleteLastPremier_Click(object sender, RoutedEventArgs e)
        {
            // Recherche du dernier 'Premier' créé
            int i;
            for (i = listProcesses.Count - 1; i >= 0; i--)
            {
                if (listProcesses[i].StartInfo.FileName == "Premier.exe")
                    break;
            }

            // Suppression du processus
            Process p = listProcesses[i];
            RemoveProcess(p.StartInfo.FileName, i);
            p.Kill();
        }

        // Déclenche la suppression de tous les processus en cours.
        private void DeleteAllProcesses_Click(object sender, RoutedEventArgs e)
        {
            for (int i = listProcesses.Count - 1; i >= 0; i--)
            {
                Process p = listProcesses[i];
                RemoveProcess(p.StartInfo.FileName, i);
                p.Kill();
            }
        }

        // Supprime tous les processus en cours, puis quitte l'application.
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllProcesses_Click(sender, e);
            MessageBox.Show("Au revoir !");
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Quit_Click(sender, null);
        }

        // Ajoute le processus à la liste des processus, met à jour l'affichage des processus
        // et désactive le bouton 'Démarrer' si l'on atteint la limite du nombre de processus actifs.
        private void AddProcess(Process p)
        {
            switch (p.StartInfo.FileName)
            {
                case "Ballon.exe":
                    listProcesses.Add(p);
                    nbBallon++;
                    nombreProcBox.Items[0] = nbBallon + " processus 'Ballon'";
                    listeProcBox.Items.Add("Processus 'Ballon' de PID " + p.Id);

                    //(Ré)activation du bouton 'Supprimer dernier 'Ballon''
                    boutonSupprDernierBallon.IsEnabled = true;
                    break;

                case "Premier.exe":
                    listProcesses.Add(p);
                    nbPremier++;
                    nombreProcBox.Items[1] = nbPremier + " processus 'Premier'";
                    listeProcBox.Items.Add("Processus 'Premier' de PID " + p.Id);

                    //(Ré)activation du bouton 'Supprimer dernier 'Premier''
                    boutonSupprDernierPremier.IsEnabled = true;
                    break;

                default:
                    Console.WriteLine("Processus non supporté");
                    break;
            }

            // Désactivation du bouton 'Démarrer'
            if (listProcesses.Count >= maxNbProcesses)
                boutonDemarrer.IsEnabled = false;

            // (Ré)activation des boutons de suppression de tous les processus et du dernier processus
            boutonSupprDernierProc.IsEnabled = true;
            boutonSupprTousProc.IsEnabled = true;
        }

        // Retire le processus de la liste des processus, met à jour l'affichage des processus
        // et réactive le bouton 'Démarrer' s'il était désactivé (car on passe sous la limite).
        private void RemoveProcess(String procType, int index)
        {
            switch (procType)
            {
                case "Ballon.exe":
                    listProcesses.RemoveAt(index);
                    nbBallon--;
                    nombreProcBox.Items[0] = nbBallon + " processus 'Ballon'";
                    listeProcBox.Items.Remove(listeProcBox.Items[index]);

                    // Désactivation du bouton 'Supprimer dernier 'Ballon''
                    if (nbBallon == 0)
                        boutonSupprDernierBallon.IsEnabled = false;
                    break;

                case "Premier.exe":
                    listProcesses.RemoveAt(index);
                    nbPremier--;
                    nombreProcBox.Items[1] = nbPremier + " processus 'Premier'";
                    listeProcBox.Items.Remove(listeProcBox.Items[index]);

                    // Désactivation du bouton 'Supprimer dernier 'Premier''
                    if (nbPremier == 0)
                        boutonSupprDernierPremier.IsEnabled = false;
                    break;

                default:
                    Console.WriteLine("Processus non supporté");
                    break;
            }

            // Réactivation du bouton 'Démarrer'
            if (!boutonDemarrer.IsEnabled)
                boutonDemarrer.IsEnabled = true;

            // Désactivation des boutons de suppression de tous les processus et du dernier processus
            if (listProcesses.Count == 0)
            {
                boutonSupprDernierProc.IsEnabled = false;
                boutonSupprTousProc.IsEnabled = false;
            }
        }

        // Vérifie régulièrement quels sont les processus en cours pour détecter les fermetures par la croix rouge.
        private async void CheckAliveProcesses()
        {
            while (true)
            {
                // Délai entre deux vérifications
                await Task.Delay(1000);

                for (int i = 0; i < listProcesses.Count; i++)
                {
                    if (listProcesses[i].HasExited)
                        RemoveProcess(listProcesses[i].StartInfo.FileName, i);
                }
            }
        }
    }
}
