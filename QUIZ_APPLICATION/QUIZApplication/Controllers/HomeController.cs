using QUIZApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QUIZApplication.Models;



namespace QUIZApplication.Controllers
{

    public class HomeController : Controller
    {

        string newSecurityKey = SecurityKeyGenerator.GenerateSecurityKey();


        QuizAppDBEntities1 db = new QuizAppDBEntities1();
        // GET: Home

        [HttpGet]
        public ActionResult SRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SRegister(Student SVW, HttpPostedFileBase imgfile)
        {
            Student S = new Student();

            S.Std_Name = SVW.Std_Name;
            S.Std_Password = SVW.Std_Password;
            S.Std_Image = uploadimage(imgfile);

            db.Students.Add(S);
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Registration Sucessfull')</script>";


            return RedirectToAction("SLogin");
        }

        public string uploadimage(HttpPostedFileBase imgfile)
        {
            string path = "-1";

            try
            {
                if (imgfile != null && imgfile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imgfile.FileName);

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                    {
                        string fileName = Path.GetFileName(imgfile.FileName);
                        Random R = new Random();
                        path = Path.Combine(Server.MapPath("'../../Content/Image/"), R.Next() + fileName);
                        imgfile.SaveAs(path);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error message.
                Debug.WriteLine("Error uploading image: " + ex.Message);
            }
            return path;
        }



        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");

        }

