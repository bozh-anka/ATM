using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2_Client
{
    class HW2_Client
    {

        //GLOBAL VARIABLES
        private const int serverPort = 10572;
        private const string serverAddress = "127.0.0.1";
        static void Main(string[] args)
        {
            //Initial instructions to user
            Console.Write("Type START to connect to Bank.");

            //Checking for start prompt before attempting to connect to server
            if (Console.ReadLine().ToUpper() == "START")
            {
                //try catch to exit app if server is offline
                try
                {
                    //Establishing connection
                    TcpClient client = new TcpClient();
                    client.Connect(serverAddress, serverPort);
                    Socket sock = client.Client;

                    //Instructing user 
                    string instructions =        "Enter 1 to authenticate and use the Bank ATM functions below. \n" +
                                                                               "Enter 2 to view current balance.\n" +
                                                               "Enter 3 and follow instrctions to deposit a sum.\n" +
                                                             "Enter 4 and follow instructions to withdraw a sum.\n" +
                                                                                                 "Enter 0 to exit.\n";
                    Console.WriteLine(instructions);


                    while (true)
                    {

                        //try catch block to process communication erros
                        try
                        {
                            //Reading input
                            string input = Console.ReadLine();

                            if (input.Length == 0)
                            {
                                //Skip to next input if previous input is a blank line
                                continue;
                            }

                            //Send input to server
                            byte[] requestBuff = Encoding.ASCII.GetBytes(input);
                            sock.Send(requestBuff);

                            //Recieve output from server 
                            byte[] receiveBuff = new byte[1024];
                            int bytesRec = sock.Receive(receiveBuff);

                            //Decode responce and print it
                            string response = Encoding.ASCII.GetString(receiveBuff, 0, bytesRec);
                            Console.WriteLine("Server response is: " + response);

                            //Quit if responce says quit
                            if (response == "CLOSE" || (response == "NOTOK" && response.Length == 5))
                            {
                                Console.WriteLine("Closing Connection");
                                client.Dispose();
                                Thread.Sleep(1000); // allow  user to read
                                break; // Exit loop
                            }
                        }
                        catch (Exception e)
                        {
                            //Inform of error and exit loop
                            Console.WriteLine("ENCOUNTERED ERROR: RESTART CLIENT");
                            //Console.WriteLine(e.ToString());
                            Console.WriteLine("Closing Connection");
                            client.Dispose();
                            Thread.Sleep(1000);
                            break;
                        }
                    }
                }catch ( SocketException e)
                {
                    //Inform of error and exit loop 
                    Console.WriteLine("ENCOUNTERED ERROR: SERVER OFFLINE ");
                   // Console.WriteLine(e.ToString());
                    Console.WriteLine("Closing Connection");
                   
                    Thread.Sleep(1000);

                }
            }
            else
            {
                Console.WriteLine("Request not sent. Closing program."); Thread.Sleep(1000);
            }
            //Exit client
            Environment.Exit(0);
        }
    }
}
