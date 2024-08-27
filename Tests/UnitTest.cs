using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace TextCompression.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void CompressText_ShouldCompressCorrectly()
        {
            string text = "hello hello world world world example example";
            int minRep = 2;
            int minCharNumber = 4;
            bool caseSensitive = false;

            string result = Program.compressText(text, minRep, minCharNumber, caseSensitive);

            string expectedDictionary = "Dictionary:\n$1 => example\n$2 => hello\n$3 => world\n";
            string expectedCompressedText = $"{expectedDictionary}\n$2 $2 $3 $3 $3 $1 $1";
            Assert.Equal(expectedCompressedText, result);
        }

        [Fact]
        public void CompressText_ShouldHandleCaseSensitivity()
        {
            string text = "Hello hello HELLO world WORLD";
            int minRep = 1;
            int minCharNumber = 1;
            bool caseSensitive = true;

            string result = Program.compressText(text, minRep, minCharNumber, caseSensitive);

            string expectedDictionary = "Dictionary:\n$1 => hello\n$2 => Hello\n$3 => HELLO\n$4 => world\n$5 => WORLD\n";
            string expectedCompressedText = $"{expectedDictionary}\n$2 $1 $3 $4 $5";
            Assert.Equal(expectedCompressedText, result);
        }

        [Fact]
        public void CompressText_ShouldReturnOriginalTextIfNoWordsMeetCriteria()
        {
            string text = "hi hi hi";
            int minRep = 4;
            int minCharNumber = 2;
            bool caseSensitive = false;

            string result = Program.compressText(text, minRep, minCharNumber, caseSensitive);

            string expectedDictionary = "Dictionary:\n";
            string expectedCompressedText = $"{expectedDictionary}\nhi hi hi";
            Assert.Equal(expectedCompressedText, result);
        }
    }
}