        public ActionResult Index()
        {
            if (Session["admin_id"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpGet]
        public ActionResult TLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TLogin(Admin A)
        {
            Admin ad = db.Admins.Where(x => x.Name == A.Name && x.Password == A.Password).SingleOrDefault();
            if (ad != null)
            {
                Session["admin_id"] = ad.Id;
                ViewBag.message = "<script>alert('Login Successfull ')</script>";
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.message = "<script>alert('Invalid Username and Password ')</script>";
            }
            return View();
        }
        [HttpPost]
        public ActionResult SLogin(Student S)
        {
            Student std = db.Students.Where(x => x.Std_Name == S.Std_Name && x.Std_Password == S.Std_Password).SingleOrDefault();

            if (std != null)
            {
                Session["student_id"] = std.Std_Id;
                ViewBag.msg = "<script>alert('Login Successful ')</script>";
                return RedirectToAction("StudentExam");
            }
            else
            {

                ViewBag.msg = "<script>alert('Login Failed ')</script>";
            }

            return View();
        }

        [HttpGet]
        public ActionResult SLogin()
        {
            return View();
        }


        public ActionResult StudentExam()
        {
            if (Session["student_id"] == null)
            {
                return RedirectToAction("SLogin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult StudentExam(string room)
        {

            List<Category> list = db.Categories.ToList();

            foreach (var item in list)
            {
                if (item.cat_encyptedstring == room)
                {
                    List<Tbl_Questions> li = db.Tbl_Questions.Where(x => x.Cat_Id == item.Cat_Id).ToList();

                    Queue<Tbl_Questions> queue = new Queue<Tbl_Questions>();

                    foreach (Tbl_Questions a in li)
                    {
                        queue.Enqueue(a);
                    }
                    TempData["questions"] = queue;
                   
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Room Found....";
                }
            }
            return View();
        }


      
            public ActionResult QuizStart()
            {
                if (Session["student_id"] == null)
                {
                    return RedirectToAction("SLogin");
                }

                if (TempData["score"] == null)
                {
                    TempData["score"] = 0;
                }
                Tbl_Questions q = null;

                if (TempData["questions"] != null)
                {
                    Queue<Tbl_Questions> qlist = (Queue<Tbl_Questions>)TempData["questions"];
                    if (qlist.Count > 0)
                    {
                        q = qlist.Peek();
                        qlist.Dequeue();
                        TempData["questions"] = qlist;
                        TempData.Keep();
                    }
                    else
                    {
                    return RedirectToAction("EndExam");
                }
                }
                else
                {
                    return RedirectToAction("StudentExam");
                }

                    if (TempData["score"] == null)
                    {
                        TempData["score"] = 0;
                    }

            // Pass the list of options as ViewData to the view
            ViewData["Options"] = new List<string> { q.OPA, q.OPB, q.OPC, q.OPD };

                return View(q);
            }

        [HttpPost]
       public ActionResult QuizStart(int id, Tbl_Questions Q)
        {

            Tbl_Questions correctans = db.Tbl_Questions.FirstOrDefault(x => x.Q_Id == id);

            string selectoption = Request.Form["SelectedOption"];
            int currentscore = 0;

            if(TempData["score"] != null)
            {
                currentscore = (int)TempData["score"];
            }
            if(selectoption != null && selectoption.Equals(correctans.COP))
            {
                currentscore++;
            }

            TempData["score"] =  currentscore;

            return RedirectToAction("QuizStart");
        }

        public ActionResult EndExam()
        {
            int score = Convert.ToInt32(TempData["score"]);

            return View(score);
        }

        public ActionResult Dashboard()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            if(Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Category> li = db.Categories.OrderByDescending(x => x.Cat_Id).ToList();
            ViewData["list"] = li;
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Category cat)
        {

            //Session["admin_id"] = 1;

            List<Category> li = db.Categories.OrderByDescending(x => x.Cat_Id).ToList();
            ViewData["list"] = li;

            Random R = new Random();

            Category C = new Category();
            C.Cat_Name = cat.Cat_Name;
            C.cat_encyptedstring = Crypto.Encrypt(C.Cat_Name.Trim() + R.Next().ToString(),true);
            C.Id = Convert.ToInt32(Session["admin_id"].ToString());
            db.Categories.Add(C);
            db.SaveChanges();

            return RedirectToAction("AddCategory");
        }
        [HttpGet]
        public ActionResult AddQuestions()
        {
            /*(Session["admin_id"] = 1;
            int sid = Convert.ToInt32(Session["admin_id"]);
            List<Category> li = db.Categories.Where(x => x.Id == sid).ToList();
            ViewBag.list = new SelectList(li, "Cat_Id", "Cat_Name");
            return View();*/

            int adminId = Convert.ToInt32(Session["admin_id"]);
            List<Category> categories = db.Categories.Where(x => x.Id == adminId).ToList();

            // Pass the categories to the view
            ViewBag.Categories = new SelectList(categories, "Cat_Id", "Cat_Name");

            return View();

        }
        [HttpPost]
        public ActionResult AddQuestions(Tbl_Questions Q)
        {/*
           if(ModelState.IsValid)
            {
                //Session["admin_id"] = 1;
                int sid = Convert.ToInt32(Session["admin_id"]);
                List<Category> li = db.Categories.Where(x => x.Id == sid).ToList();
                ViewBag.list = new SelectList(li, "Cat_Id", "Cat_Name");

                Tbl_Questions qa = new Tbl_Questions();

                qa.Q_Text = Q.Q_Text;
                qa.OPA = Q.OPA;
                qa.OPB = Q.OPB;
                qa.OPC = Q.OPC;
                qa.OPD = Q.OPD;
                qa.COP = Q.COP;
                qa.Cat_Id = Q.Cat_Id;

                db.Tbl_Questions.Add(qa);
                db.SaveChanges();
                TempData["message"] = "<script>alert('Question Added Sucessfully !!')</script>";
                TempData.Keep();

                return RedirectToAction("AddQuestions");
            }
            return View();*/

            if (ModelState.IsValid)
            {
                // Ensure that the category selected in the dropdown is assigned to the question
                if (Q.Cat_Id > 0)
                {
                    Tbl_Questions qa = new Tbl_Questions();

                    qa.Q_Text = Q.Q_Text;
                    qa.OPA = Q.OPA;
                    qa.OPB = Q.OPB;
                    qa.OPC = Q.OPC;
                    qa.OPD = Q.OPD;
                    qa.COP = Q.COP;
                    qa.Cat_Id = Q.Cat_Id; // Set the Cat_Id based on the dropdown selection

                    db.Tbl_Questions.Add(qa);
                    db.SaveChanges();
                    TempData["message"] = "<script>alert('Question Added Successfully !!')</script>";
                    TempData.Keep();

                    return RedirectToAction("AddQuestions");
                }
                else
                {
                    ModelState.AddModelError("Cat_Id", "Please select a category.");
                }
            }

            // Reload categories and return the view if there are validation errors
            int adminId = Convert.ToInt32(Session["admin_id"]);
            List<Category> categories = db.Categories.Where(x => x.Id == adminId).ToList();
            ViewBag.Categories = new SelectList(categories, "Cat_Id", "Cat_Name");

            return View(Q);

        }


    }

    
}