using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Core.Objects;

namespace Parsers
{
    public class CSVParser<T> : IParser<T>
    {
        public List<T> ParseFile<T>(string filePath, List<ErrorMessage> errorMessages) where T : new ()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File does not exist.");
                }

                var newObject = new T();
                var listNewObject = new List<T>();
                var readFile = new StreamReader(File.OpenRead(filePath));
                int lineCount = 1; //start at 1 to skip header
                var getHeaders = readFile.ReadLine().Split(','); //Read headers
                // List to store products.
                while (!readFile.EndOfStream)
                {
                    var line = readFile.ReadLine(); //read each line
                    //and split each line to get the values of the respective headers
                    var delimiter = new char[','];
                    var values = line.Split(',');
                    int i = 0;
                    foreach (String val in values)
                    {
                        //Get the header
                        var currHeader = getHeaders[i].ToString();
                        //find the object property -- note I am removing spaces and ToUpper and Trimming the file column headers
                        var ojbProperty =
                            newObject.GetType()
                                .GetProperties()
                                .Where(t => t.Name.ToUpper() == currHeader.Replace(" ", "").ToUpper().Trim())
                                .FirstOrDefault();
                        try
                        {
                            //try to cast the value to the current object property using reflection
                            ojbProperty.SetValue(newObject, Convert.ChangeType(val, ojbProperty.PropertyType));
                        }
                        catch (Exception)
                        {
                            errorMessages.Add(new ErrorMessage(String.Format("An error occurred on line {0} property ({1}) value ({2}) is invalid. This property will be skipped.", lineCount.ToString(), ojbProperty.Name, val)));
                        }

                        i++;
                    }
                    //add new obj to list and reset
                    listNewObject.Add(newObject);
                    newObject = new T();
                    lineCount++;
                }


                return listNewObject;
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
