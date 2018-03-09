using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PermissaoViagem.DAL;
using PermissaoViagem.Extension;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;


namespace PermissaoViagem.Controllers
{
    public class SolicitacaoViagemsController : Controller
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();

        // GET: SolicitacaoViagems
        public ActionResult Index()
        {

         
            var solicitacaoViagems = db.SolicitacaoViagems.Include(s => s.Destino)
                                                          .Include(s => s.Origem)
                                                          .Include(s => s.Transporte)
                                                          .Include(s => s.Empregado)
                                                          .Include(s => s.AprovadorSolicitacaoId)
                                                          .Include(s => s.ViajanteSolicitacaoId).ToList();


            FillObjects(solicitacaoViagems);
            return View(solicitacaoViagems.ToList());
        }

        // GET: SolicitacaoViagems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitacaoViagem solicitacaoViagem = db.SolicitacaoViagems.Find(id);
            if (solicitacaoViagem == null)
            {
                return HttpNotFound();
            }
            return View(solicitacaoViagem);
        }

        // GET: SolicitacaoViagems/Create
        public ActionResult Create()
        {
            var empregadosOrdenados = db.Empregados.OrderBy(x => x.Nome).ToList();
            var aprovadores = db.Aprovadores.Select(x => x).Include(x => x.Empregado).
                OrderBy(x => x.Empregado.Nome).ToList();

            ViewBag.EmpregadoId = new SelectList(empregadosOrdenados, "Id", "Nome");
            ViewBag.DestinoId = new SelectList(db.Locals, "Id", "Nome");
            ViewBag.OrigemId = new SelectList(db.Locals, "Id", "Nome");
            ViewBag.TransporteId = new SelectList(db.Transportes, "Id", "Nome");
            ViewBag.AprovadorId = new SelectList(aprovadores, "Id", "Empregado.Nome");
            ViewBag.ViajantesId = new SelectList(empregadosOrdenados, "Id", "Nome");

            return View();
        }

        // POST: SolicitacaoViagems/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DataPartida,DataChegadaPrevista,TransporteId,OrigemId,DestinoId,EmpregadoId")] SolicitacaoViagem solicitacaoViagem,
            List<int> ViajantesId, int AprovadorId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.SolicitacaoViagems.Add(solicitacaoViagem);

                    foreach (var matricula in ViajantesId)
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
                    aprovadorSolicitacao.AprovadorId = AprovadorId;
                    db.AprovadorSolicitacao.Add(aprovadorSolicitacao);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    DebugLog.Logar(Utility.Details(e));
                }
            }

            var empregadosOrdenados = db.Empregados.OrderBy(x => x.Nome).ToList();
            var aprovadores = db.Aprovadores.Select(x => x).Include(x => x.Empregado).
                OrderBy(x => x.Empregado.Nome).ToList();
            ViewBag.DestinoId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.DestinoId);
            ViewBag.OrigemId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.OrigemId);
            ViewBag.TransporteId = new SelectList(db.Transportes, "Id", "Nome", solicitacaoViagem.TransporteId);
            ViewBag.EmpregadoId = new SelectList(empregadosOrdenados, "Id", "Nome", solicitacaoViagem.EmpregadoId);
            ViewBag.AprovadorId = new SelectList(aprovadores, "Id", "Empregado.Nome", AprovadorId);
            ViewBag.ViajantesId = new MultiSelectList(empregadosOrdenados, "Id", "Nome", ViajantesId);

            return View(solicitacaoViagem);
        }

        // GET: SolicitacaoViagems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitacaoViagem solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == id).Include(s => s.Destino)
                                                                                              .Include(s => s.Origem)
                                                                                              .Include(s => s.Transporte)
                                                                                              .Include(s => s.Empregado)
                                                                                              .Include(s => s.AprovadorSolicitacaoId)
                                                                                              .Include(s => s.ViajanteSolicitacaoId).FirstOrDefault();
            if (solicitacaoViagem == null)
            {
                return HttpNotFound();
            }

            List<SolicitacaoViagem> lvSolicitation = new List<SolicitacaoViagem>();
            lvSolicitation.Add(solicitacaoViagem);
            FillObjects(lvSolicitation);

            var empregadosOrdenados = db.Empregados.OrderBy(x => x.Nome).ToList();
            var aprovadores = db.Aprovadores.Select(x => x).Include(x => x.Empregado).
                OrderBy(x => x.Empregado.Nome).ToList();
            var aprovador = solicitacaoViagem.AprovadorSolicitacaoId.FirstOrDefault().AprovadorId;
            List<int> viajantes = solicitacaoViagem.ViajanteSolicitacaoId.Select(x => x.EmpregadoId).ToList();

            ViewBag.DestinoId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.DestinoId);
            ViewBag.OrigemId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.OrigemId);
            ViewBag.TransporteId = new SelectList(db.Transportes, "Id", "Nome", solicitacaoViagem.TransporteId);
            ViewBag.EmpregadoId = new SelectList(empregadosOrdenados, "Id", "Nome", solicitacaoViagem.EmpregadoId);


            ViewBag.AprovadorId = new SelectList(aprovadores, "Id", "Empregado.Nome", aprovador);
            ViewBag.ViajantesId = new MultiSelectList(empregadosOrdenados, "Id", "Nome", empregadosOrdenados.Where(x => viajantes.Contains(x.Id)).Select(x => x.Id).ToList());
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Nome", solicitacaoViagem.AprovadorSolicitacaoId.FirstOrDefault().Status.Id);


            return View(solicitacaoViagem);
        }

        // POST: SolicitacaoViagems/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public ActionResult Edit([Bind(Include = "Id,DataPartida,DataChegadaPrevista,TransporteId,OrigemId,DestinoId,EmpregadId")] SolicitacaoViagem solicitacaoViagem)
        public ActionResult Edit([Bind(Include = "Id,DataPartida,DataChegadaPrevista,TransporteId,OrigemId,DestinoId,EmpregadoId")] SolicitacaoViagem solicitacaoViagem,
            List<int> ViajantesId, int AprovadorId, int StatusId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitacaoViagem).State = EntityState.Modified;

                AprovadorSolicitacao aprovadorSolicitacao = new AprovadorSolicitacao();
                aprovadorSolicitacao.SolicitacaoViagemId = solicitacaoViagem.Id;
                aprovadorSolicitacao.StatusId = StatusId;
                aprovadorSolicitacao.DataStatus = DateTime.Now;
                aprovadorSolicitacao.AprovadorId = AprovadorId;
                db.AprovadorSolicitacao.Add(aprovadorSolicitacao);

                var viajantesAntigos = db.ViajanteSolicitacao.Where(x => x.SolicitacaoViagemId == solicitacaoViagem.Id).ToList();
                var removidos = viajantesAntigos.Where(x => !ViajantesId.Contains(x.EmpregadoId)).ToList();
                var adicionados = ViajantesId.Where(x => !(viajantesAntigos.Select(y => y.EmpregadoId)).Contains(x)).ToList();

                removidos.ForEach(x => db.ViajanteSolicitacao.Remove(x));
                adicionados.ForEach(x =>
                {
                    ViajanteSolicitacao viajanteSolicitacao = new ViajanteSolicitacao();
                    viajanteSolicitacao.SolicitacaoViagemId = solicitacaoViagem.Id;
                    viajanteSolicitacao.EmpregadoId = x;
                    db.ViajanteSolicitacao.Add(viajanteSolicitacao);                    
                });

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var empregadosOrdenados = db.Empregados.OrderBy(x => x.Nome).ToList();
            var aprovadores = db.Aprovadores.Select(x => x).Include(x => x.Empregado).
                OrderBy(x => x.Empregado.Nome).ToList();
            ViewBag.DestinoId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.DestinoId);
            ViewBag.OrigemId = new SelectList(db.Locals, "Id", "Nome", solicitacaoViagem.OrigemId);
            ViewBag.TransporteId = new SelectList(db.Transportes, "Id", "Nome", solicitacaoViagem.TransporteId);
            ViewBag.EmpregadoId = new SelectList(empregadosOrdenados, "Id", "Nome", solicitacaoViagem.EmpregadoId);
            ViewBag.AprovadorId = new SelectList(aprovadores, "Id", "Empregado.Nome", AprovadorId);
            ViewBag.ViajantesId = new SelectList(empregadosOrdenados, "Id", "Nome");

            return View(solicitacaoViagem);
        }

        // GET: SolicitacaoViagems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitacaoViagem solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == id).
                                                     Include(x => x.Empregado).FirstOrDefault();
            if (solicitacaoViagem == null)
            {
                return HttpNotFound();
            }
            return View(solicitacaoViagem);
        }

        // POST: SolicitacaoViagems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            

            SolicitacaoViagem solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == id).Include(s => s.Destino)
                                                                                              .Include(s => s.Origem)
                                                                                              .Include(s => s.Transporte)
                                                                                              .Include(s => s.Empregado)
                                                                                              .Include(s => s.AprovadorSolicitacaoId)
                                                                                              .Include(s => s.ViajanteSolicitacaoId).FirstOrDefault();
            List<SolicitacaoViagem> lvSolicitation = new List<SolicitacaoViagem>();
            lvSolicitation.Add(solicitacaoViagem);
            FillObjects(lvSolicitation);

            var viajantesAntigos = db.ViajanteSolicitacao.Where(x => x.SolicitacaoViagemId == solicitacaoViagem.Id).ToList();
            viajantesAntigos.ForEach(x => db.ViajanteSolicitacao.Remove(x));

            var StatusSolicitacaoAntigos = db.AprovadorSolicitacao.Where(x => x.SolicitacaoViagemId == solicitacaoViagem.Id).ToList();
            StatusSolicitacaoAntigos.ForEach(x => db.AprovadorSolicitacao.Remove(x));

            var contatosAntigos = db.Contatos.Where(x => x.SolicitacaoViagemId == solicitacaoViagem.Id).ToList();
            contatosAntigos.ForEach(x => db.Contatos.Remove(x));

            db.SolicitacaoViagems.Remove(solicitacaoViagem);

            db.SaveChanges();
            return RedirectToAction("Index");
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
