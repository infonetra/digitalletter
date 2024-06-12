using letterhead.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace letterhead.Controllers
{
    public class HomeController : Controller
    {
        letterheadEntities db = new letterheadEntities();
        EmailTemplate etemp = new EmailTemplate();
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
            int spvletter = db.AssignSPVs.Where(a => a.UserID == userid).Count();
            if (spvletter >= 1)
            {
                Session["spvshow"] = "1";
            }
            else
            {
                Session["spvshow"] = "0";
            }
           
            //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENO + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
            var data = new List<latterrvm>();
            var settingdata = db.settings.FirstOrDefault();
            Session["CNAME"] = settingdata.CNAME;
            Session["fname"] = settingdata.FONTNAME;
            Session["fsize"] = settingdata.FONTSIZE;
            if (role==2)
            {
                ViewBag.totalsite = db.LocationAssignUsers.Where(a => a.IsActive == true && a.USERID== userid).Count();
                ViewBag.totaluser = db.Mst_USER.Where(a => a.ISACTIVE == true).Count();
                ViewBag.totalatter = db.LatterRequests.Where(a => a.ISACTIVE == true && a.USERID==userid).Count();
                data = (from later in db.LatterRequests
                            join user in db.Mst_USER on later.USERID equals user.ID
                            join site in db.Mst_SITE on later.LocID equals site.ID
                            join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                            join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                            where later.USERID == userid
                            select new latterrvm
                            {
                                ID = later.ID,
                                FULLNAME = user.FULLNAME,
                                LATTERNO = later.LATTERNO,
                                LatterData = later.LatterData,
                                Department = dept.DEPARTMENT,                               
                                DeptID = dept.ID,
                                LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
                ViewBag.totalsite = db.Mst_SITE.Where(a => a.ISACTIVE == true).Count();
                ViewBag.totaluser = db.Mst_USER.Where(a => a.ISACTIVE == true).Count();
                ViewBag.totalatter = db.LatterRequests.Where(a => a.ISACTIVE == true).Count();
                data = (from later in db.LatterRequests
                            join user in db.Mst_USER on later.USERID equals user.ID
                            join site in db.Mst_SITE on later.LocID equals site.ID
                            join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                            join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID                           
                            select new latterrvm
                            {
                                ID = later.ID,
                                FULLNAME = user.FULLNAME,
                                LATTERNO = later.LATTERNO,
                                LatterData = later.LatterData,
                                Department = dept.DEPARTMENT,
                                DeptID = dept.ID,
                                LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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




        public ActionResult SPVLetterRequest()
        {

            return View();
        }

        #region AssignSPV

        public ActionResult AssignSPVList()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from assign in db.AssignSPVs                               
                                join user in db.Mst_USER on assign.UserID equals user.ID
                                join spv in db.spvmasters on assign.SpvID equals spv.ID                                                              
                                select new SPVvm
                                {
                                    ID = assign.ID, 
                                    username=user.FULLNAME,
                                    empcode=user.EMPCODE,
                                    spvtitle=spv.TITLE,
                                    CREATEBY = assign.CREATEBY,
                                    CRAETEDATE = assign.CREATEDATE,
                                    ISACTIVE=assign.IsActive
                                }).ToList().OrderBy(a=>a.username);
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
        public ActionResult AssignSPVAdd()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.spv = db.spvmasters.Where(x => x.IsActive == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID==2).Select(x => new SelectListItem { Text = x.FULLNAME+"/ "+x.EMPCODE, Value = x.ID.ToString() }).ToList();
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
        public ActionResult AssignSPVAdd(AssignSPV model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.spv = db.spvmasters.Where(x => x.IsActive == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = userid;
                    model.CREATEDATE = DateTime.Now;
                    db.AssignSPVs.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Assigned successfully.";
                    return RedirectToAction("AssignSPVList");
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

        public ActionResult AssignSPVEdit(int id)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.spv = db.spvmasters.Where(x => x.IsActive == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.AssignSPVs.Where(a=>a.ID==id).FirstOrDefault();
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
        public ActionResult AssignSPVEdit(AssignSPV model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.spv = db.spvmasters.Where(x => x.IsActive == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.AssignSPVs.Where(a => a.ID == model.ID).FirstOrDefault();  
                    data.UserID=model.UserID;
                    data.SpvID = model.SpvID;
                    db.SaveChanges();
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

        #endregion

        #region SPVMaster
        public ActionResult SPVMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = db.spvmasters.ToList();
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

        public ActionResult AddSPV()
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
        public ActionResult AddSPV(spvmaster model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {

                    model.CREATEBY = 1;
                    model.CREATEDATE = DateTime.Now;
                    db.spvmasters.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "SPV created successfully.";
                    return RedirectToAction("SPVMaster");
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

        public ActionResult EditSPV(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var prd = db.spvmasters.Where(x => x.ID == id).FirstOrDefault();
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
        public ActionResult EditSPV(spvmaster model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var role = db.spvmasters.Where(x => x.ID == model.ID).FirstOrDefault();
                    role.ID = model.ID;
                    role.IsActive = model.IsActive;
                    role.CREATEBY = model.CREATEBY;
                    role.CREATEDATE = model.CREATEDATE;
                    role.TITLE = model.TITLE;
                    role.StartName = model.StartName;
                    db.SaveChanges();

                    TempData["success"] = "SPV has been updated successfully";

                    return RedirectToAction("SPVMaster");
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
                                //join site in db.Mst_SITE on user.SITEID equals site.ID  
                                //join dept in db.Mst104_DEPARTMENT on user.DEPTID equals dept.ID                               
                                select new uservm
                                {
                                    ID = user.ID,
                                    FULLNAME = user.FULLNAME,
                                    MOBILENO = user.MOBILENO,
                                    EMAILID = user.EMAILID,
                                    //TITLE = site.TITLE,
                                    //SITENO = site.SITENO,
                                    //SITENONAME = site.SITENONAME,
                                   // Department=dept.DEPARTMENT,
                                    EMPCODE=user.EMPCODE,
                                   // DeptID=dept.ID,
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
                    int UserID = Convert.ToInt32(Session["userid"].ToString());
                    //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE +"/"+x.SITENONAME, Value = x.ID.ToString() }).ToList();
                    //ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.IsApprover==1).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
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
                    model.IsApprover = 0;
                    model.CANLOGIN = true;
                    model.CRAETEDATE = DateTime.Now;
                    db.Mst_USER.Add(model);
                    db.SaveChanges();

                    //DeptAssignUser duser =new DeptAssignUser();
                    //duser.USERID = model.ID;
                    //duser.DEPTID = model.DEPTID;
                    //duser.CREATEBY = 1;
                    //duser.CREATEDATE = DateTime.Now;
                    //db.DeptAssignUsers.Add(duser);
                    //db.SaveChanges();
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
                    int UserID = Convert.ToInt32(Session["userid"].ToString());
                    //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
                   // ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.IsApprover== 1).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
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
                    site.EMPCODE = model.EMPCODE;                  
                    site.FULLNAME = model.FULLNAME;
                    site.USERPWD = model.USERPWD;
                    site.USERNAME = model.USERNAME;
                    site.SITEID = model.SITEID;
                    site.MOBILENO = model.MOBILENO;
                    site.EMAILID = model.EMAILID;
                    site.ROLEID = model.ROLEID;                   
                    site.Approver = model.Approver;
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

        #region approver

        public ActionResult ApproverMaster()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from user in db.Mst_USER                              
                                where user.IsApprover == 1
                                select new uservm
                                {
                                    ID = user.ID,
                                    FULLNAME = user.FULLNAME,
                                    MOBILENO = user.MOBILENO,
                                    EMAILID = user.EMAILID,
                                  
                                    EMPCODE = user.EMPCODE,
                                  
                                    USERNAME = user.USERNAME,
                                    USERPWD = user.USERPWD,
                                    CANLOGIN = user.CANLOGIN,
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

        public ActionResult AddApprover()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    int UserID = Convert.ToInt32(Session["userid"].ToString());
                    //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
                    //ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;
                    
                    // ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.IsApprover == 1).Select(x => new SelectListItem { Text = x.FULLNAME, Value = x.ID.ToString() }).ToList();
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
        public ActionResult AddApprover(Mst_USER model)
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
                        return RedirectToAction("ApproverMaster");
                    }
                    model.USERPWD = model.EMPCODE;
                    model.OneTimeUser = true;
                    model.CREATEBY = 1;
                    model.ROLEID = 2;
                    model.IsApprover = 1;                    
                    model.CANLOGIN = true;
                    model.CRAETEDATE = DateTime.Now;
                    db.Mst_USER.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Approver Added successfully.";
                    return RedirectToAction("ApproverMaster");
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

        public ActionResult EditApprover(int id = 0)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    int UserID = Convert.ToInt32(Session["userid"].ToString());
                    //ViewBag.SITE = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE + "/" + x.SITENONAME, Value = x.ID.ToString() }).ToList();
                    //ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList(); ;                   
                    //ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.IsApprover == 1).Select(x => new SelectListItem { Text = x.FULLNAME, Value = x.ID.ToString() }).ToList();
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
        public ActionResult EditApprover(Mst_USER model)
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
                    site.IsApprover = model.IsApprover;
                    site.Approver = model.Approver;
                    site.CANLOGIN = true;
                    site.ISACTIVE = model.ISACTIVE;
                    site.CREATEBY = model.CREATEBY;
                    site.CRAETEDATE = model.CRAETEDATE;

                    db.SaveChanges();

                    TempData["success"] = "Approver has been updated successfully";

                    return RedirectToAction("ApproverMaster");
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




        #region SubDepartment
        public ActionResult SubDepartmentList()
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from subdep in db.Mst_SUBDEPARTMENT
                                join dep in db.Mst104_DEPARTMENT on subdep.DeptID equals dep.ID
                                where subdep.ISACTIVE == true
                                select new SubDeptVM
                                {
                                    ID = subdep.ID,
                                    DEPTID= subdep.DeptID,
                                    DEPARTMENT=dep.DEPARTMENT,
                                    SubDEPARTMENT=subdep.SubDEPARTMENT,                                
                                    CREATEBY = subdep.CREATEBY,
                                    CRAETEDATE = subdep.CREATEDATE,
                                    ISACTIVE = subdep.ISACTIVE
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

        public ActionResult AddSubDepartment()
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.Department = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT , Value = x.ID.ToString() }).ToList();
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
        public ActionResult AddSubDepartment(Mst_SUBDEPARTMENT model)
        {

            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.Department = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    var codeexistcheck = db.Mst_SUBDEPARTMENT.Where(a => a.SubDEPARTMENT == model.SubDEPARTMENT&& a.ISACTIVE==true).Count();
                    if (codeexistcheck > 0)
                    {
                        TempData["error"] = "Data Allready Exist!";
                        return RedirectToAction("SubDepartmentList");
                    }
                    model.CREATEBY = 1;
                    model.CREATEDATE = DateTime.Now;
                    db.Mst_SUBDEPARTMENT.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Sub Department added successfully.";
                    return RedirectToAction("SubDepartmentList");

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

        public ActionResult EditSubDepartment(int id = 0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.Department = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    var data = db.Mst_SUBDEPARTMENT.Where(x => x.ID == id).FirstOrDefault();
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
        public ActionResult EditSubDepartment(Mst_SUBDEPARTMENT model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.Department = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    var data = db.Mst_SUBDEPARTMENT.Where(a => a.ID == model.ID).FirstOrDefault();                    
                    data.ISACTIVE = model.ISACTIVE;
                    data.SubDEPARTMENT = model.SubDEPARTMENT;                   
                    db.SaveChanges();
                    TempData["success"] = "Sub Department Upadted.";
                    return RedirectToAction("SubDepartmentList");

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
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                        where later.ISACTIVE == true && (LocID != 0 ? site.ID == LocID : later.ID != 0) && (DeptID != 0 ? dept.ID == DeptID : later.ID != 0) && (sDate != "" && eDate != "" ? later.CRAETEDATE >= startDate && later.CRAETEDATE <= endDate : later.ISACTIVE == true)
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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

                //if (sDate != "" && eDate != "")
                //{
                //    data = data.Where(x => x.CRAETEDATE >= startDate && x.CRAETEDATE <= endDate).ToList();
                //}
                return PartialView("_reportsdata", data);
            }
            if (role == 2)
            {
                data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
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
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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


        public ActionResult LetterRequest(int? aid)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    if (aid == 1)
                    {
                        TempData["error"] = "You are not authorized! please contact to admin.";
                        return RedirectToAction("Dashboard", "Home");
                    }
                    int userid = Convert.ToInt32(Session["userid"]);                   
                    var data = (from later in db.LatterRequests
                                join ltype in db.LetterCrateTypes on later.LetterType equals ltype.ID
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                where later.USERID == userid
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,   
                                    LATTERNO=later.LATTERNO,  
                                    isapv=later.IsSpv,
                                    spvid=later.SPVID,
                                    LatterData= later.LatterData,
                                    StatusID=later.StatusId,
                                    Department=dept.DEPARTMENT,
                                    DeptID=dept.ID,
                                    LATTERNOSerice= ("HGIEL/" + site.TITLE+ "/" +dept.DEPARTMENT+ "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
                                }).ToList().OrderByDescending(a=>a.CRAETEDATE);
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

        public ActionResult LetterSpvRequest(int? aid)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    if (aid == 1)
                    {
                        TempData["error"] = "You are not map with approver! please contact with admin and try again.";
                    }
                    int userid = Convert.ToInt32(Session["userid"]);
                    var data = (from later in db.LatterSPVRequests
                                join ltype in db.LetterCrateTypes on later.LetterType equals ltype.ID
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                join spvm in db.spvmasters on later.SPVID equals spvm.ID
                                where later.USERID == userid 
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    isapv = later.IsSpv,
                                    spvid = later.SPVID,
                                    LatterData = later.LatterData,
                                    StatusID = later.StatusId,
                                    Department = dept.DEPARTMENT,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = (spvm.StartName+ "/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
                                    REMARK = ltype.TITLE,
                                    TITLE = site.TITLE,
                                    SITEID = site.ID,
                                    CODE = site.CODE,
                                    EMPCODE = user.EMPCODE,
                                    SITENO = site.SITENO,
                                    SITENONAME = site.SITENONAME,
                                    CREATEBY = later.CREATEBY,
                                    CRAETEDATE = later.CRAETEDATE,
                                    ISACTIVE = later.ISACTIVE
                                }).ToList().OrderByDescending(a=>a.CRAETEDATE);
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
                    int userid = Convert.ToInt32(Session["userid"]);                   
                    var approverin = db.Mst_USER.Where(a => a.ID == userid && a.Approver != null).Count();
                    if (approverin ==0)
                    {
                        return RedirectToAction("LetterRequest", "Home", new {aid=1});
                    }

                    ViewBag.ltype = db.LetterCrateTypes.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList(); ;
                    ViewBag.dept = (from d in db.Mst_SUBDEPARTMENT
                                            join da in db.DeptAssignUsers
                                            on d.ID equals da.DEPTID
                                            where d.ISACTIVE == true && da.USERID == userid && da.IsActive==true
                                            select new SelectListItem
                                            {
                                                Value = d.ID.ToString(),
                                                Text = d.SubDEPARTMENT.ToString()
                                            }).ToList();

                    ViewBag.site = (from l in db.Mst_SITE
                                    join la in db.LocationAssignUsers
                                    on l.ID equals la.LocID
                                    where l.ISACTIVE == true && la.USERID == userid
                                    select new SelectListItem
                                    {
                                        Value = l.ID.ToString(),
                                        Text = l.TITLE.ToString()
                                    }).ToList();
                    var date = new DateTime(DateTime.Now.Year, 4, 6);
                    //int lastinsert = db.LatterRequests.Where(a => a.USERID == userid && a.IsSpv==false).Count();               
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
        public ActionResult AddLetterRequest(LatterRequest model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    if (Convert.ToInt32(model.LATTERNO) <= 0)
                    {
                        TempData["error"] = "Letter Issued Failed";
                        return RedirectToAction("LetterRequest", "Home");
                    }
                    ViewBag.ltype = db.LetterCrateTypes.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList(); ;
                    model.USERID = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = 1;
                    model.ISACTIVE = true;
                    model.StatusId = 5;
                    model.FinanceYear = "2024-25";
                    model.IsSpv = false;
                    model.CRAETEDATE = DateTime.Now;
                    db.LatterRequests.Add(model);
                    db.SaveChanges();
                    int cid = Convert.ToInt32(Session["userid"].ToString());
                    db.loginsert(model.ID, "Letter create", 5,cid);
                    var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                    var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                    var msgtemp = etemp.RequestCreate(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                    mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                    TempData["success"] = "Letter Issued Succesfully";
                   return RedirectToAction("LetterRequest","Home");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "AddLetterRequest";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return View();
            }

        }


        public ActionResult AddSpvLetterRequest()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    int userid = Convert.ToInt32(Session["userid"]);
                    var approverin = db.Mst_USER.Where(a => a.ID == userid && a.Approver != null).Count();
                    if (approverin == 0)
                    {
                        return RedirectToAction("LetterSpvRequest", "Home", new { aid = 1 });
                    }

                    ViewBag.ltype = db.LetterCrateTypes.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList(); ;                    
                    ViewBag.spvtype = (from p in db.AssignSPVs
                                        join l in db.spvmasters on p.SpvID equals l.ID
                                        where l.IsActive == true && p.UserID == userid
                                       select new SelectListItem
                                        {
                                            Text = l.TITLE,
                                            Value = l.ID.ToString()
                                        }).ToList();
                    ViewBag.dept = (from d in db.Mst_SUBDEPARTMENT
                                    join da in db.DeptAssignUsers
                                    on d.ID equals da.DEPTID
                                    where d.ISACTIVE == true && da.USERID == userid && da.IsActive==true
                                    select new SelectListItem
                                    {
                                        Value = d.ID.ToString(),
                                        Text = d.SubDEPARTMENT.ToString()
                                    }).ToList();
                    ViewBag.site = (from l in db.Mst_SITE
                                    join la in db.LocationAssignUsers
                                    on l.ID equals la.LocID
                                    where l.ISACTIVE == true && la.USERID == userid
                                    select new SelectListItem
                                    {
                                        Value = l.ID.ToString(),
                                        Text = l.TITLE.ToString()
                                    }).ToList();
                    var date = new DateTime(DateTime.Now.Year, 4, 6);
                    
                    //int lastinsert = db.LatterSPVRequests.Where(a => a.USERID == userid).Count();
                    //LatterRequest latter = new LatterRequest();
                    //latter.LATTERNO = Convert.ToString(lastinsert + 1);
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
        public ActionResult AddSpvLetterRequest(LatterSPVRequest model)
        {
            //TempData["confirm"] = "true";
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    if (Convert.ToInt32(model.LATTERNO) <= 0)
                    {
                        TempData["error"] = "Letter Issued Failed";
                        return RedirectToAction("LetterSpvRequest", "Home");
                    }
                    int userid = Convert.ToInt32(Session["userid"]);
                    ViewBag.ltype = db.LetterCrateTypes.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList(); ;
                    ViewBag.spvtype = (from p in db.AssignSPVs
                                       join l in db.spvmasters on p.SpvID equals l.ID
                                       where l.IsActive == true && p.UserID == userid
                                       select new SelectListItem
                                       {
                                           Text = l.TITLE,
                                           Value = l.ID.ToString()
                                       }).ToList();
                    model.USERID = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = 1;
                    model.FinanceYear = "2024-25";
                    model.ISACTIVE = true;
                    model.StatusId = 5;
                    model.IsSpv = true;
                    model.CRAETEDATE = DateTime.Now;
                    db.LatterSPVRequests.Add(model);
                    db.SaveChanges();
                    int cid = Convert.ToInt32(Session["userid"].ToString());
                    db.loginsertSPV(model.ID, "Letter create", 5, cid);
                    var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                    var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                    var msgtemp = etemp.RequestCreate(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                    mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                    TempData["success"] = "Letter Issued Succesfully";
                    return RedirectToAction("LetterSpvRequest", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "AddLetterRequest";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return View();
            }

        }


        public ActionResult Performa(int id=0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    int uid = Convert.ToInt32(Session["userid"].ToString());
                    ViewBag.ltype = db.USERSIGNs.Where(x => x.IsActive == true && x.USERID== uid).Select(x => new SelectListItem { Text = x.SIGNTITLE, Value = x.ID.ToString() }).ToList(); ;
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

        public ActionResult PerformaAprover(int id = 0, int userid=0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    int uid = userid;
                    ViewBag.ltype = db.USERSIGNs.Where(x => x.IsActive == true && x.USERID == uid).Select(x => new SelectListItem { Text = x.SIGNTITLE, Value = x.ID.ToString() }).ToList(); ;
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


        public ActionResult PerformaSPV(int id = 0)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "2")
                {
                    int uid = Convert.ToInt32(Session["userid"].ToString());
                    ViewBag.ltype = db.USERSIGNs.Where(x => x.IsActive == true && x.USERID == uid).Select(x => new SelectListItem { Text = x.SIGNTITLE, Value = x.ID.ToString() }).ToList(); ;
                    var data = db.LatterSPVRequests.Where(x => x.ID == id).FirstOrDefault();
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
                    data.StatusId = 1;
                    db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, "In Progress", 1,cid);
                
                var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestSend(userdata.FULLNAME,userdata.EMPCODE,approver.FULLNAME, data.ID);
                mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);

                TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveDraft(string latterdata = null, int id = 0)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.LatterData = latterdata;
                data.StatusId = 6;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, "Save As Draft",6,cid);

                //var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                //var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                //var msgtemp = etemp.RequestSend(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                //mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);

                TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "SaveDraft";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PerformaSaveDraft(string latterdata = null, int id = 0)
        {
            try
            {
                var data = db.LatterSPVRequests.Where(a => a.ID == id).FirstOrDefault();
                data.LatterData = latterdata;
                data.StatusId = 6;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, "Save As Draft", 6, cid);

                //var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                //var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                //var msgtemp = etemp.RequestSend(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                //mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);

                TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "PerformaSaveDraft";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PerformadataEdit(string latterdata = null, int id = 0)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.LatterData = latterdata;
                data.ApproveDate = DateTime.Now;
                data.StatusId = 4;
                db.SaveChanges();
                int? cid = data.USERID;
              

                var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                db.loginsert(data.ID, "Edited and Approved ", 1, approver.ID);
                var msgtemp = etemp.RequestSend(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME,data.ID);
                mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);

                TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PerformaSPVdata(string latterdata = null, int id = 0)
        {
            try
            {
                var data = db.LatterSPVRequests.Where(a => a.ID == id).FirstOrDefault();
                data.LatterData = latterdata;
                data.StatusId = 1;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsertSPV(data.ID, "In Progress", 1, cid);

                var userdata = db.Mst_USER.Where(a => a.ID == cid).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestSendSPV(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(approver.EMAILID, msgtemp, "Electronic Letter Pad Request", true);

                TempData["success"] = "Thanks.";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult getsign(int id = 0)
        {
            try
            {
                var data = db.USERSIGNs.Where(a => a.ID == id).FirstOrDefault();                                            
                return Json(new { success = true,link=data.SIGNIMAGE }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult printviewDash(int id = 0)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = (from later in db.LatterRequests
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                where later.ID == id
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    ApproveDate=later.ApproveDate,
                                    Department = dept.DEPARTMENT,
                                    StatusID = later.StatusId,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
            catch (Exception ex)
            {
                return View();
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
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID                               
                                where later.ID == id
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    Department = dept.DEPARTMENT,
                                    StatusID=later.StatusId,
                                    ApproveDate=later.ApproveDate,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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

        public ActionResult printviewSPV(int id = 0)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = (from later in db.LatterSPVRequests
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                join SPVM in db.spvmasters on later.SPVID equals SPVM.ID
                                where later.ID == id
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    Department = dept.DEPARTMENT,
                                    ApproveDate=later.ApproveDate,
                                    StatusID = later.StatusId,
                                    svptitle=SPVM.TITLE,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = (SPVM.StartName+"/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
            catch (Exception ex)
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
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                        where later.ID == id
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            ApproveDate=later.ApproveDate,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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


        public ActionResult performaMail(int id = 0)
        {            
            var data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                        where later.ID == id
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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

        public ActionResult performaviewREQUEST(int id = 0, string date = null)
        {
            Session["datawise"] = null;
            if (date != null)
            {
                DateTime dateTime = DateTime.Parse(date);
                Session["datawise"] = dateTime.ToString("dd-MM-yyyy");
            }
            var data = (from later in db.LatterRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                        where later.ID == id
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            DeptID = dept.ID,
                            LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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

        public ActionResult performaSPVview(int id = 0, string date = null)
        {
            Session["datawise"] = null;
            if (date != null)
            {
                DateTime dateTime = DateTime.Parse(date);
                Session["datawise"] = dateTime.ToString("dd-MM-yyyy");
            }
            var data = (from later in db.LatterSPVRequests
                        join user in db.Mst_USER on later.USERID equals user.ID
                        join site in db.Mst_SITE on later.LocID equals site.ID
                        join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                        join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                        join spvm in db.spvmasters on later.SPVID equals spvm.ID
                        where later.ID == id
                        select new latterrvm
                        {
                            ID = later.ID,
                            FULLNAME = user.FULLNAME,
                            LATTERNO = later.LATTERNO,
                            LatterData = later.LatterData,
                            Department = dept.DEPARTMENT,
                            ApproveDate=later.ApproveDate,
                            svptitle=spvm.TITLE,
                            DeptID = dept.ID,
                            LATTERNOSerice = (spvm.StartName +"/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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


        public ActionResult SignList()
        {
            try
            {
                if (Session["userid"] != null)
                {
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    var data = db.USERSIGNs.Where(a => a.USERID == userid && a.IsActive==true).ToList();
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
        public ActionResult UploadSign()
        {
            try
            {
                if (Session["userid"] != null)
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

        public ActionResult SignDeactive(int id)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = db.USERSIGNs.Where(a => a.ID == id).FirstOrDefault();
                    data.IsActive = false;
                    db.SaveChanges();
                    return RedirectToAction("SignList", "Home");
                }
                else
                {
                    return RedirectToAction("SignList", "Home");
                }
            }
            catch (Exception ex)
            {
                return View();
            }            
        }


        [HttpPost]
        public ActionResult UploadSign(USERSIGN model,HttpPostedFileBase imgProfileC = null)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    USERSIGN uSERSIGN = new USERSIGN();
                    int userid = Convert.ToInt32(Session["userid"].ToString());                   
                    string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    string guid = Guid.NewGuid().ToString("N");
                    string ActualPathjoing = string.Concat(Domain + "/SignImage/", string.Concat(guid, Path.GetExtension(imgProfileC.FileName)));
                    var ServerPath = Path.Combine(Server.MapPath("~/SignImage"), string.Concat(guid, Path.GetExtension(imgProfileC.FileName)));
                    string extn = Path.GetExtension(ServerPath);
                    if (extn != ".pdf")
                    {
                        imgProfileC.SaveAs(ServerPath);
                        if (System.IO.File.Exists(ServerPath))
                        {
                            uSERSIGN.SIGNIMAGE = ActualPathjoing;
                        }
                        uSERSIGN.USERID = userid;
                        uSERSIGN.SIGNTITLE = model.SIGNTITLE;
                        uSERSIGN.CREATEDATE = DateTime.Now;
                        uSERSIGN.IsActive = true;
                        uSERSIGN.CRERATEBY = userid;
                        db.USERSIGNs.Add(uSERSIGN);
                        db.SaveChanges();
                    }

                    return RedirectToAction("SignList", "Home");
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

        public ActionResult UploadManual()
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = db.usermanuals.ToList();
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

        public ActionResult EditUploadManual(int id)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = db.usermanuals.Where(a=>a.ID==id).FirstOrDefault();
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
        public ActionResult EditUploadManual(usermanual model, HttpPostedFileBase imgProfileC = null)
        {
            try
            {
                if (Session["userid"] != null)
                {                    
                    var data = db.usermanuals.Where(a => a.ID == model.ID).FirstOrDefault();
                    string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    string guid = Guid.NewGuid().ToString("N");
                    string ActualPathjoing = string.Concat(Domain + "/UserManual/", string.Concat(guid, Path.GetExtension(imgProfileC.FileName)));
                    var ServerPath = Path.Combine(Server.MapPath("~/UserManual"), string.Concat(guid, Path.GetExtension(imgProfileC.FileName)));
                    string extn = Path.GetExtension(ServerPath);
                    if (extn == ".pdf")
                    {
                        imgProfileC.SaveAs(ServerPath);
                        if (System.IO.File.Exists(ServerPath))
                        {
                            data.ManualLink = ActualPathjoing;
                        }
                        data.IsActive = model.IsActive;
                        data.ManualTitle = model.ManualTitle;                                          
                        db.SaveChanges();
                    }

                    return RedirectToAction("UploadManual", "Home");
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
        public ActionResult ViewManual(int id)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    var data = db.usermanuals.Where(a => a.ID == id).FirstOrDefault();
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


        public ActionResult RequestReceived()
        {
            try
            {
                if (Session["userid"] != null)
                {
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    var data = (from later in db.LatterRequests
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                join status in db.mst_status on later.StatusId equals status.ID
                                where user.Approver == userid && later.StatusId != 5 
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    UserId = user.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    StatusData=status.TITLE,
                                    StatusID=later.StatusId,
                                    Department = dept.DEPARTMENT,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = ("HGIEL/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
                                }).OrderByDescending(a => a.CRAETEDATE).ToList();
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

        public ActionResult RequestReceivedSPV()
        {
            try
            {
                if (Session["userid"] != null)
                {
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    var data = (from later in db.LatterSPVRequests
                                join user in db.Mst_USER on later.USERID equals user.ID
                                join site in db.Mst_SITE on later.LocID equals site.ID
                                join subdept in db.Mst_SUBDEPARTMENT on later.DeptID equals subdept.ID
                                join dept in db.Mst104_DEPARTMENT on subdept.DeptID equals dept.ID
                                join status in db.mst_status on later.StatusId equals status.ID
                                join spvm in db.spvmasters on later.SPVID equals spvm.ID
                                where user.Approver == userid && later.StatusId != 5
                                select new latterrvm
                                {
                                    ID = later.ID,
                                    FULLNAME = user.FULLNAME,
                                    LATTERNO = later.LATTERNO,
                                    LatterData = later.LatterData,
                                    StatusData = status.TITLE,
                                    StatusID = later.StatusId,
                                    Department = dept.DEPARTMENT,
                                    DeptID = dept.ID,
                                    LATTERNOSerice = (spvm.StartName+"/" + site.TITLE + "/" + dept.DEPARTMENT + "/" + subdept.SubDEPARTMENT + "/"+later.FinanceYear+"/" + later.LATTERNO.ToString()),
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
                                }).OrderByDescending(a => a.CRAETEDATE).ToList();
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
        //RejectRequest
        public ActionResult ApproveData(int id)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.StatusId = 4;
                data.ApproveDate = DateTime.Now;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, "Approve", 4, cid);

                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestApprove(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Approved.";
                return RedirectToAction("RequestReceived", "Home");
            }
            catch(Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceived", "Home");

            }
            
        }


        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.ApproveDate = DateTime.Now;
                data.StatusId = 4;
                db.SaveChanges();
                //int cid = Convert.ToInt32(Session["userid"].ToString());                
                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                db.loginsert(data.ID, "Approve By Mail", 4, userdata.Approver);
                var msgtemp = etemp.RequestApprove(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                return View();
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return View();

            }

        }



        public ActionResult RejectRequest(int id)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.StatusId = 2;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, "Reject", 2, cid);

                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestReject(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Rejected.";
                return RedirectToAction("RequestReceived", "Home");
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceived", "Home");

            }

        }

        public ActionResult ApproveDataSPV(int id)
        {
            try
            {
                var data = db.LatterSPVRequests.Where(a => a.ID == id).FirstOrDefault();
                data.ApproveDate = DateTime.Now;
                data.StatusId = 4;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsertSPV(data.ID, "Approve", 4, cid);

                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestApprove(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Approved.";
                return RedirectToAction("RequestReceivedSPV", "Home");
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceivedSPV", "Home");

            }

        }

        public ActionResult RejectRequestSPV(int id)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == id).FirstOrDefault();
                data.StatusId = 2;
                db.SaveChanges();
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsertSPV(data.ID, "Reject", 2, cid);

                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestReject(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Rejected.";
                return RedirectToAction("RequestReceivedSPV", "Home");
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceivedSPV", "Home");

            }

        }

        [HttpPost]
        public ActionResult SendBack(string sendback,int sbid)
        {
            try
            {
                var data = db.LatterRequests.Where(a => a.ID == sbid).FirstOrDefault();
                data.StatusId = 3;
                db.SaveChanges();
                var remark = "Send To Back Reason are " + sendback;
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsert(data.ID, remark, 3, cid);
                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestSendtoback(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Send To Back.";
                return RedirectToAction("RequestReceived", "Home");
            }
            catch(Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceived", "Home");
            }
           
        }


        [HttpPost]
        public ActionResult SendBackSPV(string sendback, int sbid)
        {
            try
            {
                var data = db.LatterSPVRequests.Where(a => a.ID == sbid).FirstOrDefault();
                data.StatusId = 3;
                db.SaveChanges();
                var remark = "Send To Back Reason are " + sendback;
                int cid = Convert.ToInt32(Session["userid"].ToString());
                db.loginsertSPV(data.ID, remark, 3, cid);
                var userdata = db.Mst_USER.Where(a => a.ID == data.USERID).FirstOrDefault();
                var approver = db.Mst_USER.Where(a => a.ID == userdata.Approver).FirstOrDefault();
                var msgtemp = etemp.RequestSendtoback(userdata.FULLNAME, userdata.EMPCODE, approver.FULLNAME);
                mailsend(userdata.EMAILID, msgtemp, "Electronic Letter Pad Request", true);
                TempData["success"] = "Request Send To Back.";
                return RedirectToAction("RequestReceivedSPV", "Home");
            }
            catch (Exception ex)
            {
                errorlog err = new errorlog();
                err.actionname = "Performadata";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
                return RedirectToAction("RequestReceivedSPV", "Home");
            }

        }

        public ActionResult GetEmployeeLog(int id)
        {
            var data = (from log in db.Letter_Log_Process
                        join user in db.Mst_USER on log.CREATEBY equals user.ID
                        join ststus in db.mst_status on log.STATUSID equals ststus.ID
                        where log.LID == id
                        select new LogRequestVM
                        {
                            Remark = log.REMARK,
                            createbyname = user.FULLNAME,
                            CRAETEDATE=log.CREATEDATE,
                            createdates=log.CREATEDATE.ToString(),
                            Status =ststus.TITLE,
                           StatusID=ststus.ID
                        }).ToList();
            return Json(data: new { success = true, adata = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeLogSPV(int id)
        {
            var data = (from log in db.Letter_Log_SPV_Process
                        join user in db.Mst_USER on log.CREATEBY equals user.ID
                        join ststus in db.mst_status on log.STATUSID equals ststus.ID
                        where log.LID == id
                        select new LogRequestVM
                        {
                            Remark = log.REMARK,
                            createbyname = user.FULLNAME,
                            CRAETEDATE = log.CREATEDATE,
                            createdates = log.CREATEDATE.ToString(),
                            Status = ststus.TITLE,
                            StatusID = ststus.ID
                        }).ToList();
            return Json(data: new { success = true, adata = data }, JsonRequestBehavior.AllowGet);
        }

        public bool mailsend(string tomail, string message, string subject, bool mb)
        {
            try
            {
                MailMessage mc = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), tomail);
                mc.Subject = subject;
                mc.Body = message;
                if (mb == true)
                {
                    mc.IsBodyHtml = true;
                }
                SmtpClient smtp = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtpserver"].ToString(), 587);
                smtp.Timeout = 1000000;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;
                smtp.Send(mc);
            }
            catch (Exception ex)
            {
                Session["mailerror"] = ex.Message;
                errorlog err = new errorlog();
                err.actionname = "Mailsend";
                err.errormessage = ex.Message;
                err.createdate = DateTime.Now;
                db.errorlogs.Add(err);
                db.SaveChanges();
            }

            return true;

        }

        #region AssignDeptUserWise

        public ActionResult AssignUserDeptList()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from assign in db.DeptAssignUsers
                                join user in db.Mst_USER on assign.USERID equals user.ID
                                join sdept in db.Mst_SUBDEPARTMENT on assign.DEPTID equals sdept.ID
                                join dept in db.Mst104_DEPARTMENT on sdept.DeptID equals dept.ID
                                select new DEPTvm
                                {
                                    ID = assign.ID,
                                    username = user.FULLNAME,
                                    empcode = user.EMPCODE,
                                    department = sdept.SubDEPARTMENT,
                                    DeptName=dept.DEPARTMENT,
                                    CREATEBY = assign.CREATEBY,
                                    CRAETEDATE = assign.CREATEDATE,
                                    ISACTIVE = assign.IsActive
                                }).ToList().OrderBy(a => a.username);                  
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
        public ActionResult GetDeptBySubDept(int bid = 0)
        {
            try
            {
                //var locations = db.Mst108_COSTCENTER.Where(x => x.ISACTIVE == true && x.BusinessID == bid).Select(x => new SelectListItem { Text = x.COSTCENTER, Value = x.LocationID.ToString() }).ToList();

                var subdept = db.Mst_SUBDEPARTMENT.Where(x => x.ISACTIVE == true && x.DeptID==bid).Select(x => new SelectListItem { Text = x.SubDEPARTMENT, Value = x.ID.ToString() }).ToList();


                return Json(data: new { success = true, subdept = subdept }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(data: new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult getLno(int did = 0,int lid=0)
        {
            try
            {
                //var locations = db.Mst108_COSTCENTER.Where(x => x.ISACTIVE == true && x.BusinessID == bid).Select(x => new SelectListItem { Text = x.COSTCENTER, Value = x.LocationID.ToString() }).ToList();

                var subdept = db.LatterRequests.Where(a=>a.DeptID==did && a.LocID== lid && a.FinanceYear!="2023-24").Count();
                int lno = Convert.ToInt32(subdept + 1);

                return Json(data: new { success = true, lno = lno }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(data: new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult getsLno(int did = 0, int lid = 0)
        {
            try
            {
                //var locations = db.Mst108_COSTCENTER.Where(x => x.ISACTIVE == true && x.BusinessID == bid).Select(x => new SelectListItem { Text = x.COSTCENTER, Value = x.LocationID.ToString() }).ToList();

                var subdept = db.LatterSPVRequests.Where(a => a.DeptID == did && a.LocID == lid && a.FinanceYear!="2023-24").Count();
                int lno = Convert.ToInt32(subdept + 1);

                return Json(data: new { success = true, lno = lno }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(data: new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AssignUserDeptAdd()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.subdept = db.Mst_SUBDEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.SubDEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
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
        public ActionResult AssignUserDeptAdd(DeptAssignUser model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.dept = db.Mst_SUBDEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.SubDEPARTMENT, Value = x.ID.ToString() }).ToList();
                    //ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = userid;
                    model.CREATEDATE = DateTime.Now;
                    db.DeptAssignUsers.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Assigned successfully.";
                    return RedirectToAction("AssignUserDeptList");
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

        public ActionResult AssignUserDeptEdit(int id)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.subdept = db.Mst_SUBDEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.SubDEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.DeptAssignUsers.Where(a => a.ID == id).FirstOrDefault();
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
        public ActionResult AssignUserDeptEdit(DeptAssignUser model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.dept = db.Mst104_DEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.DEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.subdept = db.Mst_SUBDEPARTMENT.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.SubDEPARTMENT, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.DeptAssignUsers.Where(a => a.ID == model.ID).FirstOrDefault();
                    data.USERID = model.USERID;
                    data.DEPTID = model.DEPTID;
                    data.IsActive = model.IsActive;
                    db.SaveChanges();
                    TempData["success"] = "Data Updated Successfully";
                    return RedirectToAction("AssignUserDeptList", "Home", new {id=1});
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


        #region AssignLocUserWise

        public ActionResult AssignUserLocList()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    var data = (from assign in db.LocationAssignUsers
                                join user in db.Mst_USER on assign.USERID equals user.ID
                                join site in db.Mst_SITE on assign.LocID equals site.ID
                                select new Locvm
                                {
                                    ID = assign.ID,
                                    username = user.FULLNAME,
                                    empcode = user.EMPCODE,
                                    Location = site.TITLE,
                                    CREATEBY = assign.CREATEBY,
                                    CRAETEDATE = assign.CREATEDATE,
                                    ISACTIVE = assign.IsActive
                                }).ToList().OrderBy(a => a.username);
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
        public ActionResult AssignUserLocAdd()
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.site = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
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
        public ActionResult AssignUserLocAdd(LocationAssignUser model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.site = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    //ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    model.CREATEBY = userid;
                    model.CREATEDATE = DateTime.Now;
                    db.LocationAssignUsers.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "Assigned successfully.";
                    return RedirectToAction("AssignUserLocList");
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

        public ActionResult AssignUserLocEdit(int id)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.site = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.LocationAssignUsers.Where(a => a.ID == id).FirstOrDefault();
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
        public ActionResult AssignUserLocEdit(LocationAssignUser model)
        {
            try
            {
                if (Session["userid"] != null && Session["userrole"].ToString() == "1")
                {
                    ViewBag.site = db.Mst_SITE.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                    ViewBag.user = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ROLEID == 2).Select(x => new SelectListItem { Text = x.FULLNAME + "/ " + x.EMPCODE, Value = x.ID.ToString() }).ToList();
                    var data = db.LocationAssignUsers.Where(a => a.ID == model.ID).FirstOrDefault();
                    data.USERID = model.USERID;
                    data.LocID = model.LocID;
                    data.IsActive = model.IsActive;
                    db.SaveChanges();
                    TempData["success"] = "Data Updated Successfully";
                    return RedirectToAction("AssignUserLocList");
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

    }
}