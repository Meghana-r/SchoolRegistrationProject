using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationsMVC.Models;

namespace ApplicationsMVC.Controllers
{
    public class UserController : Controller
    {
        ProjectDBEntities db = new ProjectDBEntities();
        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            var data = new SelectList(db.Branches, "branch_id", "location");
            Session["rsdata"] = data;

            return View();

        }
        [HttpPost]
        public ActionResult Register(ApplicationForm ap)
        {

            {

                int seatavailble = 0;
                int branchid = int.Parse(Request.Form["ddlbranchid"].ToString());

                int classid = int.Parse(Request.Form["ddlclassid"].ToString());
                string name = Request.Form["txtname"].ToString();
                if (name == "")
                {
                    Response.Write("Enter name");
                    return View();
                }

                if (Request.Form["txtage"].ToString() == "")
                {
                    Response.Write("Enter age");
                    return View();
                }
                int age = int.Parse(Request.Form["txtage"].ToString());
                if (age < 5 || age > 15)
                {
                    Response.Write("Enter age between 5 and 15");
                    return View();
                }
                DateTime dob = DateTime.Parse(Request.Form["txtdob"].ToString());
                if (Request.Form["txtaddress"].ToString() == "")
                {
                    Response.Write("Enter address");
                    return View();
                }
                string addr = Request.Form["txtaddress"].ToString();

                var data = db.ApplicationForms.GroupBy(x => new { x.branch_id, x.classid }).Select(x => new { bran = x.Key, seats = x.Count() }).ToList();
                foreach (var r in data)
                {
                    if ((r.bran.branch_id == branchid))
                    {
                        if (r.bran.classid == classid)
                            seatavailble = r.seats;

                    }

                }
               
                int result = db.Seats.Where(x => x.branch_id == branchid && x.classid == classid).Select(x => x.seats).SingleOrDefault();
                if (seatavailble < result)
                {
                    ap.branch_id = branchid;
                    ap.classid = classid;
                    ap.name = name;
                    ap.age = age;
                    ap.dob = dob;
                    ap.address = addr;
                    ap.category = "not processed";
                    db.ApplicationForms.Add(ap);
                    var b = db.SaveChanges();
                    if (b > 0)
                        ModelState.AddModelError("", "Your application with id " + ap.Id + "is submitted.Thank you for applying");
                }
                else
                {
                    ModelState.AddModelError("", "Your application is not submitted  ");
                }

                return View();

            }
            }
        [HttpGet]
        public ActionResult checkstatus()
        {
            

            return View();

        }
        [HttpPost]
        public ActionResult checkstatus(int id=0)
        {
            id = int.Parse(Request.Form["txtcheck"].ToString());
            var data = db.ApplicationForms.Where(x => x.Id == id).SingleOrDefault();
            
            if(data.category=="not processed")
            {
                Response.Write("Application not processed,checklater");
            }
            else
            {
                var dat = db.ProcessedForms.Where(x => x.Application_id == id).SingleOrDefault();
                Response.Write("Your Aplication is processed by :" + dat.resolved_By);
                

            }
            return View();

        }
        [HttpGet]
        public ActionResult process()
        {

            return View();
        }
        [HttpPost]
        public ActionResult process(string command)
        {
            int id = int.Parse(Request.Form["txtid"].ToString());
            if (command == "status")
            {
                var data = db.ApplicationForms.Where(x => x.Id == id).SingleOrDefault();
                if (data != null)
                {
                    if (data.category == "not processed")
                    {
                        Response.Write("Application not processed");
                    }
                    else
                    {
                        var dat = db.ProcessedForms.Where(x => x.Application_id == id).SingleOrDefault();
                        Response.Write("Application processed by :" + dat.resolved_By);


                    }
                }
                else
                {
                    Response.Write("Invalid Application Id");
                }


            }
            if (command == "Submit")
            {
                string cmt = Request.Form["txtcmt"].ToString();
                if (cmt == "accepted")
                {
                    Response.Write("Application is processed");
                    var data = db.ApplicationForms.Where(x => x.Id == id).SingleOrDefault();
                    var bid = data.branch_id;
                    data.category = "processed";
                    db.SaveChanges();
                    var resdata = db.Branches.Where(x => x.branch_id == bid).SingleOrDefault();
                    ProcessedForm rs=new ProcessedForm();
                    rs.Application_id = id;
                    rs.comments = cmt;
                    rs.resolved_By = resdata.contact_person;
                    rs.date_of_resolve = DateTime.Now;
                    db.ProcessedForms.Add(rs);
                    var b=db.SaveChanges();

                }
            }
            return View();

        }
        [HttpGet]
        public ActionResult Report()
        {
            int data = db.ApplicationForms.Count();
            Session["data"] = data;
            int res = db.ProcessedForms.Count();
            Session["sdata"] = res;
            Response.Write("  "+data+"    "+ res);
            
            var result = db.ApplicationForms.ToList();

            return View(result);
        }
        [HttpPost]
        public ActionResult Report(string ddl)
        {
            string option = Request.Form["ddlmember"].ToString();
            var result = db.ApplicationForms.ToList();
            if (option == "processed")
            {
                var result1 = db.ApplicationForms.Where(x => x.category == "processed").ToList();
                return View(result1);
            }
            if (option == "not processed")
            {
                var result1 = db.ApplicationForms.Where(x => x.category == "not processed").ToList();
                return View(result1);
            }
            return View(result);
        }
        
    }
}