using Core.Objects;
using System;
using System.Collections.Generic;

namespace Parsers
{
    public class FileParser<T>
    {
        private readonly IParser<T> _parser;

        public FileParser(IParser<T> parser)
        {
            _parser = parser ?? throw new ArgumentNullException("parser");
        }

        public List<T> ParseFile<T>(string filePath, List<ErrorMessage> errorMessages) where T : new()
        {
            return _parser.ParseFile<T>(filePath, errorMessages);
        }
    }
}
