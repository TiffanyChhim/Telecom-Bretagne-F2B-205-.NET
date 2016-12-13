
// TP3 Chat - Maxime MONIN et Tiffany CHHIM

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.Generic;
using RemotingInterface;

namespace remoteServeur
{
	// Le serveur contient les données du chat et implémente les méthodes de l'interface IRemoteChain.

    /* A propos de la gestion des messages :
     * Lorsqu'un utilisateur envoie un message, celui-ci est ajouté à une liste de messages.
     * Lorsqu'un utilisateur demande un message (avec son thread de réception, 
     * le premier de la liste lui est envoyé. 
     * L'utilisateur envoie alors un acquittement.
     * Lorsque le message a été acquitté par tous les utilisateurs, on le retire de liste
     * et on passe au message suivant.
     */ 

	public class Serveur : MarshalByRefObject, RemotingInterface.IRemoteChain
    {
        private List<String> clientList = new List<String>();
        private List<String> latestMessages = new List<String>();
        private int nbAck = 0;

        static void Main()
		{
			// Création d'un nouveau canal pour le transfert des données via un port.
			TcpChannel canal = new TcpChannel(28234);

			// Le canal ainsi défini doit être enregistré dans l'annuaire.
			ChannelServices.RegisterChannel(canal, false);

			// Démarrage du serveur en écoute sur objet en mode Singleton.
			// Publication du type avec l'URI et son mode. 
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(Serveur), "Serveur",  WellKnownObjectMode.Singleton);

            Console.WriteLine("Le serveur est bien démarré (port d'écoute : 28234).");
			// Pour garder la main sur la console.
			Console.ReadLine();	
		}

		// Pour laisser le serveur fonctionner sans time out.
		public override object InitializeLifetimeService()
		{
			return null;
		}

        #region Membres de IRemoteChain

        // Renvoie un message pour indiquer que le serveur est opérationnel.
        public string PingServeur()
		{
			return "Serveur de chat disponible.\n" ;
		}

        // Vérifie si le pseudo existe. L'ajoute s'il n'existe pas.
        public bool RegisterUser(string pseudo)
        {
            if (!clientList.Contains(pseudo))
            {
                clientList.Add(pseudo);

                // Message indiquant la connexion aux autres clients.
                SendMessage("connection%" + pseudo);
                Console.WriteLine("Utilisateur " + pseudo + " s'est connecté.");

                return true;
            }
            // Si le pseudo existe déjà, la connection n'est pas acceptée.
            return false;
        }

        // Retire le client (pseudo) de la liste.
        public void UnregisterUser(string pseudo)
        {
            clientList.Remove(pseudo);

            // Message indiquant la déconnexion aux autres clients.
            SendMessage("disconnection%" + pseudo);
            Console.WriteLine("Utilisateur " + pseudo + " s'est déconnecté.");
        }

        // Ajoute le message à la liste d'attente des messages à envoyer.
        public void SendMessage(string message)
        {
            latestMessages.Add(message);
        }

        // Incrémente le nombre d'acquittements pour le premier message de la file d'attente.
        public void SendAck()
        {
            nbAck++;
            Console.WriteLine("Message recu par " + nbAck + " utilisateur(s) sur " + clientList.Count + " utilisateur(s) au total.");

            // Lorsque tout le monde a reçu le message, il est retiré de la liste d'attente.
            if (nbAck == clientList.Count)
            {
                nbAck = 0;
                latestMessages.RemoveAt(0);
            }
        }

        // Renvoie le premier message de la liste d'attente des messages à envoyer.
        public string GetMessage()
        {
            if (latestMessages.Count > 0)
                return latestMessages[0];
            else
                return "";
        }

        // Retourne la liste des utilisateurs connectés.
        public List<string> GetConnectedUsers()
        {
            return clientList;
        }

        #endregion
    }
}
