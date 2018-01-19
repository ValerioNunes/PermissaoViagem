using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PermissaoViagem.DAL;
using PermissaoViagem.Models;

namespace PermissaoViagem.Controllers
{
    public class LocalsController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: api/Locals
        public IQueryable<Local> GetLocals()
        {
            return db.Locais;
        }

        // GET: api/Locals/5
        [ResponseType(typeof(Local))]
        public IHttpActionResult GetLocal(int id)
        {
            Local local = db.Locais.Find(id);
            if (local == null)
            {
                return NotFound();
            }

            return Ok(local);
        }

        // PUT: api/Locals/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLocal(int id, Local local)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != local.Id)
            {
                return BadRequest();
            }

            db.Entry(local).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Locals
        [ResponseType(typeof(Local))]
        public IHttpActionResult PostLocal(Local local)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Locais.Add(local);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = local.Id }, local);
        }

        // DELETE: api/Locals/5
        [ResponseType(typeof(Local))]
        public IHttpActionResult DeleteLocal(int id)
        {
            Local local = db.Locais.Find(id);
            if (local == null)
            {
                return NotFound();
            }

            db.Locais.Remove(local);
            db.SaveChanges();

            return Ok(local);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocalExists(int id)
        {
            return db.Locais.Count(e => e.Id == id) > 0;
        }
    }
}