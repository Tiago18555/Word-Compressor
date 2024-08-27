using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextCompression
{
    public class Program
    {
        private static byte MININUM_REP = 5;
        private static byte MINIMUM_CHAR_NUMBER = 5;
        private static string filePath = "";
        public static bool caseSensitive;

        static void Main(string[] args)
        {
            if (!startUp())
                return;

            string text = File.ReadAllText(filePath);

            string compressedFilePath = filePath.Insert(filePath.LastIndexOf('.'), "_compressed");
            string compressedText = compressText(text, MININUM_REP, MINIMUM_CHAR_NUMBER, caseSensitive);
            File.WriteAllText(compressedFilePath, compressedText);

            Console.WriteLine($"File saved at: {compressedFilePath}");
        }

        public static bool startUp()
        {
            Console.WriteLine("Do you want compression to be case-sensitive? (S/N): ");
            caseSensitive = Console.ReadLine()?.Trim().ToUpper() == "S"; // Atribuir à variável estática

            Console.WriteLine("Enter with the file path below:");
            filePath = Console.ReadLine();

            try
            {
                Console.WriteLine("Compress word with at least how many characters?");
                MINIMUM_CHAR_NUMBER = byte.Parse(Console.ReadLine());

                Console.WriteLine("Compress word with at least how many repetitions?");
                MININUM_REP = byte.Parse(Console.ReadLine());
            }
            catch (FormatException e)
            {
                return false;
            }
            catch (OverflowException e)
            {
                return false;
            }
            catch (ArgumentNullException e)
            {
                return false;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return false;
            }

            Console.WriteLine(caseSensitive);

            return true;
        }

        public static string compressText(string text, int minRep, int minCharNumber, bool caseSensitive)
        {
            Console.WriteLine("Dictionary:");
            Dictionary<string, string> compressionDictionary = new Dictionary<string, string>();

            char[] delimiters = { ' ', ',', '.', ';', ':', '!', '?', '\n', '\r', '\t' };
            List<string> words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!caseSensitive)
                words = words.Select(word => word.ToLower()).ToList();

            words.Sort();

            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (word.Length <= minCharNumber) continue;

                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            int code = 1;

            foreach (var kvp in wordCount)
            {
                if (kvp.Value >= minRep)
                {
                    compressionDictionary[kvp.Key] = "$" + code.ToString();
                    code++;
                }
            }

            foreach (var kvp in compressionDictionary)
            {
                string pattern = $@"\b{Regex.Escape(kvp.Key)}\b";
                text = Regex.Replace(text, pattern, kvp.Value);
            }

            string compressedText = "Dictionary:\n";
            foreach (var kvp in compressionDictionary)
            {
                compressedText += $"{kvp.Value} => {kvp.Key}\n";
            }
            compressedText += "\n" + text;

            return compressedText;
        }
    }
}
