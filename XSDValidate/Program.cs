using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XSDValidate
{
    class Program
    {
        static void Main(string[] args)
        {

            try {
                bool retry = true;

                do
                {
                    Console.WriteLine();

                    Console.Write("Path of XSD file: ");
                    string xsdPath = Console.ReadLine();

                    if (!checkFile(xsdPath)) continue;

                    Console.Write("Path of XML file: ");
                    string xmlPath = Console.ReadLine();

                    if (!checkFile(xmlPath)) continue;

                    validate(xsdPath, xmlPath);

                } while (retry);

            }
            catch (Exception e) {
                printException(e);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

        }

        /// <summary>
        /// Validate XMLS file against XSD schema
        /// </summary>
        /// <param name="xsdFile">XSD file</param>
        /// <param name="xmlFile">XML file</param>
        /// <returns>True if is a valid XML</returns>
        private static bool validate(string xsdFile, string xmlFile)
        {
            try
            {
                XmlSchema xmlSchema;
                XDocument xml;

                Console.WriteLine("Checking...");

                Console.WriteLine(string.Format("\tReading XSD file...", xsdFile));

                using (XmlTextReader readerXSD = new XmlTextReader(xsdFile))
                    xmlSchema = XmlSchema.Read(readerXSD, ValidationCallback);

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(xmlSchema);

                Console.WriteLine(string.Format("\tReading XML file...", xmlFile));

                using (XmlTextReader readerXML = new XmlTextReader(xmlFile))
                    xml = XDocument.Load(readerXML);

                Console.WriteLine(string.Format("\tValidating...", xmlFile));

                bool error = false;

                xml.Validate(schemaSet, (o, e) => {
                    printError(string.Format("\t{0}", e.Message));
                    error = true;
                });

                Console.WriteLine(string.Format("\t{0}", error ? "Invalid" : "Valid"));
            }
            catch (Exception e)
            {
                printError(e.Message);
            }

            return false;
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("\tWARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("\tERROR: ");

            Console.WriteLine(args.Message);
        }

        /// <summary>
        /// Check if is a valid file path
        /// </summary>
        /// <param name="path">Path to validate</param>
        /// <returns>True is path is valid</returns>
        private static bool checkFile(string path)
        {
            try
            {
                if (path == null || path.Trim().Equals(""))
                    throw new Exception("Path must be informed!");

                if (!File.Exists(path))
                    throw new Exception(string.Format("File '{0}' not exist!", path));

                return true;
            }
            catch (Exception e)
            {
                printError(e.Message);
            }

            return false;
        }

        /// <summary>
        /// Print an exception details
        /// </summary>
        /// <param name="e">Exception to print</param>
        private static void printException(Exception e)
        {
            var corAnt = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("*******************************************************");
            Console.WriteLine(string.Format("Erro: {0}", e.Message));
            Console.WriteLine(e.StackTrace);
            Console.WriteLine("*******************************************************");
            Console.ForegroundColor = corAnt;
        }

        /// <summary>
        /// Print an message error
        /// </summary>
        /// <param name="errorMessage">Message to print</param>
        private static void printError(string errorMessage)
        {
            var corAnt = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(string.Format("\t! {0}", errorMessage));

            Console.ForegroundColor = corAnt;
        }
    }
}
