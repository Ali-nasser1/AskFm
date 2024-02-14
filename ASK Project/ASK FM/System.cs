using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASK_FM
{
    internal class System
    {
        int Choice;
        UserAdmin Admin = new UserAdmin();
        User user = new User();
        public System()
        {

            Console.WriteLine("|| ======================================================================= ||");
            Console.WriteLine("||                          Ask Fm project                                 ||");
            Console.WriteLine("||                                                                         ||");
            Console.WriteLine("||                Press 1 for sign in or 2 for sign up                     ||");
            Console.WriteLine("||                                                                         ||");
            Console.WriteLine("||                                                                         ||");
            Console.WriteLine("|| ======================================================================= ||\n\n");
            int Type;
            while (!int.TryParse(Console.ReadLine(), out Type) || (Type < 1 || Type > 2))
            {
                Console.WriteLine("Error in chosen a number !");
            }

            if (Type == 1)
            {
                user.UserName = "null";
                user.Password = "null";

                // sql connection 
                SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
                con.Open();
                string sql = "select * from UserData where UserName = @user and Password = @pass";

                using SqlCommand cmd = new SqlCommand(sql, con);

                while (true)
                {
                    Console.WriteLine("please enter your user name : ");
                    user.UserName = Console.ReadLine();
                    Console.WriteLine("please enter your password : ");
                    user.Password = Console.ReadLine();
                    cmd.Parameters.Clear(); // clear previous para.
                    cmd.Parameters.AddWithValue("@user", user.UserName);
                    cmd.Parameters.AddWithValue("@pass", user.Password);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user.Id = (int)reader["Id"];
                        user.Name = (string)reader["Name"];
                        user.Email = (string)reader["Email"];
                        user.UserName = (string)reader["UserName"];
                        user.Password = (string)reader["Password"];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid username or password! Please try again.\n");
                    }
                }
                con.Close();
            }
            else
            {
                // sign up
                // assign an id for this user
                // get last id number and assign id + 1 for this user <-- do this -->

                // Name
                Console.WriteLine("plase enter your name");
                user.Name = Console.ReadLine();
                //Console.WriteLine($"debug : {user.Name}");
                while (String.IsNullOrEmpty(user.Name))
                {
                    Console.WriteLine("please enter a valid name");
                    user.Name = Console.ReadLine();
                }
                // email
                Console.WriteLine("plase enter your email");
                user.Email = Console.ReadLine();
                while (String.IsNullOrEmpty(user.Email))
                {
                    Console.WriteLine("please enter a valid email");
                    user.Email = Console.ReadLine();
                }
                // note* handle if this email and not exsit before

                // user name
                Console.WriteLine("plase enter your user name");
                user.UserName = Console.ReadLine();
                while (String.IsNullOrEmpty(user.UserName))
                {
                    Console.WriteLine("try not exist user name"); // handle this exception using query
                    user.UserName = Console.ReadLine();
                }

                // password
                Console.WriteLine("plase enter your Password");
                user.Password = Console.ReadLine();
                while (String.IsNullOrEmpty(user.Password))
                {
                    Console.WriteLine("a valid password"); // handle this exception using query
                    user.Password = Console.ReadLine();
                }

                // anonymous
                Console.WriteLine("Do you accept anonymous questions ? press (0 = NO | 1 = YES)");
                int isOK;
                while (!int.TryParse(Console.ReadLine(), out isOK) || (isOK < 0 || isOK > 1))
                {
                    Console.WriteLine("a valid number"); // handle this exception using query

                }
                user.Anonymous = isOK;
                // adding the user into data base

                SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
                con.Open();
                string sql = "insert into UserData values (@Name, @Email, @UserName, @Password, @Anonymous)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("@Name", user.Name));
                cmd.Parameters.Add(new SqlParameter("@Email", user.Email));
                cmd.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                cmd.Parameters.Add(new SqlParameter("@Password", user.Password));
                cmd.Parameters.Add(new SqlParameter("@Anonymous", user.Anonymous));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void ShowMinue()
        {
            Console.WriteLine($"Hello, {user.Name} !\n");
            string Menu =
            "1: Print Question To Me.\n" +
            "2: Print Question From Me.\n" +
            "3: Answer Question.\n" +
            "4: Delete Question.\n" +
            "5: Ask Question.\n" +
            "6: List System Users.\n" +
            "7: Feed.\n" + // i have a trouble with this
            "8: Logout";

            Console.WriteLine(Menu);
            Console.WriteLine("please choose a number ");
            while (!int.TryParse(Console.ReadLine(), out Choice) || (Choice < 1 || Choice > 8))
            {
                Console.WriteLine("Invalid Choice");
            }

            if (Choice == 1)
            {
                // Print Inbox
                Admin.PrintToMe(user);

            }
            else if (Choice == 2)
            {
                Admin.PrintFromMe(user);

            }
            else if (Choice == 3)
            {
                Admin.SetAnswer(user);
            }
            else if (Choice == 4)
            {
                Admin.DeleteQ(user);
            }
            else if (Choice == 5)
            {
                Admin.AskQ(user);
            }
            else if (Choice == 6)
            {
                Admin.ListAllUsers();
            }
            else if (Choice == 7)
            {
                Admin.PrintFeed();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}