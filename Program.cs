using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        // This function to load a xml file that written manually 
        //The Ids are not sorted and cities are unique values
        static void XmlFile()
        {
            try
            {
                XmlDocument Xdoc = new XmlDocument();
                Xdoc.Load("People.xml");
                Console.WriteLine("XML file loaded successfully.");
            }
            catch (IOException ex)  // Handles file-related errors
            {
                Console.WriteLine($"Error: Unable to access 'People.xml'. {ex.Message}");
            }
            catch (System.Xml.XmlException ex)  // Handles XML structure errors
            {
                Console.WriteLine($"Error: Invalid XML format. {ex.Message}");
            }
            catch (Exception ex)  // Catches any other unexpected errors
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }


        // this function to sort the xml file that includes all data 
        static public void XSortedById()
        {
            try
            {
                XDocument XDoc1 = XDocument.Load("People.xml");
                XDocument XDoc2 = new XDocument(
                    new XElement("Persons",
                        from node in XDoc1.Root.Elements("Person")
                        let idElement = node.Element("Id")
                        where idElement != null
                        orderby (int)idElement
                        select node
                    )
                );

                XDoc2.Save("People.xml");
                Console.WriteLine("XML sorted by Id and saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sorting XML: {ex.Message}");
            }
        }

        //this function to spilit the xml file to 
        static public void SplitXmlFile()
        {
            try
            {
                XmlDocument Xdoc = new XmlDocument();
                Xdoc.Load("People.xml");
                XmlNodeList Xln = Xdoc.GetElementsByTagName("Person");

                int fileCount = 1;
                for (int i = 0; i < Xln.Count; i += 10)
                {
                    string filePath = $"PersonData{fileCount}.xml";
                    using (XmlWriter writer = XmlWriter.Create(filePath))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Persons");

                        for (int j = i; j < i + 10 && j < Xln.Count; j++)
                        {
                            writer.WriteStartElement("Person");

                            foreach (XmlNode child in Xln[j].ChildNodes)
                            {
                                writer.WriteElementString(child.Name, child.InnerText);
                            }

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }

                    fileCount++;
                }
                Console.WriteLine("XML files split successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error splitting XML: {ex.Message}");
            }

        }


        // this function to search by city 
        static public void SearchByCity()
        {
            Console.Write("City Name: ");
            string cityName = Console.ReadLine();

            XmlDocument Xdoc = new XmlDocument();
            Xdoc.Load("People.xml");
            XmlNodeList Xln = Xdoc.GetElementsByTagName("Person");

            int iterationCount = 0;
            bool found = false;

            foreach (XmlNode person in Xln)
            {
                iterationCount++; // Count each loop iteration
                XmlNodeList details = person.ChildNodes;

                if (details.Count >= 3 && details[2].InnerText.Equals(cityName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Id: {details[0].InnerText}");
                    Console.WriteLine($"Name: {details[1].InnerText}");
                    Console.WriteLine($"City: {details[2].InnerText}");
                    Console.WriteLine($"Country: {details[3].InnerText}");
                    Console.WriteLine($"Iterations to find result: {iterationCount}");
                    Console.WriteLine("------------------------------------------------------------------------------");
                    found = true;
                    break; // Stop searching once found
                }
            }

            if (!found)
                Console.WriteLine($"Error! City not found after {iterationCount} iterations. Please try again.");
        }

        // This function to search by Id
        static public void SearchById()
        {
            Console.Write("Enter Id: ");
            if (!int.TryParse(Console.ReadLine(), out int ID))
            {
                Console.WriteLine("Invalid input. Please enter a numeric Id.");
                return;
            }

            string filePath = ID <= 10 ? "PersonData1.xml" : ID <= 20 ? "PersonData2.xml" : null;

            if (filePath == null)
            {
                Console.WriteLine("Error! ID not found.");
                return;
            }

            XmlDocument Xdoc = new XmlDocument();
            Xdoc.Load(filePath);
            XmlNodeList Xln = Xdoc.GetElementsByTagName("Person");

            int iterationCount = 0;
            bool found = false;

            foreach (XmlNode person in Xln)
            {
                iterationCount++; // Count each loop iteration
                XmlNodeList details = person.ChildNodes;

                if (details.Count >= 1 && int.TryParse(details[0].InnerText, out int personId) && personId == ID)
                {
                    Console.WriteLine($"Id: {details[0].InnerText}");
                    Console.WriteLine($"Name: {details[1].InnerText}");
                    Console.WriteLine($"City: {details[2].InnerText}");
                    Console.WriteLine($"Country: {details[3].InnerText}");
                    Console.WriteLine($"Iterations to find result: {iterationCount}");
                    Console.WriteLine("------------------------------------------------------------------------------");
                    found = true;
                    break; // Stop searching once found
                }
            }

            if (!found)
                Console.WriteLine($"Error! ID not found after {iterationCount} iterations.");

        }
        static void Main(string[] args)
        {
            try
            {
                XmlFile();
                XSortedById();
                SplitXmlFile();

                Console.WriteLine("Press 1 if you want to search by Id");
                Console.WriteLine("Press 2 if you want to search by City");
                Console.Write("Enter Your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                    return;
                }

                Console.WriteLine("------------------------------------------------------------------------------");

                switch (choice)
                {
                    case 1:
                        SearchById();
                        break;
                    case 2:
                        SearchByCity();
                        break;
                    default:
                        Console.WriteLine("Error!! Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
