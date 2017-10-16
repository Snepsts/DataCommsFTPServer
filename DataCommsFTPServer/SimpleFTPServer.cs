/******************************************************************************
            SimpleEchoServer.cs - Simple TCP echo server using sockets

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

            SimpleFTPServer.cs - Simple FTP server using sockets

  This program demonstrates the use of socket APIs to receive files from the
  client. The user interface is via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2017.

  Copyright 2017 by Michael Ranciglio for VS2017
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleFTPServer
 * Purpose: wait for a connection from an FTP client and receive files
 * Usage:   SimpleFTPServer <portnum>
 *
 *-----------------------------------------------------------------------
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

class SimpleFTPServer
{
	public static void Main(string[] args)
	{
		int recv;
		byte[] data = new byte[1024];

		if(args.Length > 1) // Test for correct # of args
			throw new ArgumentException("Parameters: [<Port>]");

		IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Int32.Parse(args[0]));
		Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		server.Bind(ipep);
		server.Listen(10);

		string path = "C:\\Users\\Public\\";
		for (; ; )
		{
			Console.WriteLine("Do you need to shut down server? Yes or No");
			string choice = Console.ReadLine();
			if (choice.Contains("Y") || choice.Contains("y"))
			{
				Console.WriteLine("The server is shutting down...");
				break;
			}

			Console.WriteLine("Waiting for a client...");
			Socket client = server.Accept();
			IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
			string connectMsg = "Connected with " + clientep.Address + " at port " + clientep.Port;
			Console.WriteLine(connectMsg);

			string welcome = "Welcome to my test server";
			data = Encoding.ASCII.GetBytes(welcome);
			client.Send(data, data.Length, SocketFlags.None);

			while (true)
			{
				data = new byte[1024];
				recv = client.Receive(data);

				if (recv == 0)
					break;

				string name = Encoding.ASCII.GetString(data, 0, recv);

				client.Send(data, recv, SocketFlags.None);

				data = new byte[102400]; //100 KB
				recv = client.Receive(data);

				if (recv == 0)
					break;

				string file = path + name;
				FileStream download = new FileStream(file, FileMode.Create);

				Console.WriteLine("Receiving file...");
				download.Write(data, 0, recv);

				client.Send(Encoding.ASCII.GetBytes("accept"));
			}

			Console.WriteLine("Disconnected from " + clientep.Address);
			client.Close();
		}

		server.Close();
	}
}