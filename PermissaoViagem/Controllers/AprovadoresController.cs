using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PermissaoViagem.DAL;
using PermissaoViagem.Extension;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;

namespace PermissaoViagem.Controllers
{
    public class AprovadoresController : Controller
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: Aprovadores
        public ActionResult Index()
        {
            IQueryable<Aprovador> aprovadors = null;
            try
            {
                aprovadors = db.Aprovadores.Include(a => a.Empregado);
                return View(aprovadors.ToList());
            }
            catch (Exception e)
            {
                DebugLog.Logar(Utility.Details(e));
            }
            return View(aprovadors.ToList());
        }

        // GET: Aprovadores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprovador aprovador = db.Aprovadores.Where( x => x.Id == id).Include(x => x.Empregado).FirstOrDefault();

            if (aprovador == null)
            {
                return HttpNotFound();
            }
            return View(aprovador);
        }

        // GET: Aprovadores/Create
        public ActionResult Create()
        {
            var listaEmpregados = db.Empregados.ToList();
            listaEmpregados = listaEmpregados.Where(x => x.NivelGerencial.Contains("Manager")).OrderBy(x => x.Nome).ToList();
            ViewBag.EmpregadoId = new SelectList(listaEmpregados, "Id", "Nome");
            return View();
        }

        // POST: Aprovadores/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmpregadoId")] Aprovador aprovador)
        {
            if (ModelState.IsValid)
            {
                db.Aprovadores.Add(aprovador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpregadoId = new SelectList(db.Empregados, "Id", "Nome", aprovador.EmpregadoId);
            return View(aprovador);
        }

        // GET: Aprovadores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprovador aprovador = db.Aprovadores.Find(id);
            if (aprovador == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpregadoId = new SelectList(db.Empregados, "Id", "Nome", aprovador.EmpregadoId);
            return View(aprovador);
        }

        // POST: Aprovadores/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmpregadoId")] Aprovador aprovador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aprovador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpregadoId = new SelectList(db.Empregados, "Id", "Nome", aprovador.EmpregadoId);
            return View(aprovador);
        }

        // GET: Aprovadores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
    
             Aprovador aprovador = db.Aprovadores.Where(x => x.Id == id).Include(x => x.Empregado).FirstOrDefault();

            if (aprovador == null)
            {
                return HttpNotFound();
            }
            return View(aprovador);
        }

        // POST: Aprovadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //db.Aprovadores.Where( x => x.Id == id).Include(x => x.Empregado).FirstOrDefault();
            Aprovador aprovador = db.Aprovadores.Where(x => x.Id == id).Include(x => x.Empregado).FirstOrDefault();
            db.Aprovadores.Remove(aprovador);
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
    }
}
