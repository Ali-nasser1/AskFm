using System.Data.SqlClient;

namespace ASK_FM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System system = new System();
            while (true)
            {
                system.ShowMinue();
                Console.WriteLine("Press enter to return to the main menu !");
                Console.ReadKey();
                Console.Clear();
            }

        }
    }
}