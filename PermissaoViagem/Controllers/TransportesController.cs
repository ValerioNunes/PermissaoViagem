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
    public class TransportesController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: api/Transportes
        public IQueryable<Transporte> GetTransportes()
        {
            return db.Transportes;
        }

        // GET: api/Transportes/5
        [ResponseType(typeof(Transporte))]
        public IHttpActionResult GetTransporte(int id)
        {
            Transporte transporte = db.Transportes.Find(id);
            if (transporte == null)
            {
                return NotFound();
            }

            return Json(transporte);
        }

        // PUT: api/Transportes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransporte(int id, Transporte transporte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transporte.Id)
            {
                return BadRequest();
            }

            db.Entry(transporte).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransporteExists(id))
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

        // POST: api/Transportes
        [ResponseType(typeof(Transporte))]
        public IHttpActionResult PostTransporte(Transporte transporte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Transportes.Add(transporte);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = transporte.Id }, transporte);
        }

        // DELETE: api/Transportes/5
        [ResponseType(typeof(Transporte))]
        public IHttpActionResult DeleteTransporte(int id)
        {
            Transporte transporte = db.Transportes.Find(id);
            if (transporte == null)
            {
                return NotFound();
            }

            db.Transportes.Remove(transporte);
            db.SaveChanges();

            return Ok(transporte);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransporteExists(int id)
        {
            return db.Transportes.Count(e => e.Id == id) > 0;
        }
    }
}