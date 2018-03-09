using PermissaoViagem.DAL;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace PermissaoViagem.Extension
{
    public class Mensagem 
    {

        System.Net.Mail.SmtpClient client;
        private PermissaoViagemContext db = new PermissaoViagemContext();
        SolicitacaoViagem SV;

        public Boolean HabilitadoEmail { get; set; }
        public Boolean HabilitadoSMS   { get; set; }

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

            if(!HabilitadoSMS)
            { 
                        Thread t = new Thread(ParaAprovadorAnalisar);
                        t.Start();
            }
            else
            {
                ParaAprovadorAnalisar();
            }
        }

        public void MudancaDeStatus(int id)
        {
            SV = Solicitacao(id).FirstOrDefault();

            if (!HabilitadoSMS)
            {
                Thread t = new Thread(ParaAprovadorStatus);
                t.Start();
            }
            else
            {
                ParaAprovadorStatus();
            }
        }

        void Page_Load(List<Empregado> Empregados, String msg)
        {
            DebugLog.Logar("Page_Load(List<Empregado> Empregados, String msg)");


            if (HabilitadoSMS)
                SMS(Empregados, msg);

            if (HabilitadoEmail)
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
                             "Você é um dos Viajantes da Solicitação de Viagem - SV : " + SV.Id + " <br/>" +
                             "De : " + SV.Origem.Nome + "<br/> " +
                             "Para :  " + SV.Destino.Nome + "<br/>" +
                             "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>" +
                             "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>" +
                             "Transporte : " + SV.Transporte.Nome + "<br/>" +
                             "Solicitante : " + SV.Empregado.Nome + "<br/>" +
                             "<h2>Status de Viagem : " + aprovadorSolicitacao.Status.Nome.ToUpper() + "  -  Data: " + aprovadorSolicitacao.DataStatus.ToLocalTime().ToString() + "</h2>";
                DebugLog.Logar(msg);
                List<Empregado> Empregados = SV.ViajanteSolicitacaoId.Select(e => e.Empregado).ToList();

                Page_Load(Empregados, msg);

            }

        }

        void ParaAprovadorAnalisar()
        {
            if (SV != null)
            {
                try
                {
                    Empregado Aprovador = db.Empregados.Find(547492);

                    String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                                 "Oi, " + Aprovador.Nome + "<br/>" +
                                 "Você tem uma nova Solicitação de Viagem para Analisar - SV :" + SV.Id + " <br/>" +
                                 "De : " + SV.Origem.Nome + "<br/> " +
                                 "Para :  " + SV.Destino.Nome + "<br/>" +
                                 "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>" +
                                 "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>" +
                                 "Transporte : " + SV.Transporte.Nome + "<br/>" +
                                 "Solicitante : " + SV.Empregado.Nome + "<br/>" +
                                 "";

                    DebugLog.Logar(msg);

                    List<Empregado> Empregados = new List<Empregado>();
                    Empregados.Add(Aprovador);

                    Page_Load(Empregados, msg);

                    ParaViajantes();
                }catch (Exception e)
                {
                    DebugLog.Logar("ParaAprovadorAnalisar() ln: 140"+e.Data );
                }
            }
            else {
                DebugLog.Logar("Solicitação viagem NULL ln: 140 Mensagem");
            }

        }

        void ParaAprovadorStatus()
        {
            if (SV != null)
            {
                try
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
                catch (Exception e)
                {
                    DebugLog.Logar("ParaAprovadorStatus() ln: 182" + e.Data);
                }
            }
            else
            {
                DebugLog.Logar("Solicitação viagem NULL ln: 187 Mensagem.cs");
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
        
        private void SMS(List<Empregado> Empregados, String msg) {

            const string accountSid = "ACe8df82ae66422256f834d0d0e4fee440";
            const string authToken =  "5d472ca642930c7b5e524bcaaadd53fe";
           
            String SMS = msg.Replace("<h1>", " ").Replace("</h1>", " ").Replace("<br/>", " ").Replace("<h2>", " ").Replace("</h2>", " ");
            DebugLog.Logar(SMS);
            Empregados.ForEach(x =>
            {
                    if (x.Telefone != null) {
    
                            try
                            {
                            // Find your Account Sid and Auth Token at twilio.com/user/account 
                            TwilioClient.Init(accountSid, authToken);

                            var message = MessageResource.Create(
                                new PhoneNumber("+55" + x.Telefone),
                                from: new PhoneNumber("+19524795118"),
                                body: "Permissão de Viagem: +1 Notificação"
                            );
                            }
                            catch (Exception ex)
                            {
                                DebugLog.Logar(ex.Message);
                            }
                        }
            });

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