using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Homework2_Server
{
    class HW2_Server
    {
        //Global variables
        private const int port = 10572;
        static void Main(string[] args)
        {
            //Start server
            System.Net.Sockets.TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Started..");

            //Accept client connections
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for client connection.");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected to client.");

                    //Listen for new clients and accept them
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        private static void HandleClient(object param)
        {
            TcpClient client = (TcpClient)param;
            Socket sock = client.Client;
            string clientId = sock.RemoteEndPoint.ToString();

           
            Console.WriteLine("Accepted client connection from {0} +\n", clientId);
        

          //List of Bank Clients that can use the app
            List<ClientClass.Client> clients = new List<ClientClass.Client>();

            clients.Add(new ClientClass.Client("0001", "1000000000000000", 4.20M, "User 1"));
            clients.Add(new ClientClass.Client("0011", "1000000000000001", 0.01M, "User 2"));
            clients.Add(new ClientClass.Client("0101", "1000000000000021", 7.00M, "User 3"));
            clients.Add(new ClientClass.Client("1001", "1000000000000032", 10.00M, "User 4"));
            clients.Add(new ClientClass.Client("1221", "1000000000000008", 10.10M, "User 5"));


            //Variables used in processin user commands
            bool authentication = false;        //To check if correct pin has been entered.
            int currentUser = -1;                //index of current user
            decimal change = 0.00M;             //Stores credit/debit number
            bool deducing = false;              //To check if current input is for a balance deduction
            bool adding = false;                //To check if current input is for adding to balance
            bool checkPin = false;              //Has pin been verified
           
            
            /* 
             * Debug code for client list
             * 
             * foreach (ClientClass.Client user in clients)
             {
                 Console.WriteLine(user.ToString());
             }
             *
             */

            while (true)
            {
                try
                {
                    //Recieve client input
                    byte[] bytes = new byte[1024];
                    int bytesRec = sock.Receive(bytes);
                    string request = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    string response = "NOTOK Retry"; //Default



                    //Process commands
                    if (request == "0")
                    {
                        response = "CLOSE";
                    }
                    else if ((adding != true && deducing != true) && request == "1" && authentication == false)
                    //Check if not authenticated and start authentication
                    {

                        //Prompt user to input pin
                        checkPin = true;
                        response = "OK Enter 4 digit pin.";
                        
                    } else if  (checkPin == true && request.Length==4 && (adding != true && deducing != true) && authentication == false)
                    //Check pin against client list pins
                    {
                       

                        //Console.WriteLine("Pin entered {0}", pin);

                        for (int i = 0; i < clients.Count; i++) 
                                                                    //Compare pin to each client's pin
                        {
                          
                            if (request == clients[i].PinNum)       //If a match is found mark as authenticated
                            {
                                authentication = true;
                                response = "OK";
                                currentUser = i;

                                // Console.WriteLine("Current user index: " + currentUser);
                                break; //Exit for loop on match
                            }
                        }
                        if (authentication == false)               //If no match has been found
                        {
                            response = "NOTOK"; 
                        }
                    }
                    else if (request == "2" && authentication == true && (adding != true && deducing != true))
                    //If authenticated print balance
                    {
                        response = "OK Current balance is: " + clients[currentUser].AccBalance + "$";
                    }
                    else if (request == "3" && authentication == true && (adding != true && deducing != true))
                    //Start credit sequence
                    {

                        adding = true;
                        response = "OK Insert amount to deposit. Use correct system decimal delimeter.";
                        //Decimal processing depends on the systems delimeter.
                        //If system uses ',' typing 1.23 will be read as 123.
                    }
                    else if (request == "4" && authentication == true && (adding != true && deducing != true))
                    //Start debit sequence
                    {
                        deducing = true;
                        response = "OK Insert ammount to withdraw. Use correct system decimal delimeter.";
                    
                    }
                    else if (authentication == true && deducing == true && true == decimal.TryParse(request, out change))
                    //If input is for a deduction, deduce
                    {
                        //Debit function checks funds prints a matching server responce 
                        response = clients[currentUser].Debit(change);
                        deducing = false;           //End deduction sequence
                    }
                    else if (authentication == true && adding == true && true == decimal.TryParse(request, out change))
                    //If input is for addition, add
                    {
                        //Credit function adds to balance and gives server responce
                        response= clients[currentUser].Credit(change);
                        adding = false;
                    } 
                    else if (authentication == true)
                    //Input is not serving a command after authentication, possibly a typo
                    {
                        response = "NOTOK Wrong input.";
                    }
                    else
                    //Input is not a command or is a command without prior authentication
                    {
                        response = "NOTOK Authenticate.";
                    }

                   
                    //Send responce to client
                    Console.WriteLine("{0} sent: {1}  responded: {2} ", clientId, request, response);
                    byte[] replyBuffer = Encoding.ASCII.GetBytes(response);

                    sock.Send(replyBuffer);

                    //Official connection close upon close command
                    if (response == "CLOSE")
                    {
                        Console.WriteLine("Closing Connection");
                        client.Dispose();
                        break;
                    }
                } catch (SocketException e)
                {
                    Console.WriteLine(e.SocketErrorCode);
                    break;
                } catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }

        }

    }
}
