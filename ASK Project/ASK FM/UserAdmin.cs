using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ASK_FM
{
    internal class UserAdmin
    {
        static Dictionary<int, User> AllUsers = new Dictionary<int, User>();
        static UserAdmin()
        {
            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "Select * from UserData";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AllUsers[(int)reader["Id"]] = new User()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Email = (string)reader["Email"],
                    UserName = (string)reader["UserName"],
                    Anonymous = (int)reader["Anonymous"]
                };

            }
            con.Close();
        }
        public void PrintToMe(User user)
        {
            Dictionary<int, Question> ParentQ = new Dictionary<int, Question>(); // pass to constructure is bettter
            Dictionary<int, List<Question>> Threads = new Dictionary<int, List<Question>>();

            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "Select * from QuestionData where Thread is null and ToId = @Id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@Id", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            // get all parent question
            while (reader.Read())
            {
                ParentQ[(int)reader["Id"]] = new Question()
                {
                    Body = (string)reader["Body"],
                    Answer = reader["Answer"] != DBNull.Value ? (string)reader["Answer"] : null,
                    FromUser = (int)reader["FromId"],
                    ToUser = (int)reader["ToId"]
                };

            }
            reader.Close();
            cmd.Parameters.Clear();
            sql = "Select * from QuestionData where Thread is not null and ToId = @Id";
            cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@Id", user.Id));
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int threadId = (int)reader["Thread"];
                if (!Threads.ContainsKey(threadId))
                {
                    Threads[threadId] = new List<Question>();
                }

                Threads[threadId].Add(
                    new Question()
                    {
                        Body = (string)reader["Body"],
                        Answer = (string)reader["Answer"],
                        FromUser = (int)reader["FromId"],
                        ToUser = (int)reader["ToId"]
                    });
            }
            reader.Close();
            con.Close();
            if (ParentQ.Count() == 0)
            {
                Console.WriteLine("there is no Questions for you !");
                return;
            }

            foreach (var parent in ParentQ)
            {
                Console.WriteLine($"Question ID ({parent.Key}) from user id ({parent.Value.FromUser})  Question : {parent.Value.Body} ?"); // from & to
                Console.WriteLine($"Answer: {parent.Value.Answer ?? "N/A"}");
                List<Question> list = new List<Question>();
                if (Threads.ContainsKey(parent.Key))
                {
                    list = Threads[parent.Key];
                    foreach (var Thrd in list)
                    {
                        Console.WriteLine($"Thread: Question ID ({Thrd.IdQ}) from user ID ({Thrd.FromUser})");
                        Console.WriteLine($"Question :{Thrd.Body}");
                        Console.WriteLine($"Thread: Answer: {Thrd.Answer}");
                    }
                }
            }
        }

        public void PrintFromMe(User user)
        {
            List<Question> QData = new List<Question>();
            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "select * from QuestionData where FromId = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@Id", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                QData.Add(
                new Question()
                {
                    IdQ = (int)reader["Id"],
                    Body = (string)reader["Body"],
                    Answer = reader["Answer"] != DBNull.Value ? (string)reader["Answer"] : "",
                    FromUser = (int)reader["FromId"],
                    ToUser = (int)reader["ToId"]
                });
            }
            reader.Close();
            cmd.Parameters.Clear();
            con.Close();
            if (QData.Count == 0)
            {
                Console.WriteLine("You didn't ask any question !");
                return;
            }
            foreach (var Q in QData)
            {
                string AQ = AllUsers[Q.ToUser].Anonymous == 1 ? "" : "!AQ";
                Console.WriteLine($"Question ID ({Q.IdQ}) to {AQ} user ID ({Q.ToUser}) : ");
                Console.WriteLine($"Question : {Q.Body} ?");
                Console.WriteLine($"Answer : {Q.Answer} \n");
            }
        }

        public void SetAnswer(User user)
        {
            // set answer in data base for a question
            Console.WriteLine("what is the ID of wanted question or -1 to cancel?");
            int ID;
            while (!int.TryParse(Console.ReadLine(), out ID))
                Console.WriteLine("Please enter a valid number !");
            if (ID == -1) return;
            Console.WriteLine($"enter your answer for the questin ID {ID}");
            string answer = Console.ReadLine();
            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "update QuestionData set Answer = @value where Id = @id and ToId = @Uid";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@value", answer);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.Parameters.AddWithValue("@Uid", user.Id);
            int affectedRows = cmd.ExecuteNonQuery();
            con.Close();
            if (affectedRows == 0)
            {
                Console.WriteLine("Error has been occured maybe you don't have this question or id is wrong !");
                return;
            }
            Console.WriteLine("successful operation");
        }

        public void DeleteQ(User user)
        {
            Console.WriteLine("what is the ID of wanted question or -1 to cancel?");
            int ID;
            while (!int.TryParse(Console.ReadLine(), out ID))
                Console.WriteLine("Please enter a valid number !");
            if (ID == -1) return;
            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "delete from QuestionData where Id = @id and FromId = @Uid";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.Parameters.AddWithValue("@Uid", user.Id);
            int affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0)
                Console.WriteLine("You don't have this question to delete !");
            else
                Console.WriteLine("the question has been delete it");
            con.Close();
        }

        public void AskQ(User user)
        {
            Console.WriteLine("enter user ID or -1 to cancel ?");
            int ID;
            while (!int.TryParse(Console.ReadLine(), out ID))
                Console.WriteLine("Please enter a valid number !");

            if (ID == -1) return;

            while (!AllUsers.ContainsKey(ID))
            {
                Console.WriteLine("Please enter exist user !");
                while (!int.TryParse(Console.ReadLine(), out ID))
                    Console.WriteLine("Please enter a valid number !");

            }
            if (AllUsers[ID].Anonymous == 0)
                Console.WriteLine("*NOTE: this user doesn't accept anonymous questions!");

            Console.WriteLine("For thread questin Enter question id or -1 for new one");
            int QID;
            while (!int.TryParse(Console.ReadLine(), out QID))
                Console.WriteLine("Please enter a valid number !");
            Console.WriteLine("enter question text");
            string text = Console.ReadLine();

            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "NULL";
            if (QID == -1)
            {
                // make a new one
                sql = "insert into QuestionData (Body, FromId, ToId) values(@text, @from, @to)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@text", text);
                cmd.Parameters.AddWithValue("@from", user.Id);
                cmd.Parameters.AddWithValue("@to", ID);
                cmd.ExecuteNonQuery();
            }
            else
            {
                // check if the Question id exist or not
                sql = "select count(*) from QuestionData where Id = @QID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@QID", QID);
                int affectedRows = (int)cmd.ExecuteScalar();
                if (affectedRows == 0)
                {
                    Console.WriteLine("Question not found, please try again !");
                    return;
                }
                // make a thread
                cmd.Parameters.Clear();
                sql = "insert into QuestionData (Body, FromId, ToId, Thread) values(@text, @from, @to, @QID)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@text", text);
                cmd.Parameters.AddWithValue("@from", user.Id);
                cmd.Parameters.AddWithValue("@to", ID);
                cmd.Parameters.AddWithValue("@QID", QID);
                cmd.ExecuteNonQuery(); // The INSERT statement conflicted with the FOREIGN KEY SAME TABLE constraint "FK__QuestionD__Threa__5AEE82B9". The conflict occurred in database "AskFmDB", table "dbo.QuestionData", column 'Id'.
                

            }
            con.Close();
            Console.WriteLine("successful operation");
        }

        public void ListAllUsers()
        {
            foreach (var user in AllUsers)
            {
                Console.WriteLine($"ID {user.Key} Name : {user.Value.Name}");
            }
        }

        public void PrintFeed()
        {
            Dictionary<int, Question> ParentQ = new Dictionary<int, Question>(); // pass to constructure is bettter
            Dictionary<int, List<Question>> Threads = new Dictionary<int, List<Question>>();

            SqlConnection con = new SqlConnection("Data Source=Dell-xps;Initial Catalog=AskFmDB;Integrated Security=True");
            con.Open();
            string sql = "Select * from QuestionData where Thread is null";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader = cmd.ExecuteReader();
            // get all parent question
            while (reader.Read())
            {
                int id = (int)reader["Id"];
                ParentQ[id] = new Question()
                {
                    Body = (string)reader["Body"],
                    Answer = reader["Answer"] != DBNull.Value ? (string)reader["Answer"] : "",
                    FromUser = (int)reader["FromId"],
                    ToUser = (int)reader["ToId"]
                };

            }
            reader.Close();
            cmd.Parameters.Clear();
            sql = "Select * from QuestionData where Thread is not null";
            cmd = new SqlCommand(sql, con);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int threadId = reader["Thread"] != DBNull.Value ? (int)reader["Thread"] : 0; // Assuming default value for int is 0

                if (!Threads.ContainsKey(threadId))
                {
                    Threads[threadId] = new List<Question>();
                }

                Threads[threadId].Add(
                    new Question()
                    {
                        Body = (string)reader["Body"],
                        Answer = reader["Answer"] != DBNull.Value ? (string)reader["Answer"] : "",
                        FromUser = (int)reader["FromId"],
                        ToUser = (int)reader["ToId"]
                    });
            }
            reader.Close();
            con.Close();

            foreach (var parent in ParentQ)
            {
                Console.WriteLine($"Questin ID ({parent.Key}) from user ID [{parent.Value.FromUser}] to user ID [{parent.Value.ToUser}] is {parent.Value.Body} ?");
                Console.WriteLine($"Answer : {parent.Value.Answer ?? "N/A"} ");
                List<Question> list = new List<Question>();
                list = Threads[parent.Key];
                foreach (var Thrd in list)
                {
                    Console.WriteLine($"Thread : Parent Questin ID {parent.Key} Question ID {parent.Value.IdQ} Q {Thrd.Body}");
                    Console.WriteLine($"Answer : {Thrd.Answer}");
                }
            }
        }
    }
}