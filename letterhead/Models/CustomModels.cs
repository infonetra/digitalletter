using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace letterhead.Models
{
    public class uservm
    {
        public int ID { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILENO { get; set; }
        public string EMAILID { get; set; }
        public string EMPCODE { get; set; }
        public string USERNAME { get; set; }
        public int DeptID { get; set; }
        public string Department { get; set; }
        public string USERPWD { get; set; }
        public Nullable<int> SITEID { get; set; }
        public Nullable<int> ROLEID { get; set; }
        public Nullable<bool> CANLOGIN { get; set; }
        public string TITLE { get; set; }
        public string CODE { get; set; }
        public Nullable<int> SITENO { get; set; }
        public string SITENONAME { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }
        public string LATTERNO { get; set; }
        public string LATTERNOSerice { get; set; }
        public string REMARK { get; set; }
    }

    public class SubDeptVM
    {
        public int ID { get; set; }
        public Nullable<int> DEPTID { get; set; }
        public string DEPARTMENT { get; set; }
        public string SubDEPARTMENT { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }
    }
    public class SPVvm
    {
        public int ID { get; set; }
        public string empcode { get; set; }
        public string username { get; set; }
        public string spvtitle { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }

    }
    public class DEPTvm
    {
        public int ID { get; set; }
        public string empcode { get; set; }
        public string username { get; set; }
        public string department { get; set; }
        public string DeptName { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }

    }

    public class Locvm
    {
        public int ID { get; set; }
        public string empcode { get; set; }
        public string username { get; set; }
        public string Location { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }

    }

    public class latterrvm
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string LATTERNO { get; set; }
        public string EMPCODE { get; set; }
        public string LATTERNOSerice { get; set; }
        public string LatterData { get; set; }
        public string FinanceYear { get; set; }
        public string REMARK { get; set; }
        public string FULLNAME { get; set; }     
        public Nullable<int> SITEID { get; set; }
        public Nullable<int> ROLEID { get; set; }  
        public string TITLE { get; set; }
        public string CODE { get; set; }
        public Nullable<int> SITENO { get; set; }
        public string SITENONAME { get; set; }
        public int DeptID { get; set; }
        public string Department { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusData { get; set; }
        public Nullable<bool> isapv { get; set; }
        public Nullable<int> spvid { get; set; }
        public string svptitle { get; set; }
    }

    public class LogRequestVM
    {
        public int ID { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public string createbyname { get; set; }
        public string createdates { get; set; }

        public Nullable<System.DateTime> CRAETEDATE { get; set; }

    }

    public class sitevm
    {
        public int ID { get; set; }
        public string TITLE { get; set; }
        public string CODE { get; set; }
        public Nullable<int> SITENO { get; set; }
        public string SITENONAME { get; set; }
        public Nullable<int> DeptID { get; set; } 
        public string Department { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public Nullable<int> CREATEBY { get; set; }
        public Nullable<System.DateTime> CRAETEDATE { get; set; }
    }
}