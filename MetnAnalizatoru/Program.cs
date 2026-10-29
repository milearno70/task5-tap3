using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetnAnalizatoru
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Mətn faylının yolu: ");
            string yol = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(yol))
            {
                Console.WriteLine("Xəta: Fayl yolu boş ola bilməz.");
                return;
            }

            yol = yol.Trim().Trim('"');

            if (!File.Exists(yol))
            {
                Console.WriteLine("Xəta: Fayl tapılmadı: " + yol);
                return;
            }

            int setirSayi = 0;
            int sozSayi = 0;
            int simvolSayi = 0;
            string enUzunSoz = "";
            Dictionary<string, int> tekrar = new Dictionary<string, int>();

            char[] ayiricilar = { ' ', '\t', ',', '.', '!', '?', ';', ':', '-', '(', ')', '"', '\'', '/', '\\' };

            try
            {
                using (StreamReader sr = new StreamReader(yol, Encoding.UTF8))
                {
                    string setir;
                    while ((setir = sr.ReadLine()) != null)
                    {
                        setirSayi++;
                        simvolSayi += setir.Length;

                        string[] sozler = setir.Split(ayiricilar, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string soz in sozler)
                        {
                            sozSayi++;

                            if (soz.Length > enUzunSoz.Length)
                                enUzunSoz = soz;

                            string kicik = soz.ToLower();
                            if (tekrar.ContainsKey(kicik))
                                tekrar[kicik]++;
                            else
                                tekrar[kicik] = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fayl oxunarkən xəta: " + ex.Message);
                return;
            }


            List<KeyValuePair<string, int>> siyahi = new List<KeyValuePair<string, int>>(tekrar);
            siyahi.Sort((a, b) => b.Value.CompareTo(a.Value));

            int top = Math.Min(3, siyahi.Count);


            try
            {
                using (StreamWriter sw = new StreamWriter("hesabat.txt", false, Encoding.UTF8))
                {
                    sw.WriteLine("===== MƏTN ANALİZ HESABATI =====");
                    sw.WriteLine("Fayl: " + yol);
                    sw.WriteLine("Tarix: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine();
                    sw.WriteLine("Sətir sayı: " + setirSayi);
                    sw.WriteLine("Söz sayı: " + sozSayi);
                    sw.WriteLine("Simvol sayı: " + simvolSayi);
                    sw.WriteLine("Ən uzun söz: " + enUzunSoz + " (" + enUzunSoz.Length + " hərf)");
                    sw.WriteLine();
                    sw.WriteLine("Ən çox təkrarlanan " + top + " söz:");
                    for (int i = 0; i < top; i++)
                    {
                        sw.WriteLine((i + 1) + ". " + siyahi[i].Key + " - " + siyahi[i].Value + " dəfə");
                    }
                }

                Console.WriteLine("Analiz tamamlandı. Nəticələr hesabat.txt faylına yazıldı.");
                Console.WriteLine();
                Console.WriteLine(File.ReadAllText("hesabat.txt", Encoding.UTF8));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hesabat yazılarkən xəta: " + ex.Message);
            }
        }
    }
}
