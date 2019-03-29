using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Core.Objects;
using Newtonsoft.Json;

namespace Parsers
{
    public class JSONParser<T> : IParser<T>
    {
        public List<T> ParseFile<T>(string filePath, ref List<ErrorMessage> errorMessages) where T : new()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File does not exist.");
                }

                return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            }
            catch (FileNotFoundException ex)
            {//file not found exceptions
                errorMessages.Add(new ErrorMessage(ex.ToString(), true));
                return null;
            }
            catch (Exception ex)
            {//general error exceptions
                errorMessages.Add(new ErrorMessage(ex.ToString(), true));
                return null;
            }
        }
    }
}
