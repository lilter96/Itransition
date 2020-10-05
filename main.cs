using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace script
{
    class Program
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static Random rnd = new Random();
        private const int size = 25;
        private static String PlayerMove(ref List<String> values)
        {
            int move = values.Count() + 1;
            var good = false;
            while (!good || (move > values.Count()  || move < 0))
            {
                Console.WriteLine("Available moves:");
                for (int i = 0; i < values.Count(); i++) Console.WriteLine("{0} - {1}", i + 1, values[i]);
                Console.WriteLine("0 - exit");
                Console.Write("Enter your move:");
                good = int.TryParse(Console.ReadLine(), out move);
                if (!good || move > values.Count() || move <0)
                {
                    Console.WriteLine("Invalid move!");
                }
            }
            if (move == 0)
            {
                Console.WriteLine("See you later nerds! :)");
                return "exit";
            }
            else
            {
                Console.WriteLine("Your move: {0}", values[move - 1]);
            }
            return values[move - 1];
        }
        private static String FightResult(ref List<String> values,ref String player, ref String computer)
        {
            int index1 = values.IndexOf(player);
            int index2 = values.IndexOf(computer);
            if (index1 == index2) return "Draw!!!";
            int count = (values.Count() - 1) >> 1;
            if (index1 - count >= 0)
                return (index2 < index1 && index2 >= index1 - count)? "You win!" : "Computer win!";
            else
            {
                return (index2 < index1 || index2 >= values.Count() - 1 + index1 - count) ? "You win!" : "Computer win!";
            }
        }
        private static bool Check(ref List<String> values)
        {
            SortedSet<String> tmp = new SortedSet<String>();
            values.ForEach(x => tmp.Add(x));
            if (values.Count() != tmp.Count() || values.Count()%2!=1) return false;
            return true;
        }
        static void Main(string[] args)
        {
            //args.ToList().ForEach(x => Console.Write(x + ' '));
            //Console.WriteLine();
            List<String> values = args.ToList();//Console.ReadLine().Split(' ').ToList();
            if (!Check(ref values))
            {
                Console.WriteLine("Invalid input!");
                return;
            }
            String computerMove = values[rnd.Next(0, values.Count() - 1)];
            byte[] key = new Byte[size];
            rngCsp.GetBytes(key, 0, size);
            String HMAC_key = "";
            key.ToList().ForEach(x=>HMAC_key+=string.Format("{0:x2}", x));
            byte[] tmp = Encoding.UTF8.GetBytes( computerMove);
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] hashValue = hmac.ComputeHash(tmp);
                var hmac_str = "";
                hashValue.ToList().ForEach(x => hmac_str += string.Format("{0:x2}", x));
                Console.WriteLine("HMAC: {0}", hmac_str);
            }
            var playerMove = PlayerMove(ref values);
            if (playerMove == "exit") return;
            Console.WriteLine("Computer move: {0}", computerMove);
            Console.WriteLine(FightResult(ref values, ref playerMove, ref computerMove));
            Console.WriteLine("HMAC key: {0}", HMAC_key);
        }
    }
}
