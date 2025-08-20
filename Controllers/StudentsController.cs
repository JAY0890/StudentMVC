using StudentMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StudentMVC.Controllers
{
    public class StudentsController : Controller
    {
        private StudentEntities1 db = new StudentEntities1();

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Course).Include(s => s.Dept);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.Course_id = new SelectList(db.Courses, "Id", "Course_name");
            ViewBag.Dept_id = new SelectList(db.Depts, "Id", "Dept_name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Phone,Dept_id,Course_id,Password")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Course_id = new SelectList(db.Courses, "Id", "Course_name", student.Course_id);
            ViewBag.Dept_id = new SelectList(db.Depts, "Id", "Dept_name", student.Dept_id);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.Course_id = new SelectList(db.Courses, "Id", "Course_name", student.Course_id);
            ViewBag.Dept_id = new SelectList(db.Depts, "Id", "Dept_name", student.Dept_id);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Phone,Dept_id,Course_id,Password")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Course_id = new SelectList(db.Courses, "Id", "Course_name", student.Course_id);
            ViewBag.Dept_id = new SelectList(db.Depts, "Id", "Dept_name", student.Dept_id);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        public ActionResult Search(int? dept, int? course)
        {
            var students = db.Students.Include(s => s.Course).Include(s => s.Dept).AsQueryable();

            if (dept.HasValue)
            {
                students = students.Where(s => s.Dept_id == dept.Value);
            }

            if (course.HasValue)
            {
                students = students.Where(s => s.Course_id == course.Value);
            }

            // Adjusted to match your actual table column names: Id and Dept_name, Course_name
            ViewBag.DeptId = new SelectList(db.Depts, "Id", "Dept_name");
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Course_name");

            return View(students.ToList());
        }
        /*
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Tbl_Student
                             .FirstOrDefault(u => u.Email == model.Email
                                               && u.Password == model.Password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or password");
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Index");
        }
        */

        // LOGIN - GET
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // LOGIN - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = db.Students.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (student != null)
            {
                FormsAuthentication.SetAuthCookie(student.Email, false);
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // LOGOUT
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
