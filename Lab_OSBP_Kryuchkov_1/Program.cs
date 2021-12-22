using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Lab_OSBP_Kryuchkov_1
{
    class Program
    {
        static void Main()
        {
            new BruteForce().start();
        }
    }

    class BruteForce
    {
        Stopwatch timer = new Stopwatch();
        List<string> hash = new List<string>();

        public void start()
        {
            timer.Start();
            List<string> words = new List<string>();
            string str = "";
            Console.WriteLine("Введите количество потоков: ");
            int threads = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("");

            string path = "hash.txt";
            StreamReader read = new StreamReader(path);
            string[] alphabet = new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
            "m", "n", "o", "p", "q","r", "s", "t", "u", "v", "w", "x", "y", "z"};
            string line;
            while ((line = read.ReadLine()) != null)
            {
                hash.Add(line);
            }
            foreach (string symbol1 in alphabet)
            {
                foreach (string symbol2 in alphabet)
                {
                    foreach (string symbol3 in alphabet)
                    {
                        foreach (string symbol4 in alphabet)
                        {
                            foreach (string symbol5 in alphabet)
                            {
                                str = symbol1 + symbol2 + symbol3 + symbol4 + symbol5; //само слово
                                words.Add(str);
                            }
                        }
                    }
                }
            }
            ExecuteThreads(threads, words);

        }

        public string sha256(string str)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void ExecuteThreads(int threads, List<string> words)
        {
            int i = 0;
            int j = 0; //конец текущего набора слов
            int endPart = words.Count/threads; //количество слов для одного потока

            int part = 0;
            while (i < threads)
            {
                int u = j;
                part += endPart; //количество слов в данном потоке
                i++;
                for (;j<words.Count;j++)
                {
                    if (j > part)
                    {
                        break;
                    }
                }
                new Thread(() => { Proc(words,u,  j); }).Start();
            }
        }

        public void Proc(List<string> currentWords, int start, int end)
        {
            for (int i = 0; i < currentWords.Count; i++)
            {
                if (i >= start && i<=end)
                {
                    string word = currentWords[i];
                    string buff = sha256(word);
                    foreach (string H in hash)
                    {

                        if (buff == H)
                        {
                            Console.WriteLine(word);
                            Console.WriteLine("Количество времени на поиск слова: " + (timer.ElapsedMilliseconds)/1000 + "\n");
                        }
                    }
                }
            }
        }
    }
}
