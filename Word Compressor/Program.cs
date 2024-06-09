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
            // Passo 1: Perguntar sobre Case-Sensitive
            Console.WriteLine("Deseja que a compressão seja case-sensitive? (S/N): ");
            bool caseSensitive = Console.ReadLine().Trim().ToUpper() == "S";

            // Passo 2: Ler o arquivo de texto e colocar as palavras em um conjunto de dados
            Console.WriteLine("Digite o caminho do arquivo de texto:");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Arquivo não encontrado.");
                return;
            }

            string text = File.ReadAllText(filePath);
            char[] delimiters = { ' ', ',', '.', ';', ':', '!', '?', '\n', '\r', '\t' };
            List<string> words = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Passo 3: Organizar em ordem alfabética e aplicar Case-Sensitivity se necessário
            if (!caseSensitive)
            {
                words = words.Select(word => word.ToLower()).ToList();
            }
            words.Sort();

            // Passo 4: Contagem das palavras
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (word.Length <= 3) continue; // Ignorar palavras com 3 caracteres ou menos

                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            // Passo 5: Criar um dicionário para as palavras que se repetiram mais de 3 vezes
            Dictionary<string, string> compressionDictionary = new Dictionary<string, string>();
            int code = 1;

            foreach (var kvp in wordCount)
            {
                if (kvp.Value > 5)
                {
                    compressionDictionary[kvp.Key] = "$" + code.ToString();
                    code++;
                }
            }

            // Passo 6: Substituição no texto usando Regex para garantir substituição de palavras inteiras
            foreach (var kvp in compressionDictionary)
            {
                string pattern = $@"\b{Regex.Escape(kvp.Key)}\b";
                text = Regex.Replace(text, pattern, kvp.Value);
            }

            // Construir o texto comprimido com o dicionário no início
            string compressedText = "Dicionário de Compressão:\n";
            foreach (var kvp in compressionDictionary)
            {
                compressedText += $"{kvp.Value} => {kvp.Key}\n";
            }
            compressedText += "\n" + text;

            // Salvar o texto comprimido em um novo arquivo
            string compressedFilePath = filePath.Insert(filePath.LastIndexOf('.'), "_compressed");
            File.WriteAllText(compressedFilePath, compressedText);

            Console.WriteLine("Dicionário de Compressão:");
            foreach (var kvp in compressionDictionary)
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }

            Console.WriteLine($"Texto comprimido salvo em: {compressedFilePath}");
        }
    }
}
