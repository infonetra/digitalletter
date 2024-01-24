using letterhead.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace letterhead.Controllers
{
    public class HomeController : Controller
    {
        letterheadEntities db = new letterheadEntities();
        public ActionResult Dashboard(int? id=0)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == 1)
            {
                TempData["success"] = "Password updated successfully.";                
            }
            if (id == 2)
            {
                TempData["success"] = "Data updated successfully.";
            }
            int role = Convert.ToInt32(Session["userrole"]);
            int userid = Convert.ToInt32(Session["userid"]);
            ViewBag.totalsite = db.Mst_SITE.Where(a => a.ISACTIVE == true).Count();
            ViewBag.totaluser=db.Mst_USER.Where(a=>a.ISACTIVE == true).Count();
            ViewBag.totalatter=db.LatterRequests.Where(a => a.ISACTIVE == true).Count();
            //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENO + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
            var data = new List<latterrvm>();
            var settingdata = db.settings.FirstOrDefault();
            Session["CNAME"] = settingdata.CNAME;
            Session["fname"] = settingdata.FONTNAME;
            Session["fsize"] = settingdata.FONTSIZE;
            if (role==2)
            {
                 data = (from later in db.LatterRequests
                            join user in db.Mst_USER on later.USERID equals user.ID
                            join site in db.Mst_SITE on user.SITEID equals site.ID
                            join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                            where later.USERID == userid
                            select new latterrvm
                            {
                                ID = later.ID,
                                FULLNAME = user.FULLNAME,
                                LATTERNO = later.LATTERNO,
                                LatterData = later.LatterData,
                                Department = dept.DEPARTMENT,
                                DeptID = dept.ID,
                                LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                                REMARK = later.REMARK,
                                TITLE = site.TITLE,
                                SITEID = site.ID,
                                CODE = site.CODE,
                                EMPCODE = user.EMPCODE,
                                SITENO = site.SITENO,
                                SITENONAME = site.SITENONAME,
                                CREATEBY = later.CREATEBY,
                                CRAETEDATE = later.CRAETEDATE,
                                ISACTIVE = later.ISACTIVE
                            }).OrderByDescending(a => a.CRAETEDATE).Take(5).ToList();
            }
            else
            {
                 data = (from later in db.LatterRequests
                            join user in db.Mst_USER on later.USERID equals user.ID
                            join site in db.Mst_SITE on user.SITEID equals site.ID
                            join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID                            
                            select new latterrvm
                            {
                                ID = later.ID,
                                FULLNAME = user.FULLNAME,
                                LATTERNO = later.LATTERNO,
                                LatterData = later.LatterData,
                                Department = dept.DEPARTMENT,
                                DeptID = dept.ID,
                                LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                                REMARK = later.REMARK,
                                TITLE = site.TITLE,
                                SITEID = site.ID,
                                EMPCODE = user.EMPCODE,
                                CODE = site.CODE,
                                SITENO = site.SITENO,
                                SITENONAME = site.SITENONAME,
                                CREATEBY = later.CREATEBY,
                                CRAETEDATE = later.CRAETEDATE,
                                ISACTIVE = later.ISACTIVE
                            }).OrderByDescending(a=>a.CRAETEDATE).Take(5).ToList();
            }
           
            return View(data);
        }


        #region rolemaster
        public ActionResult RoleMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.Mst_Role.ToList();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult AddRole()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddRole(Mst_Role model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    
                    model.CREATEBY = 1;
                    model.CRAETEDATE = DateTime.Now;                   
                    db.Mst_Role.Add(model);
                    db.SaveChanges();                    
                    TempData["success"] = "Role created successfully.";
                    return RedirectToAction("RoleMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult EditRole(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var prd = db.Mst_Role.Where(x => x.ID == id).FirstOrDefault();
                    return View(prd);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditRole(Mst_Role model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var role = db.Mst_Role.Where(x => x.ID == model.ID).FirstOrDefault();
                    role.ID = model.ID;
                    role.ISACTIVE = model.ISACTIVE;
                    role.CREATEBY = model.CREATEBY;
                    role.CRAETEDATE = model.CRAETEDATE;
                    role.TITLE = model.TITLE;                    
                    db.SaveChanges();

                    TempData["success"] = "Role has been updated successfully";

                    return RedirectToAction("RoleMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion


        #region RequestType
        public ActionResult LetterTypeMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.LetterCrateTypes.ToList();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult AddLetterType()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddLetterType(LetterCrateType model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                  
                    model.CREATEDATE = DateTime.Now;
                    db.LetterCrateTypes.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Data created successfully.";
                    return RedirectToAction("LetterTypeMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult EditLetterType(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var prd = db.LetterCrateTypes.Where(x => x.ID == id).FirstOrDefault();
                    return View(prd);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditLetterType(LetterCrateType model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var role = db.LetterCrateTypes.Where(x => x.ID == model.ID).FirstOrDefault();
                    role.ID = model.ID;
                    role.ISACTIVE = model.ISACTIVE;
                    role.CREATEDATE = DateTime.Now;
                    role.TITLE = model.TITLE;
                    db.SaveChanges();

                    TempData["success"] = "Data has been updated successfully";

                    return RedirectToAction("LetterTypeMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion


        #region sitemaster
        public ActionResult SiteMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    //var data = db.Mst_SITE.ToList();
                    var data = db.Mst_SITE.Where(a => a.ISACTIVE == true).ToList();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult AddSite()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddSite(Mst_SITE model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {

                    model.CREATEBY = 1;
                    model.CRAETEDATE = DateTime.Now;
                    db.Mst_SITE.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Project created successfully.";
                    return RedirectToAction("SiteMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult EditSite(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var prd = db.Mst_SITE.Where(x => x.ID == id).FirstOrDefault();
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    return View(prd);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditSite(Mst_SITE model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var site = db.Mst_SITE.Where(x => x.ID == model.ID).FirstOrDefault();
                    site.ID = model.ID;
                    site.TITLE = model.TITLE;
                    site.CODE = model.CODE;
                    site.SITENO = model.SITENO;
                    site.SITENONAME = model.SITENONAME;                   
                    site.ISACTIVE = model.ISACTIVE;
                    site.CREATEBY = model.CREATEBY;
                    site.CRAETEDATE = model.CRAETEDATE;
                  
                    db.SaveChanges();

                    TempData["success"] = "Project has been updated successfully";

                    return RedirectToAction("SiteMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        #endregion

        #region usermaster
        public ActionResult UserMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from user in db.Mst_USER
                                join site in db.Mst_SITE on user.SITEID equals site.ID  
                                join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                                select new uservm
                                {
                                    ID = user.ID,
                                    FULLNAME = user.FULLNAME,
                                    MOBILENO = user.MOBILENO,
                                    EMAILID = user.EMAILID,
                                    TITLE = site.TITLE,
                                    SITENO = site.SITENO,
                                    SITENONAME = site.SITENONAME,
                                    Department=dept.DEPARTMENT,
                                    EMPCODE=user.EMPCODE,
                                    DeptID=dept.ID,
                                    USERNAME=user.USERNAME,
                                    USERPWD=user.USERPWD,
                                    CANLOGIN=user.CANLOGIN,
                                    CREATEBY = user.CREATEBY,
                                    CRAETEDATE = user.CRAETEDATE,
                                    ISACTIVE = user.ISACTIVE
                                }).ToList();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult AddUser()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE +"/"+x.SITENONAME, Value = x.ID.ToString() }).ToList();
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddUser(Mst_USER model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.Mst_USER.Where(a => a.EMPCODE == model.EMPCODE && a.ISACTIVE == true).Count();
                    if (data > 0)
                    {
                        TempData["error"] = "Already assign with another user!";
                        return RedirectToAction("UserMaster");
                    }
                    model.USERPWD = model.EMPCODE;
                    model.OneTimeUser = true;
                    model.CREATEBY = 1;
                    model.ROLEID = 2;
                    model.CANLOGIN = true;
                    model.CRAETEDATE = DateTime.Now;
                    db.Mst_USER.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "User Added successfully.";
                    return RedirectToAction("UserMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult EditUser(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
                    var prd = db.Mst_USER.Where(x => x.ID == id).FirstOrDefault();
                    return View(prd);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditUser(Mst_USER model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var site = db.Mst_USER.Where(x => x.ID == model.ID).FirstOrDefault();
                    site.ID = model.ID;
                    site.FULLNAME = model.FULLNAME;
                    site.USERPWD = model.USERPWD;
                    site.USERNAME = model.USERNAME;
                    site.SITEID = model.SITEID;
                    site.MOBILENO = model.MOBILENO;
                    site.EMAILID = model.EMAILID;
                    site.ROLEID = model.ROLEID;
                    site.CANLOGIN = true;
                    site.ISACTIVE = model.ISACTIVE;
                    site.CREATEBY = model.CREATEBY;
                    site.CRAETEDATE = model.CRAETEDATE;

                    db.SaveChanges();

                    TempData["success"] = "User has been updated successfully";

                    return RedirectToAction("UserMaster");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion



        #region Department
        public ActionResult DepartmentList()
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.Mst104_DEPARTMENT.ToList();
                    return View(data);

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult AddDepartment()
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddDepartment(Mst104_DEPARTMENT model)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var codeexistcheck = db.Mst104_DEPARTMENT.Where(a => a.DEPARTMENT == model.DEPARTMENT).Count();
                    if (codeexistcheck > 0)
                    {
                        TempData["error"] = "Data Allready Exist!";
                        return RedirectToAction("DepartmentList");
                    }
                    model.CREATEBY = 1;
                    model.CREATEDATE = DateTime.Now;
                    db.Mst104_DEPARTMENT.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Department added successfully.";
                    return RedirectToAction("DepartmentList");

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult EditDepartment(int id = 0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {

                    var data = db.Mst104_DEPARTMENT.Where(x => x.ID == id).FirstOrDefault();
                    return View(data);

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditDepartment(Mst104_DEPARTMENT model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.Mst104_DEPARTMENT.Where(a => a.ID == model.ID).FirstOrDefault();
                    //var codeexistcheck = db.Mst104_DEPARTMENT.Where(a => a.DEPARTMENT == data.DEPARTMENT).Count();
                    //if (codeexistcheck > 0)
                    //{
                    //    TempData["error"] = "Data Allready Exist!";
                    //    return RedirectToAction("DepartmentList");
                    //}
                    //data.ID = model.ID;
                    //data.CODE = model.CODE;
                    //data.DEPARTMENT = model.DEPARTMENT;
                    //data.CREATEBY = 1;
                    data.ISACTIVE = model.ISACTIVE;
                    db.SaveChanges();
                    TempData["success"] = "Department Upadted.";
                    return RedirectToAction("DepartmentList");

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion

        #region reports
        public ActionResult Reports()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int role = Convert.ToInt32(Session["userrole"]);
            int userid = Convert.ToInt32(Session["userid"]);
            var userdata = db.Mst_USER.Where(a => a.ISACTIVE == true && a.ID == userid).FirstOrDefault();
            if (role == 1)
            {
                ViewBag.sitedata = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.CODE + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
                ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
            }
            if (role == 2)
            {
                ViewBag.sitedata = (from p in db.Mst_SITE
                                    join u in db.Mst_USER on p.ID equals u.SITEID
                                    where u.ISACTIVE == true && u.ID == userid
                                    select new SelectListItem
                                    {
                                        Text = p.TITLE + "/" + p.CODE + "/" + p.SITENONAME,
                                        Value = p.ID.ToString(),
                                    }).ToList();
                ViewBag.dept = (from p in db.Mst104_DEPARTMENT                                   
                                    join u in db.Mst_USER on p.ID equals u.DEPTID
                                    where u.ISACTIVE == true && u.ID == userid
                                    select new SelectListItem
                                    {
                                        Text = p.DEPARTMENT,
                                        Value = p.ID.ToString()
                                    }).ToList();               
            }
            

            return View();
        }

        public ActionResult GetReportData(int DeptID=0,int LocID=0, string sDate = null, string eDate = null)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int role = Convert.ToInt32(Session["userrole"]);
            int userid = Convert.ToInt32(Session["userid"]);
            var userdata = db.Mst_USER.Where(a => a.ISACTIVE == true && a.ID == userid).FirstOrDefault();
            var data = new List<latterrvm>();
            DateTime startDate = ((sDate != "") ? Convert.ToDateTime(sDate) : DateTime.Now);
            DateTime endDate = ((eDate != "") ? Convert.ToDateTime(eDate) : DateTime.Now);

            if (role == 1)
            {
                data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on user.SITEID equals site.ID
                        join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                        where later.ISACTIVE == true && (LocID != 0 ? site.ID == LocID : later.ID != 0) && (DeptID != 0 ? dept.ID == DeptID : later.ID != 0) && (sDate != "" && eDate != "" ? later.CRAETEDATE >= startDate || later.CRAETEDATE <= endDate : later.ISACTIVE == true)
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                            REMARK = later.REMARK,
                            TITLE = site.TITLE,
                            SITEID = site.ID,
                            CODE = site.CODE,
                            EMPCODE = user.EMPCODE,
                            SITENO = site.SITENO,
                            SITENONAME = site.SITENONAME,
                            CREATEBY = later.CREATEBY,
                            CRAETEDATE = later.CRAETEDATE,
                            ISACTIVE = later.ISACTIVE
                        }).OrderByDescending(a => a.CRAETEDATE).ToList();
                return PartialView("_reportsdata", data);
            }
            if (role == 2)
            {
                data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on user.SITEID equals site.ID
                        join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                        //where later.USERID == userid && later.ISACTIVE==true && (LocID != 0 ? site.ID == LocID : later.ID != 0) && (DeptID != 0 ? dept.ID == DeptID : later.ID != 0) && (sDate != "" && eDate != "" ? later.CRAETEDATE >= startDate || later.CRAETEDATE <= endDate : later.ISACTIVE == true)
                        where later.USERID == userid && later.ISACTIVE == true 
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                            REMARK = later.REMARK,
                            TITLE = site.TITLE,
                            SITEID = site.ID,
                            CODE = site.CODE,
                            EMPCODE = user.EMPCODE,
                            SITENO = site.SITENO,
                            SITENONAME = site.SITENONAME,
                            CREATEBY = later.CREATEBY,
                            CRAETEDATE = later.CRAETEDATE,
                            ISACTIVE = later.ISACTIVE
                        }).OrderByDescending(a => a.CRAETEDATE).ToList();
                return PartialView("_reportsdata", data);
            }
            return RedirectToAction("Reports");
        }
        public ActionResult UserReports()
        {
            return View();
        }

        #endregion

        #region latter request     


        public ActionResult LetterRequest()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    int userid = Convert.ToInt32(Session["userid"]);                   
                    var data = (from later in db.LatterRequests
                                join ltype in db.LetterCrateTypes on later.LetterType equals ltype.ID
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on user.SITEID equals site.ID
                                join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                                where later.USERID == userid
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,   
                                    LATTERNO=later.LATTERNO,
                                    LatterData= later.LatterData,
                                    Department=dept.DEPARTMENT,
                                    DeptID=dept.ID,
                                    LATTERNOSerice= ("HGIEL/" + site.TITLE+"/"+site.SITENONAME+"/"+dept.DEPARTMENT+ "/2023-24/" + later.LATTERNO.ToString()),
                                    REMARK= ltype.TITLE,
                                    TITLE = site.TITLE,
                                    SITEID=site.ID,
                                    CODE=site.CODE,
                                    EMPCODE=user.EMPCODE,
                                    SITENO = site.SITENO,
                                    SITENONAME = site.SITENONAME,                                   
                                    CREATEBY = later.CREATEBY,
                                    CRAETEDATE = later.CRAETEDATE,
                                    ISACTIVE = later.ISACTIVE
                                }).ToList();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult Printwithdate(int id)
        {
            ViewBag.pid = id;
            return View();
        }

        [HttpPost]
        public ActionResult Printwithdate(string pdate,int id)
        {
            return RedirectToAction("performaview", "Home", new { id =id, date = pdate});
        }

        public ActionResult AddLetterRequest()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    ViewBag.ltype = db.LetterCrateTypes.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList(); ;
                    int userid = Convert.ToInt32(Session["userid"]);
                    var date = new DateTime(DateTime.Now.Year, 4, 6);
                    int lastinsert = db.LatterRequests.Where(a => a.USERID == userid).Count();
                    LatterRequest latter = new LatterRequest();
                    latter.LATTERNO = Convert.ToString(lastinsert + 1);
                    return View(latter);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddLetterRequest(LatterRequest model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    model.USERID = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = 1;
                    model.ISACTIVE = true;
                    model.CRAETEDATE = DateTime.Now;
                    db.LatterRequests.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Letter Issued Succesfully";
                    return RedirectToAction("LetterRequest");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        public ActionResult Performa(int id=0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {

                    var data = db.LatterRequests.Where(x => x.ID == id).FirstOrDefault();
                    return View(data);

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Performadata(string latterdata=null,int id=0)
        {
            try
            {                
                    var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();                   
                    data.LatterData = latterdata;
                    db.SaveChanges();
                    TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult printview(int id=0)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = (from later in db.LatterRequests
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on user.SITEID equals site.ID
                                join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                                where later.ID == id
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    Department = dept.DEPARTMENT,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                                    REMARK = later.REMARK,
                                    TITLE = site.TITLE,
                                    SITEID = site.ID,
                                    CODE = site.CODE,
                                    SITENO = site.SITENO,
                                    SITENONAME = site.SITENONAME,
                                    CREATEBY = later.CREATEBY,
                                    CRAETEDATE = later.CRAETEDATE,
                                    ISACTIVE = later.ISACTIVE
                                }).FirstOrDefault();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch(Exception ex)
            {
                return View();
            }
        }
        #endregion
      

        public ActionResult performaview(int id = 0, string date=null)
        {
            Session["datawise"] = null;
            if (date != null)
            {
                DateTime dateTime = DateTime.Parse(date);
                Session["datawise"] = dateTime.ToString("dd-MM-yyyy");
            }
            var data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on user.SITEID equals site.ID
                        join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID
                        where later.ID == id
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + site.SITENONAME + "/" + dept.DEPARTMENT + "/2023-24/" + later.LATTERNO.ToString()),
                            REMARK = later.REMARK,
                            TITLE = site.TITLE,
                            SITEID = site.ID,
                            CODE = site.CODE,
                            SITENO = site.SITENO,
                            SITENONAME = site.SITENONAME,
                            CREATEBY = later.CREATEBY,
                            CRAETEDATE = later.CRAETEDATE,
                            ISACTIVE = later.ISACTIVE
                        }).FirstOrDefault();
            return View(data);
        }
        //public static Byte[] PdfSharpConvert(String html)
        //{
        //    Byte[] res = null;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
        //        pdf.Save(ms);
        //        res = ms.ToArray();
        //    }
        //    return res;
        //}


        public ActionResult Setting()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.settings.Where(a => a.ID == 1).FirstOrDefault();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch(Exception ex)
            {
                return View();
            }                               
        }

        [HttpPost]
        public ActionResult Setting(setting st)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                   var data= db.settings.Where(a => a.ID == st.ID).FirstOrDefault();
                    data.CNAME= st.CNAME;
                    data.FONTSIZE = st.FONTSIZE;
                    data.FONTNAME = st.FONTNAME;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Home", new { id = 2 });
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult ChangePassword()
        {
            try
            {
                if (Session["userid"] != null)
                {
                    int id= Convert.ToInt32(Session["userid"].ToString());
                    var data = db.Mst_USER.Where(a => a.ID == id).FirstOrDefault();
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch(Exception ex)
            {
                return View();
            }
           
        }

        [HttpPost]
        public ActionResult ChangePassword(Mst_USER user)
        {
            var data = db.Mst_USER.Where(a => a.ID == user.ID).FirstOrDefault();
            if(data != null)
            {
                data.USERPWD = user.USERPWD;
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home", new {id=1});
            }
            return RedirectToAction("Dashboard", "Home", new { id = 1 });
        }

    }
}