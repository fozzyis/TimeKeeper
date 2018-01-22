using System;
using System.Xml.Serialization;
using System.IO;


namespace TimeKeeperShared
{
    public class EmployeeDataManager
    {
        TimeKeeper timeKeeperDroid = new TimeKeeper();
        
        public bool CheckForSavedData(string filePath, string dirPath, string packagePath)
        {
            try
            {
                try
                {
                    Directory.CreateDirectory(dirPath + "/" + packagePath);

                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Access not granted to storage");
                }
                using (StreamReader reader = new StreamReader(filePath))
                {
                    reader.Close();
                    //reader.Dispose();
                    return true;
                }

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("IT FAILED");
                return false;
            }
            catch
            {

                return false;
            }
        }

        public void SaveEmployee(Employee employeeData, string filePath)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            //string savePath = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //output path to console for error checking
            XmlSerializer serializer =
            new XmlSerializer(typeof(Employee));
            TextWriter writer = new StreamWriter(filePath);
            Employee employee = new Employee();

            // Create employee and site details.
            employee.employeeName = employeeData.employeeName;
            employee.employeeID = employeeData.employeeID;
            employee.employeeSite = employeeData.employeeSite;
            employee.employeeWorkgroup = employeeData.employeeWorkgroup;
            employee.workgroupScheduler = employeeData.workgroupScheduler;
            employee.schedulerEmail = employeeData.schedulerEmail;
            employee.workgroupTeamLeader = employeeData.workgroupTeamLeader;
            employee.teamleaderEmail = employeeData.teamleaderEmail;

            // Set last time modified to error check.
            employee.lastTimeModified = DateTime.Now.ToLongDateString();
            //Console.WriteLine(documentsPath + "/" + EMPLOYEE_DATA_FILE); //todo remove

            // Serialize the employee data and close the TextWriter.
            serializer.Serialize(writer, employee);
            writer.Close();

        }

        public Employee LoadEmployee(string filePath)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(Employee));
            /* If the XML document has been altered with unknown 
            nodes or attributes, handle them with the 
            UnknownNode and UnknownAttribute events.*/
            serializer.UnknownNode += new
            XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new
            XmlAttributeEventHandler(serializer_UnknownAttribute);

            //StreamReader reader = new StreamReader(documentsPath + "/" + EMPLOYEE_DATA_FILE);


            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            // A FileStream is needed to read the XML document.

            Console.WriteLine("It saved");
            // Declare an object variable of the type to be deserialized.
            Employee employee;
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            employee = (Employee)serializer.Deserialize(fileStream);
            // Read the order date.

            Console.WriteLine("Last Date modified: " + employee.lastTimeModified);

            // Read the shipping address.

            DisplayEmployeeDetails(employee);
            fileStream.Dispose();
            //reader.Close();
            return employee;


        }

        protected void DisplayEmployeeDetails(Employee employee)
        {
            Console.WriteLine(employee.employeeName);
            Console.WriteLine("\t" + employee.employeeID);
            Console.WriteLine("\t" + employee.employeeSite);
            Console.WriteLine("\t" + employee.employeeWorkgroup);
            Console.WriteLine("\t" + employee.workgroupScheduler);
            Console.WriteLine("\t" + employee.schedulerEmail);
            Console.WriteLine("\t" + employee.workgroupTeamLeader);
            Console.WriteLine("\t" + employee.teamleaderEmail);
            Console.WriteLine();
        }

        private void serializer_UnknownNode
        (object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }
    }
}
