using PermissaoViagem.DAL;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PermissaoViagem.Controllers
{
    public class MinhasSolicitacoesController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();
        // GET: api/MinhasSolicitacoes
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MinhasSolicitacoes/5
        public IHttpActionResult Get(int id)
        {
            Empregado empregado = db.Empregados.Find(id);
            if (empregado == null)
            {
                return NotFound();
            }

            var solicitacaoviagem = db.SolicitacaoViagems.Include(s => s.Destino).Include(s => s.Origem)
                                                                                .Include(s => s.Transporte)
                                                                                .Include(s => s.Solicitante)
                                                                                .Include(s => s.AprovadorSolicitacaoId)
                                                                                .Include(s => s.ViajanteSolicitacaoId).ToList();
            FillObjects(solicitacaoviagem);
            var minhasSolicitacoes = solicitacaoviagem.Where(x => x.SolicitanteId == id ||
                                                             x.ViajanteSolicitacaoId.Select(y => y.EmpregadoId).Contains(id) || x.AprovadorSolicitacaoId.Select(k => k.Aprovador.Empregado.Id).Contains(id)).ToList();

            List<MinhasSolicitacoes> minhasSolicitacoesNaTela = new List<MinhasSolicitacoes>();

            minhasSolicitacoes.ForEach(x => {
                MinhasSolicitacoes minhaSolicitacao = new MinhasSolicitacoes();
                minhaSolicitacao.Id = x.Id;
                minhaSolicitacao.Aprovador = x.AprovadorSolicitacaoId.FirstOrDefault().Aprovador.Empregado.Nome;
                minhaSolicitacao.Chegada = x.DataChegadaPrevista;
                minhaSolicitacao.Partida = x.DataPartida;
                minhaSolicitacao.Origem = x.Origem.Nome;
                minhaSolicitacao.Destino = x.Destino.Nome;
                minhaSolicitacao.Solicitante = x.Solicitante.Nome;
                minhaSolicitacao.Transporte = x.Transporte.Nome;
                minhaSolicitacao.Status = x.AprovadorSolicitacaoId.FirstOrDefault().Status.Nome;
                minhasSolicitacoesNaTela.Add(minhaSolicitacao);
            });                      



            return Json(minhasSolicitacoesNaTela);

        }

        // POST: api/MinhasSolicitacoes
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MinhasSolicitacoes/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MinhasSolicitacoes/5
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
