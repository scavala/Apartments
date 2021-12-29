using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Apartments.Models;
using System.Net;

namespace Apartments.Controllers
{
    public class ApartmentsController : Controller
    {

        ~ApartmentsController()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
        private readonly ModelContainer db = new ModelContainer();
        // GET: Apartments
        public ActionResult Index()
        {
            return View(db.Apartments);
        }

        // GET: Apartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Apartment apartment = db.Apartments
                .Include(ap => ap.UploadedFiles).
                SingleOrDefault(a => a.IDApartment == id);

            if (apartment == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        public ActionResult Create()
        {
            if (db.People.Count() == 0)
            {
                return RedirectToAction("Index", "People");
            }

            var model = new ApartmentVM
            {
                People = db.People
            };

            return View(model);
        }

        // POST: Apartments/Create
        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create([Bind(Include = "Address, City, Contact")] Apartment apartment, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                string strDDLValue = Request.Form["vlasnikID"].ToString();
                if (!int.TryParse(strDDLValue, out int vlasnikID)) { return RedirectToAction("Index"); }

                apartment.vlasnikID = vlasnikID;
                apartment.UploadedFiles = new List<UploadedFile>();
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var picture = new UploadedFile
                        {
                            Name = System.IO.Path.GetFileName(file.FileName),
                            ContentType = file.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(file.InputStream))
                        {
                            picture.Content = reader.ReadBytes(file.ContentLength);
                        }
                        apartment.UploadedFiles.Add(picture);
                    }
                }
                db.Apartments.Add(apartment);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Apartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Apartment apartment = db.Apartments
                .Include(ap => ap.UploadedFiles).
                SingleOrDefault(a => a.IDApartment == id);

            if (apartment == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var model = new ApartmentVM
            {
                People = db.People,
                Apartment = apartment,
                vlasnikID = apartment.vlasnikID

            };
            return View(model);
        }

        // POST: Apartments/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        public ActionResult EditConfirmed([Bind(Include = "IDApartment, Address, City, Contact")] Apartment apartment, IEnumerable<HttpPostedFileBase> files)
        {

            Apartment apartmentToUpdate = db.Apartments.Find(apartment.IDApartment);
            UpdateApartment(apartment, apartmentToUpdate);
            string strDDLValue = Request.Form["vlasnikID"].ToString();
            if (!int.TryParse(strDDLValue, out int vlasnikID)) { return RedirectToAction("Index"); }
            apartmentToUpdate.vlasnikID = vlasnikID;
            if (apartmentToUpdate.UploadedFiles == null)
            {
                apartmentToUpdate.UploadedFiles = new List<UploadedFile>();
            }
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var picture = new UploadedFile
                    {
                        Name = System.IO.Path.GetFileName(file.FileName),
                        ContentType = file.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(file.InputStream))
                    {
                        picture.Content = reader.ReadBytes(file.ContentLength);
                    }
                    apartmentToUpdate.UploadedFiles.Add(picture);
                }
            }

            db.Entry(apartmentToUpdate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        private static void UpdateApartment(Apartment apartment, Apartment apartmentToUpdate)
        {
            apartmentToUpdate.Address = apartment.Address;
            apartmentToUpdate.City = apartment.City;
            apartmentToUpdate.Contact = apartment.Contact;
        }


        // POST: Apartments/Delete/5

        public ActionResult Delete(int id)
        {
            db.UploadedFiles.RemoveRange(db.UploadedFiles.Where(f => f.ApartmentIDApartment == id));
            db.Apartments.Remove(db.Apartments.Find(id));
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK, "obrisan");
        }
    }
}
