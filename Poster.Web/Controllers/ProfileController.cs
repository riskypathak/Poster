using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Poster.Data;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using Poster.Data.Models;
using System.Data;
using ServiceStack.OrmLite;
using System.Web.Script.Serialization;
using ServiceStack.Data;
namespace Poster.Web.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                Profile model = new Profile();
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    var items = db.Select<ProfileType>().Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                    ViewBag.ProfileTypeList = items;

                    var result = db.Select<Profile>().Select(x => new Profile
                    {
                        Id = x.Id,
                        Username = x.Username,
                        ProfileName = db.LoadSelect<ProfileType>().Where(u => u.Id == x.ProfileTypeID).FirstOrDefault()
                    }).ToList();
                    ViewBag.userProfileList = result;

                }

                return View(model);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Profile model, int? Id)
        {
            try
            {
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    //EDIT CODE COMMENTED
                    //if (model.Id > 0)
                    //{
                    //    db.Update<Profile>(model);
                    //}
                    //else
                    //{
                    //    db.Insert<Profile>(model);
                    //}
                    db.Insert<Profile>(model);

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            try
            {
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    db.DeleteById<Profile>(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}