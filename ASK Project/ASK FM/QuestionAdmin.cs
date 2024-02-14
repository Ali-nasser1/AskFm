using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASK_FM
{
    internal static class QuestionAdmin
    {
        //          id    question
        //static Dictionary<int, List<Question>> Thread = new Dictionary<int, List<Question>>();
        public static void Print(Question Q)
        {
            Console.WriteLine($"Question id ({Q.IdQ}) form user {Q.FromUser} to {Q.ToUser} => {Q.Body} ");
            if (Q.Answer is not null)
            {
                Console.WriteLine($"Answer id {Q.Answer} => {Q.Answer}");
            }

        }

        public static void PrintThread(List<Question> questions)
        {
            foreach (Question q in questions)
            {
                Console.Write("Thread : ");
                Print(q);
                Console.WriteLine();
            }
        }
    }
}
