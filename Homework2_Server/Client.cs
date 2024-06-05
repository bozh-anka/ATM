using System;

namespace Homework2_Server
{
    internal class ClientClass
    {
		public class Client
		{
			//Class members
			private
			string pinNum;
			string accNum;
			decimal accBalance;
			string name;

			public Client()
			{
				//default values
				pinNum = "1000";
                accNum = "1000000000000000";
				accBalance = 0.00M;
				name = "Account Holder";
			}

			//Constructor for setting all fields
			public Client(string pin, string acc, decimal balance, string holder)
			{
				accBalance = balance;
				name = holder;

				//Verifications 
				if (true == int.TryParse(pin,out _) && pin.Length==4)
				{
					pinNum = pin;
				}
				else
				{
					Console.WriteLine("Bad pin, set to Default 1000.");
					pinNum = "1000";
				}


                if (true == long.TryParse(acc, out _) && acc.Length == 16)
				{
					accNum = acc;
				}
					else
				{
					Console.WriteLine("Bad account number, set to Default" + 1E15 + ".");
					accNum = "1000000000000000";
				}


			}



			//Getter and setter for the pin number
			public string PinNum
			{
				get
				{
					return pinNum;
				}

				set
				{
					//Verifying correct number of digits
					if(true == int.TryParse(value, out _) && value.Length == 4)
						{
						pinNum = value;
					}
					else
					{
						Console.WriteLine("Bad pin, set to Default 1000.");
						pinNum = "1000";
					}
				}
			}

			public string AccNum
			{
				get
				{
					return accNum;
				}

				set
				{
					if (true == long.TryParse(value, out _) && value.Length==16)
					{
						accNum = value;
					}
					else
					{
						Console.WriteLine("Bad account number" + value + ", set to Default" + 1E15 + ".");
						accNum = "1000000000000000";
					}
				}
			}
				public decimal AccBalance
			{
				get
				{
					return accBalance;
				}
				set
				{
					//Account balance can be negative, thus no value check
					value = accBalance;
				}
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					value = name;
				}
			}

			public string Credit(decimal add)
            {
				//Ensuring positive number
				if (add > 0)
				{
					accBalance += add;
					return "OK Account balance has been updated and now holds " + accBalance + "$.";
				}
				else
				{
					return "NOTOK Credit request refused, bad input";
				}
			}

			public string Debit(decimal sub)
            {
				//Ensuring positive number
				if(accBalance>= sub && sub >0)
                {
                    accBalance -= sub;
                    return "OK Account balance has been updated and now holds " + accBalance + "$.";
				} else
                {
					return ("NOTOK Debit request refused as account only holds "+ accBalance +"$.");
                }

				
            }

			//Override of to string for printing
			public override string ToString() {
				return "Client"+ name+ " has pin " + pinNum + " with account number: " + accNum + " balance: " + accBalance;
			}
		}
	}
}