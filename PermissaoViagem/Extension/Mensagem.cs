using PermissaoViagem.DAL;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace PermissaoViagem.Extension
{
    public class Mensagem
    {

        System.Net.Mail.SmtpClient client;
        private PermissaoViagemContext db = new PermissaoViagemContext();
        SolicitacaoViagem SV;

        Boolean Habilitado = false;

        public Mensagem()
        {
            client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("permissaoviagem@gmail.com", "solicitacaoviagem");
        }

        public void AguardandoAnalise(int id)
        {
            SV = Solicitacao(id).FirstOrDefault();
            Thread t = new Thread(ParaAprovadorAnalisar);
            t.Start();
        }

        public void MudancaDeStatus(int id)
        {
            SV = Solicitacao(id).FirstOrDefault();
            Thread t = new Thread(ParaAprovadorStatus);
            t.Start();
        }

        void Page_Load(List<Empregado> Empregados, String msg)
        {

            if (Habilitado)
            {
                MailMessage mail = new MailMessage();
                mail.Sender = new System.Net.Mail.MailAddress("permissaoviagem@gmail.com", "Permissão Viagem");
                mail.From = new MailAddress("permissaoviagem@gmail.com", "Permissão Viagem");

                Empregados.ForEach(x =>
                {
                    mail.To.Add(new MailAddress(x.Email, x.Nome));

                });

                mail.Subject = "Permissão Viagem";

                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                try
                {
                    client.Send(mail);
                }
                catch (System.Exception erro)
                {
                    DebugLog.Logar("Email=>>" + erro);//trata erro
                }
                finally
                {

                    mail = null;
                }
            }

        }
        void ParaViajantes()
        {
            if (SV != null)
            {
                AprovadorSolicitacao aprovadorSolicitacao = SV.AprovadorSolicitacaoId.FirstOrDefault();

                String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                             "Você é um dos Viajantes da Solicitação de Viagem - SV : " + SV.Id + " <br/>"+
                             "De : " + SV.Origem.Nome + "<br/> "+
                             "Para :  " + SV.Destino.Nome + "<br/>"+
                             "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>"+
                             "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>"+
                             "Transporte : " + SV.Transporte.Nome + "<br/>"+
                             "Solicitante : " + SV.Empregado.Nome + "<br/>" +
                             "<h2>Status de Viagem : " + aprovadorSolicitacao.Status.Nome.ToUpper() + "  -  Data: " + aprovadorSolicitacao.DataStatus.ToLocalTime().ToString() + "</h2>";

                List<Empregado> Empregados = SV.ViajanteSolicitacaoId.Select(e => e.Empregado).ToList();
                

                Page_Load(Empregados, msg);

            }

        }

        void ParaAprovadorAnalisar()
        {
            if (SV != null)
            {
                Empregado Aprovador = db.Empregados.Find(547492);

                      String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                                   "Oi, " + Aprovador.Nome + "<br/>" +
                                   "Você tem uma nova Solicitação de Viagem para Analisar - SV :" + SV.Id + " <br/>" +
                                   "De : " + SV.Origem.Nome + "<br/> " +
                                   "Para :  " + SV.Destino.Nome + "<br/>" +
                                   "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>" +
                                   "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>" +
                                   "Transporte : " +  SV.Transporte.Nome+ "<br/>"+
                                   "Solicitante : " + SV.Empregado.Nome + "<br/>"+
                                   "";
                                    
                    
                    List<Empregado> Empregados = new List<Empregado>();
                    Empregados.Add(Aprovador);

                    Page_Load(Empregados, msg);
                    ParaViajantes();
            }
        
        }

        void ParaAprovadorStatus()
        {

            

            if (SV != null)
            {
                Empregado Aprovador = db.Empregados.Find(547492);
                AprovadorSolicitacao aprovadorSolicitacao = SV.AprovadorSolicitacaoId.FirstOrDefault();

                String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                             "Oi, " + Aprovador.Nome + "<br/>" +
                             "A Solicitação de Viagem - SV :" + SV.Id + " <br/>" +
                             "De : " + SV.Origem.Nome + "<br/> " +
                             "Para :  " + SV.Destino.Nome + "<br/>" +
                             "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>" +
                             "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>" +
                             "Transporte : " + SV.Transporte.Nome + "<br/>" +
                             "Solicitante : " + SV.Empregado.Nome + "<br/>" +
                             "<h2>Status de Viagem : " + aprovadorSolicitacao.Status.Nome.ToUpper() + "-  Data: " + aprovadorSolicitacao.DataStatus.ToLocalTime().ToString() + "</h2>";


                List<Empregado> Empregados = new List<Empregado>();
                Empregados.Add(Aprovador);

                Page_Load(Empregados, msg);
                ParaViajantes();
            }

        }


        List<SolicitacaoViagem> Solicitacao(int id)
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
                return null;
            }


            FillObjects(solicitacaoViagem);
            return solicitacaoViagem;
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