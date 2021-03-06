﻿using Newtonsoft.Json;
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
    public class DadosParaSolicitacaoViagemAPIController : ApiController
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();
        private Mensagem msg = new Mensagem();
        // GET: api/DadosParaSolicitacaoViagemAPI
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DadosParaSolicitacaoViagemAPI/5
        public IHttpActionResult Get(int id)
        {

            List<SolicitacaoViagem> solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == id)
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

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
                        
            return Json(solicitacaoViagem);
        }

        // POST: api/DadosParaSolicitacaoViagemAPI
        [HttpOptions, HttpPost]
        [ResponseType(typeof(DadosParaSolicitacaoViagem))]
        public IHttpActionResult Post(DadosParaSolicitacaoViagem dados)
        {
            if (dados != null)
            {
                try
                {
                    SolicitacaoViagem solicitacaoViagem = new SolicitacaoViagem();
                    solicitacaoViagem.DataPartida = DateTime.Parse(dados.Partida);
                    solicitacaoViagem.DataChegadaPrevista = DateTime.Parse(dados.Chegada);



                    Local origem = db.Locals.Where(x => x.IdPlace.Equals(dados.Origem)).FirstOrDefault();
                    if(origem == null)
                    {
                            origem   = new Local();              
                            origem.Nome = dados.Origem;
                            origem.IdPlace = dados.IdOrigemPlace;
                            db.Locals.Add(origem);
                    }
                    

                    Local destino = db.Locals.Where(x => x.IdPlace.Equals(dados.Destino)).FirstOrDefault();
                    if (destino == null)
                    {
                        destino = new Local();
                        destino.Nome = dados.Destino;
                        destino.IdPlace = dados.IdDestinoPlace;
                        db.Locals.Add(destino);
                    }

                    db.SaveChanges();


                    solicitacaoViagem.DestinoId = destino.Id;
                    solicitacaoViagem.OrigemId  = origem.Id;

                    solicitacaoViagem.TransporteId = dados.Transporte;
                    solicitacaoViagem.EmpregadoId = dados.Solicitante;

                    db.SolicitacaoViagems.Add(solicitacaoViagem);


                    foreach (var telefone in dados.Contatos)
                    {
                        Contato contato = new Contato();
                        contato.SolicitacaoViagemId = solicitacaoViagem.Id;
                        contato.Telefone = telefone;
                        db.Contatos.Add(contato);
                    }


                    foreach (var matricula in dados.Viajantes)
                    {
                        ViajanteSolicitacao viajanteSolicitacao = new ViajanteSolicitacao();
                        viajanteSolicitacao.SolicitacaoViagemId = solicitacaoViagem.Id;
                        viajanteSolicitacao.EmpregadoId = matricula;
                        db.ViajanteSolicitacao.Add(viajanteSolicitacao);
                    }

                    AprovadorSolicitacao aprovadorSolicitacao = new AprovadorSolicitacao();
                    aprovadorSolicitacao.SolicitacaoViagemId = solicitacaoViagem.Id;
                    aprovadorSolicitacao.StatusId = 1;
                    aprovadorSolicitacao.DataStatus = DateTime.Now;
                    aprovadorSolicitacao.AprovadorId = db.Aprovadores.Where(x => x.EmpregadoId == dados.Aprovador).FirstOrDefault().Id;
                    db.AprovadorSolicitacao.Add(aprovadorSolicitacao);

                    db.SaveChanges();

                    msg.AguardandoAnalise(solicitacaoViagem.Id);

                    return Json("Sua solicitação foi cadastrada com sucesso!");
                }
                catch (Exception e)
                {
                    DebugLog.Logar(e.Message);
                    DebugLog.Logar(e.StackTrace);
                    DebugLog.Logar(Utility.Details(e));
                    return Json("Erro ao cadastrar solicitação!");
                }
            }

            return Ok();
        }

      

        // PUT: api/DadosParaSolicitacaoViagemAPI/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/DadosParaSolicitacaoViagemAPI/5
        public void Delete(int id)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
                    aprovadorSolicitacao.SolicitacaoViagem = null;
                }
                foreach (var viajanteSolicitacao in x.ViajanteSolicitacaoId)
                {
                    viajanteSolicitacao.Empregado = db.Empregados.Where(y => y.Id == viajanteSolicitacao.EmpregadoId).FirstOrDefault();
                    viajanteSolicitacao.SolicitacaoViagem = null;
                }
            });
        }
    }
}
