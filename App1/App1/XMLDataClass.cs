using System.Xml.Serialization;


namespace TimeKeeperShared
{
    [XmlRoot(Namespace = "DMSS_TimeKeeper", ElementName = "EmployeeDetails", DataType = "string", IsNullable = true)]
    public class Employee
    {

        public string employeeName;
        public string employeeID;
        public string employeeSite;
        public string employeeWorkgroup;
        public string workgroupScheduler;
        public string schedulerEmail;
        public string workgroupTeamLeader;
        public string teamleaderEmail;
        public string lastTimeModified;
    }

    //public class SiteInfo
    //{
    //    /* The XmlAttribute instructs the XmlSerializer to serialize the Name
    //       field as an XML attribute instead of an XML element (the default
    //       behavior). */
    //    [XmlAttribute]
    //    public string siteName;
    //    public string siteWorkgroup;

    //    [XmlElementAttribute(IsNullable = false)]
    //    public string workgroupScheduler;
    //    [XmlElementAttribute(IsNullable = false)]
    //    public string schedulerEmail;
    //    public string workgroupTeamLeader;
    //    public string teamleaderEmail;

    //}

}

