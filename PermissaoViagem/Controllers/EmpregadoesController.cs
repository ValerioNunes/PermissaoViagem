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
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;

namespace PermissaoViagem.Controllers
{
    public class EmpregadoesController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: api/Empregadoes
        public IQueryable<Empregado> GetEmpregados()
        {
            return db.Empregados;
        }

        // GET: api/Empregadoes/5
        [ResponseType(typeof(Empregado))]
        public IHttpActionResult GetEmpregado(int id)
        {
            Empregado empregado = db.Empregados.Find(id);
            if (empregado == null)
            {
                return NotFound();
            }

            return Ok(empregado);
        }

        // PUT: api/Empregadoes/5
        [HttpOptions, HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmpregado(int id, [FromBody]Empregado empregado)
        {
            if (empregado != null)
            {
                DebugLog.Logar((empregado == null).ToString());
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != empregado.Id)
                {
                    return BadRequest();
                }

                db.Entry(empregado).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpregadoExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                      //  DebugLog.Logar("empregado não existe!");
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Ok();
            
        }

        // POST: api/Empregadoes
        [HttpOptions, HttpPost]
        [ResponseType(typeof(Empregado))]
        public IHttpActionResult PostEmpregado(Empregado empregado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (EmpregadoExists(empregado.Id))
            {
                DebugLog.Logar("Já existe");
            }
                //db.Empregados.Add(empregado);

            try
            {
                db.SaveChanges();
            }

            catch (DbUpdateException)
            {
                if (EmpregadoExists(empregado.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = empregado.Id }, empregado);
        }

        // DELETE: api/Empregadoes/5
        [ResponseType(typeof(Empregado))]
        public IHttpActionResult DeleteEmpregado(int id)
        {
            Empregado empregado = db.Empregados.Find(id);
            if (empregado == null)
            {
                return NotFound();
            }

            db.Empregados.Remove(empregado);
            db.SaveChanges();

            return Ok(empregado);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmpregadoExists(int id)
        {
            return db.Empregados.Count(e => e.Id == id) > 0;
        }
    }
}