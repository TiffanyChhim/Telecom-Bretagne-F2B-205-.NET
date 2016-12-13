
// TP3 Chat - Maxime MONIN et Tiffany CHHIM

using System;
using System.Collections.Generic;

namespace RemotingInterface
{
	// Cette interface contient la déclaration de toutes les 
	// méthodes de l'objet distribué.
	public interface IRemoteChain
	{
		string PingServeur();

        bool RegisterUser(string pseudo);

        void UnregisterUser(string pseudo);

        void SendMessage(string message);

        void SendAck();

        string GetMessage();

        List<string> GetConnectedUsers();
	}
}
