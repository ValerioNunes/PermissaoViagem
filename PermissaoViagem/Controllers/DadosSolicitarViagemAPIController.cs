using PermissaoViagem.DAL;
using PermissaoViagem.Extension;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;




namespace PermissaoViagem.Controllers
{
    public class DadosSolicitarViagemAPIController : ApiController
    {

        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: api/DadosSolicitarViagemAPI
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DadosSolicitarViagemAPI/5
        public IHttpActionResult Get(int id)
        {
            Empregado empregado = db.Empregados.Find(id);
            if (empregado == null)
            {
                return NotFound();
            }

            DadosSolicitarViagem dados = new DadosSolicitarViagem();
            dados.Aprovador = db.Empregados.Where(x => (x.Gerencia.Equals(empregado.Gerencia) &&
                                               (x.NivelGerencial.Equals("Manager")) || (x.Supervisao.Equals(empregado.Supervisao) && x.NivelGerencial.Equals("Supervisor")))
                                               && db.Aprovadores.Select(y => y.EmpregadoId).ToList().Contains(x.Id)).ToList(); dados.Solicitante = empregado;
            dados.Local = db.Locals.ToList();
            dados.Transporte = db.Transportes.ToList();
            dados.Status = db.Status.ToList();



            return Json(dados);
        }


               
        public IHttpActionResult PostDadosParaSolicitacaoViagem(DadosParaSolicitacaoViagem dados)
        {

            DebugLog.Logar(dados.Aprovador.ToString());  

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }                      

            return Json("Solicitado com Sucesso!");
        }


        // PUT: api/DadosSolicitarViagemAPI/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DadosSolicitarViagemAPI/5
        public void Delete(int id)
        {
        }
    }
}
