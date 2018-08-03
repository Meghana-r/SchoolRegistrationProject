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
        RegistrationProcessEntities4 db = new RegistrationProcessEntities4();
        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            var data = new SelectList(db.Branches, "branch_id", "location");
            Session["rsdata"] = data;

            return View();

        }
        [HttpPost]
        public ActionResult Register(Application ap)
        {

            int seatavailble=0;
                int branchid = int.Parse(Request.Form["ddlbranchid"].ToString());
                int classid = int.Parse(Request.Form["ddlclassid"].ToString());
                string name = Request.Form["txtname"].ToString();
                int age = int.Parse(Request.Form["txtage"].ToString());
                DateTime dob = DateTime.Parse(Request.Form["txtdob"].ToString());
                string addr = Request.Form["txtaddress"].ToString();
          
                var data = db.Applications.GroupBy(x => new { x.branch_id, x.classid }).Select(x => new {bran=x.Key,seats=x.Count() }).ToList();
                foreach (var r in data)
                { if((r.bran.branch_id==branchid) )
                    { if(r.bran.classid==classid)
                        seatavailble = r.seats;
                       
                    }
                
            }
            // Response.Write(seatavailble+", ");
            int  result = db.Seats.Where(x => x.branch_id == branchid && x.class_id == classid).Select(x => x.seats).SingleOrDefault();
            if (seatavailble < result)
            {
                ap.branch_id = branchid;
                ap.classid = classid;
                ap.name = name;
                ap.age = age;
                ap.dob = dob;
                ap.address = addr;
                ap.category = "not processed";
                db.Applications.Add(ap);
                var b = db.SaveChanges();
                if (b > 0)
                    ModelState.AddModelError("", "your application id is " + ap.Id+"it is submitted");
            }
            else
            {
                ModelState.AddModelError("", "your application is not submitted  " );
            }

            return View();


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
            var data = db.Applications.Where(x => x.Id == id).SingleOrDefault();
            
            if(data.category=="not processed")
            {
                Response.Write("Application not processed,checklater");
            }
            else
            {
                var dat = db.ProcessedApplications.Where(x => x.Id == id).SingleOrDefault();
                Response.Write("your Aplication is processed by :" + dat.resolvedby);
                

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
                var data = db.Applications.Where(x => x.Id == id).SingleOrDefault();
                if (data != null)
                {
                    if (data.category == "not processed")
                    {
                        Response.Write("Application not processed");
                    }
                    else
                    {
                        var dat = db.ProcessedApplications.Where(x => x.Id == id).SingleOrDefault();
                        Response.Write(" Aplication is processed by :" + dat.resolvedby);


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
                    var data = db.Applications.Where(x => x.Id == id).SingleOrDefault();
                    int bid = data.branch_id;
                    data.category = "processed";
                    db.SaveChanges();
                    var resdata = db.Branches.Where(x => x.branch_id == bid).SingleOrDefault();
                    ProcessedApplication rs=new ProcessedApplication();
                    rs.Id = id;
                    rs.comments = cmt;
                    rs.resolvedby = resdata.contact_person;
                    rs.date_of_resolve = DateTime.Now;
                    db.ProcessedApplications.Add(rs);
                    var b=db.SaveChanges();

                }
            }
            return View();

        }
        [HttpGet]
        public ActionResult Report()
        {
            int data = db.Applications.Count();
            Session["data"] = data;
            int res = db.ProcessedApplications.Count();
            Session["sdata"] = res;
            Response.Write("  "+data+"    "+ res);
            
            var result = db.Applications.ToList();

            return View(result);
        }
        [HttpPost]
        public ActionResult Report(string ddl)
        {
            string option = Request.Form["ddlmember"].ToString();
            var result = db.Applications.ToList();
            if (option == "processed")
            {
                var result1 = db.Applications.Where(x => x.category == "processed").ToList();
                return View(result1);
            }
            if (option == "not processed")
            {
                var result1 = db.Applications.Where(x => x.category == "not processed").ToList();
                return View(result1);
            }
            return View(result);
        }
        
    }
}