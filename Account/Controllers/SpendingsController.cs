using Account.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Account.Controllers
{
    public class SpendingsController : Controller
    {
        // GET: Spendings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllSpendings());
        }

        IEnumerable<Spendings> GetAllSpendings()
        {
            using (CashJugDBEntities db = new CashJugDBEntities())
            {
                return db.Spendings.ToList<Spendings>();
            }
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Spendings spend = new Spendings();
            if (id != 0)
            {
                using (CashJugDBEntities db = new CashJugDBEntities())
                {
                    spend = db.Spendings.Where(x => x.ItemID == id).FirstOrDefault<Spendings>();
                }
            }
            return View(spend);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Spendings spend)
        {
            try
            {
                if (spend.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(spend.ImageUpload.FileName);
                    string extension = Path.GetExtension(spend.ImageUpload.FileName);
                    fileName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    spend.ImagePath = "~/AppFiles/Images/" + fileName;
                    spend.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (CashJugDBEntities db = new CashJugDBEntities())
                {
                    if (spend.ItemID == 0)
                    {
                        db.Spendings.Add(spend);
                        db.SaveChanges(); 
                    }
                    else
                    {
                        db.Entry(spend).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
                //return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "Index", GetAllSpendings()), message = "Submitted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (CashJugDBEntities db = new CashJugDBEntities())
                {
                    Spendings spend = db.Spendings.Where(x => x.ItemID == id).FirstOrDefault<Spendings>();
                    db.Spendings.Remove(spend);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllSpendings()), message = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}