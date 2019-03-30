using Core.Objects;
using System;
using System.Collections.Generic;

namespace Parsers
{
    public class FileParser<T>
    {
        private readonly IParser<T> Parser;

        public FileParser(IParser<T> parser)
        {
            Parser = parser ?? throw new ArgumentNullException("parser");
        }

        public List<T> ParseFile<T>(string filePath, ref List<ErrorMessage> errorMessages) where T : new()
        {
            return Parser.ParseFile<T>(filePath, ref errorMessages);
        }
    }
}
