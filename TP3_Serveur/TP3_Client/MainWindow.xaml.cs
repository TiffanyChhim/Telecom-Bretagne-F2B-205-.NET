
// TP3 Chat - Maxime MONIN et Tiffany CHHIM

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace TP3_Client
{
    // Le client peut se connecter au serveur afin d'envoyer et recevoir des messages avec un pseudo choisi.
    public partial class MainWindow : Window
    {
        private RemotingInterface.IRemoteChain RemoteInterface;
        private Thread receiveThread;
        private string pseudo;
        private string lastMessage;

        public MainWindow()
        {
            InitializeComponent();          
        }

        // Connecte le client au serveur.
        // Le pseudo choisi doit être différent de ceux des autres clients connectés.
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // Serveur, port et pseudo par défaut : localhost, 28234, Anonyme
            String serveurName = (serveurBox.Text.Length == 0) ? "localhost" : serveurBox.Text;
            String portNumber = (portBox.Text.Length == 0) ? "28234" : portBox.Text;
            pseudo = (pseudoBox.Text.Length == 0) ? "Anonyme" : pseudoBox.Text;

            // L'objet RemoteInterface récupère ici la référence de l'objet du serveur.
            // On donne l'URI (serveur, port, classe du serveur) et le nom de l'interface.
            RemoteInterface = (RemotingInterface.IRemoteChain)Activator.GetObject(
                    typeof(RemotingInterface.IRemoteChain), "tcp://" + serveurName + ":" + portNumber + "/Serveur");
            
            if (RemoteInterface.RegisterUser(pseudo))
            {
                chatBox.Text += "Connecté au serveur.\n";

                boutonConnect.IsEnabled = false;
                pseudoBox.IsEnabled = false;
                serveurBox.IsEnabled = false;
                portBox.IsEnabled = false;

                boutonDisconnect.IsEnabled = true;
                boutonPing.IsEnabled = true;
                boutonEnvoi.IsEnabled = true;
                messageBox.IsEnabled = true;

                // Récupération des utilisateurs connectés.
                List<string> usersList = RemoteInterface.GetConnectedUsers();
                for (int i = 0; i < usersList.Count - 1; i++)
                {
                    clientsBox.Items.Add(usersList[i]);
                }

                // Création et lancement du thread pour la réception de messages.
                lastMessage = "";
                receiveThread = new Thread(() => ReceiveMessages());
                receiveThread.Start();
            }
            else
                chatBox.Text += "Pseudo existant. Veuillez choisir un autre pseudo.\n";
        }

        // Déconnecte le client du serveur.
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            // Stoppe le thread de réception de messages.
            receiveThread.Abort();

            // Indique la déconnexion.
            RemoteInterface.UnregisterUser(pseudo);
            RemoteInterface = null;
            chatBox.Text += "Déconnecté du serveur.\n";

            // Vide la liste des utilisateurs connectés.
            clientsBox.Items.Clear();
            
            boutonConnect.IsEnabled = true;
            pseudoBox.IsEnabled = true;
            serveurBox.IsEnabled = true;
            portBox.IsEnabled = true;

            boutonDisconnect.IsEnabled = false;
            boutonPing.IsEnabled = false;
            boutonEnvoi.IsEnabled = false;
            messageBox.IsEnabled = false;
        }

        // Envoie un ping au serveur.
        private void Check_Click(object sender, System.EventArgs e)
        {
            chatBox.Text += (RemoteInterface.PingServeur());
        }

        // Déconnecte l'utilisateur et ferme l'application.
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            if (RemoteInterface != null)
                RemoteInterface.UnregisterUser(pseudo);

            MessageBox.Show("A bientôt :)");
            Application.Current.Shutdown();
        }

        // Déconnecte l'utilisateur et ferme l'application (avec la croix rouge).
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Quit_Click(sender, null);
        }

        // Envoyer le contenu de la boîte de texte (message) précédé du pseudo.
        private void boutonEnvoi_Click(object sender, RoutedEventArgs e)
        {
            if ((pseudo + " : " + messageBox.Text) != lastMessage)
                RemoteInterface.SendMessage(pseudo + " : " + messageBox.Text);
            // Si le message envoyé est identique au précédent, on ajoute un espace pour les différencier.
            else
                RemoteInterface.SendMessage(pseudo + " : " + messageBox.Text + " ");

            // Vide la boîte de texte.
            messageBox.Text = "";
        }

        // Méthode appelée par le thread de réception de messages pour la réception.
        private void ReceiveMessages()
        {
            while (true)
            {
                string receivedMessage = RemoteInterface.GetMessage();

                // Vérifie que le message n'est pas vide et n'a pas déjà été reçu.
                if (receivedMessage.Length > 0 && receivedMessage != lastMessage)
                {
                    /* Les messages de connexion/déconnexion sont contruits ainsi :
                     * "(dé)connection%pseudo"
                     * Les messages de chat sont construits comme suit :
                     * "pseudo : message" 
                     */

                    string[] splitMsg = receivedMessage.Split('%');

                    // Le dispatcher permet aux modifications d'être effectuées par le thread principal.
                    // Connexion d'un client
                    if (splitMsg[0] == "connection")
                    {
                        App.Current.Dispatcher.BeginInvoke(
                            new Action(() => clientsBox.Items.Add(splitMsg[1])));
                    }
                    // Déconnexion d'un client
                    else if (splitMsg[0] == "disconnection")
                    {
                        App.Current.Dispatcher.BeginInvoke(
                            new Action(() => clientsBox.Items.Remove(splitMsg[1])));
                    }
                    // Message normal
                    else
                    {
                        // Différencie les messages des autres utilisateurs.
                        if (receivedMessage.Split(':')[0].Trim() != pseudo)
                        {
                            App.Current.Dispatcher.BeginInvoke(
                                new Action(() => chatBox.Text += "> " + receivedMessage + "\n"));
                        }
                        else
                        {
                            App.Current.Dispatcher.BeginInvoke(
                                new Action(() => chatBox.Text += receivedMessage + "\n"));
                        }
                    }                     

                    // Met à jour le dernier message reçu et envoie un acquittement.
                    lastMessage = receivedMessage;
                    RemoteInterface.SendAck();
                }
            }
        }
    }
}