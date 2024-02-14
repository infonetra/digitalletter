using letterhead.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace letterhead.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        letterheadEntities db = new letterheadEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {            
            ViewBag.Title = "Login";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string username = null, string password = null)
        {            
            if (username != null && password != null)
            {
                var auth = db.Mst_USER.Where(x => x.ISACTIVE == true && x.EMPCODE == username && x.USERPWD == password).FirstOrDefault();
                if(auth != null)
                {
                    if (auth.OneTimeUser == true)
                    {
                        Session["userid"] = auth.ID;
                        return RedirectToAction("UpdatePwd");
                    }

                    if (auth.ROLEID == 1)
                    {
                        if (auth.CANLOGIN == true)
                        {
                            Session["userid"] = auth.ID;
                            Session["userrole"] = auth.ROLEID;
                            Session["username"] = auth.USERNAME;
                            Session["isapprover"] = auth.IsApprover;
                            Session["loginstatus"] = "1";
                            return RedirectToAction("Dashboard", "Home");
                        }
                        TempData["error"] = "Wrong User!";
                        return View();
                    }
                    if (auth.ROLEID == 2)
                    {
                        if (auth.CANLOGIN == true)
                        {
                            Session["userid"] = auth.ID;
                            Session["userrole"] = auth.ROLEID;
                            Session["username"] = auth.FULLNAME;
                            Session["isapprover"] = auth.IsApprover;
                            Session["loginstatus"] = "1";
                            return RedirectToAction("Dashboard", "Home");
                        }
                        TempData["error"] = "Wrong User!";
                        ViewBag.roles = db.Mst_Role.Where(x => x.ISACTIVE == true).Select(x => new SelectListItem { Text = x.TITLE, Value = x.ID.ToString() }).ToList();
                        return View();
                    }
                }                    

                TempData["error"] = "Invalid Credentials";                
                return View();
            }
            ViewBag.error = "error";           
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            TempData["sweettoaster"] = "";
            return Redirect("Login");
        }

        [AllowAnonymous]
        public ActionResult UpdatePwd()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UpdatePwd(string username = null, string password = null)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            if (username != null && password != null)
            {
                var auth = db.Mst_USER.Where(x => x.ISACTIVE == true && x.ID== userid).FirstOrDefault();
                if(auth != null)
                {
                    if (auth.EMPCODE == username && auth.USERPWD == password)
                    {
                        TempData["error"] = "Please Set Another Password! Thanks";
                        return View();
                    }
                }
                
                auth.EMPCODE = username;
                auth.USERPWD=password;
                auth.OneTimeUser = false;
                db.SaveChanges();
                TempData["success"] = "Password changed successfully";
                return RedirectToAction("Login");
            }
            ViewBag.error = "error";
            return View();
        }

     

    }
}