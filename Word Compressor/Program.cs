using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            byte MININUM_REP = 5;
            byte MINIMUM_CHAR_NUMBER = 5;
            Console.WriteLine("Do you want compression to be case-sensitive? (S/N): ");
            bool caseSensitive = Console.ReadLine().Trim().ToUpper() == "S";

            Console.WriteLine("Enter with the file path below:");
            string filePath = Console.ReadLine();

            Console.WriteLine("Compress word with at least how many characters?");
            MINIMUM_CHAR_NUMBER = byte.Parse(Console.ReadLine());

            Console.WriteLine("Compress word with at least how many repetitions?");
            MININUM_REP = byte.Parse(Console.ReadLine());

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            string text = File.ReadAllText(filePath);
            char[] delimiters = { ' ', ',', '.', ';', ':', '!', '?', '\n', '\r', '\t' };
            List<string> words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!caseSensitive)            
                words = words.Select(word => word.ToLower()).ToList();
            
            words.Sort();

            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (word.Length <= MINIMUM_CHAR_NUMBER) continue;

                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            Dictionary<string, string> compressionDictionary = new Dictionary<string, string>();
            int code = 1;

            foreach (var kvp in wordCount)
            {
                if (kvp.Value > MININUM_REP)
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

            string compressedFilePath = filePath.Insert(filePath.LastIndexOf('.'), "_compressed");
            File.WriteAllText(compressedFilePath, compressedText);

            Console.WriteLine("Dictionary:");
            foreach (var kvp in compressionDictionary)
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }

            Console.WriteLine($"File saved at: {compressedFilePath}");
        }
    }
}
