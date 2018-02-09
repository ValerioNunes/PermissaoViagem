using PermissaoViagem.DAL;
using PermissaoViagem.Extension;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PermissaoViagem.Controllers
{
    public class AlterarStatusSolicitacaoViagemController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();
        private Mensagem msg = new Mensagem();

        // GET: api/AlterarStatusSolicitacaoViagem
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AlterarStatusSolicitacaoViagem/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AlterarStatusSolicitacaoViagem
        [HttpOptions, HttpPost]
        [ResponseType(typeof(StatusSolicitacaoViagemView))]
        public IHttpActionResult Post(StatusSolicitacaoViagemView dados)
        {
            if (dados != null)
            {
                try
                {
                    

                    List<SolicitacaoViagem> solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == dados.SolicitacaoViagemId)
                                                                                    .Include(s => s.Destino)
                                                                                    .Include(s => s.Origem)
                                                                                    .Include(s => s.Transporte)
                                                                                    .Include(s => s.Empregado)
                                                                                    .Include(s => s.AprovadorSolicitacaoId)
                                                                                    .Include(s => s.ViajanteSolicitacaoId).ToList();
                if (solicitacaoViagem.FirstOrDefault() == null)
                {
                    return NotFound();
                }


                FillObjects(solicitacaoViagem);
                AprovadorSolicitacao StatusAnterior = solicitacaoViagem.FirstOrDefault().AprovadorSolicitacaoId.FirstOrDefault();


                if (StatusAnterior == null)
                {
                    return NotFound();
                }

                AprovadorSolicitacao StatusNovo = new AprovadorSolicitacao();
                StatusNovo.SolicitacaoViagemId = dados.SolicitacaoViagemId;
                StatusNovo.StatusId = dados.StatusId;
                StatusNovo.AprovadorId = StatusAnterior.AprovadorId;
                StatusNovo.DataStatus = DateTime.Now;

                db.AprovadorSolicitacao.Add(StatusNovo);
                    

                db.SaveChanges();


                msg.MudancaDeStatus(dados.SolicitacaoViagemId);
                    
                return Json("Sua solicitação foi cadastrada com sucesso!");
                }catch (Exception e)
                {
                    DebugLog.Logar(e.Message);
                    DebugLog.Logar(e.StackTrace);
                    DebugLog.Logar(Utility.Details(e));
                    return Json("Erro ao cadastrar solicitação!");
                }
            }
            return Json("Erro ao cadastrar solicitação! dados = null");
        }

        

        // PUT: api/AlterarStatusSolicitacaoViagem/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/AlterarStatusSolicitacaoViagem/5
        public void Delete(int id)
        {
        }
        private void FillObjects(List<SolicitacaoViagem> solicitacaoViagems)
        {
            solicitacaoViagems.ForEach(x =>
            {
                x.AprovadorSolicitacaoId = x.AprovadorSolicitacaoId.OrderBy(y => y.Id).Reverse().ToList();
                foreach (var aprovadorSolicitacao in x.AprovadorSolicitacaoId)
                {
                    aprovadorSolicitacao.Aprovador = db.Aprovadores.Where(y => y.Id == aprovadorSolicitacao.AprovadorId).
                    Include(y => y.Empregado).FirstOrDefault();
                    aprovadorSolicitacao.Status = db.Status.Where(y => y.Id == aprovadorSolicitacao.StatusId).FirstOrDefault();
  
                }
                foreach (var viajanteSolicitacao in x.ViajanteSolicitacaoId)
                {
                    viajanteSolicitacao.Empregado = db.Empregados.Where(y => y.Id == viajanteSolicitacao.EmpregadoId).FirstOrDefault();
                    
                }
            });
        }
    }
}
