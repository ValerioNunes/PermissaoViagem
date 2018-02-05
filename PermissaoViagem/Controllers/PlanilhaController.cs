using PermissaoViagem.DAL;
using PermissaoViagem.Extension;
using PermissaoViagem.Extensions;
using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PermissaoViagem.Controllers
{
    public class PlanilhaController : Controller
    {
        private PermissaoViagemContext db = new PermissaoViagemContext();
        // GET: Planilha
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            string[] validFileTypes = { ".xls", ".xlsx" };
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Files"), fileName);

                    if (fileName.ToLower().Contains(validFileTypes[0]) || fileName.ToLower().Contains(validFileTypes[1]))
                    {
                        file.SaveAs(path);
                        Boolean result = this.GetData(path);
                        System.IO.File.Delete(path);
                        ViewBag.Status = result ? "Load_ok" : "Load_fail";
                        ViewBag.Message = result ? "Carregado com sucesso!" : "Erro ao carregar o arquivo!";
                    }
                    else
                    {
                        ViewBag.Status = "Load_wrong";
                        ViewBag.Message = "O arquivo selecionado não está em um formato válido! (xlsx ou xls)";
                    }
                    System.IO.File.Delete(path);
                }
                else
                {
                    ViewBag.Status = "Load_wrong";
                    ViewBag.Message = "Você não selecionou um arquivo!";
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex.Message);
                ViewBag.Status = "Load_fail";
                ViewBag.Message = "Erro ao carregar o arquivo!";
                return View("Index");
            }

        }

        private bool GetData(string path)
        {
            try
            {
                string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"", path);
                DataTable dt = Utility.ConvertXLSXtoDataTable(connString, "Base$");
                List<Empregado> empregados = new List<Empregado>();

                foreach (DataRow linha in dt.Rows)
                {
                    var matricula       = linha[3].ToString();
                    var nome            = linha[8].ToString();
                    var email           = linha[20].ToString();
                    var gerencia        = linha[40].ToString();
                    var supervisao      = linha[41].ToString();
                    var nivelgerencial  = linha[25].ToString();

                    if (!string.IsNullOrEmpty(matricula) && !string.IsNullOrEmpty(nome))
                    {
                        Empregado empregado = new Empregado();
                            empregado.Id                = Int32.Parse(matricula);
                            empregado.Nome              = nome;
                            empregado.Email             = email;
                            empregado.Gerencia          = gerencia;
                            empregado.Supervisao        = supervisao; 
                            empregado.NivelGerencial    = nivelgerencial;
                        empregados.Add(empregado);
                    }
                }

                empregados.ForEach(x =>
                {                   
                    var dadosAntigos = db.Empregados.Where(y => y.Id == x.Id).FirstOrDefault();

                    if (dadosAntigos != null)
                    {
                        dadosAntigos.Nome = x.Nome;
                        dadosAntigos.Email = x.Email;
                        dadosAntigos.Gerencia = x.Gerencia;
                        dadosAntigos.Supervisao = x.Supervisao;
                        dadosAntigos.NivelGerencial = x.NivelGerencial;
                    }
                    else
                    {
                        db.Empregados.Add(x);
                    }
                });

                db.SaveChanges();
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
                DebugLog.Logar(Utility.Details(e));
                return false;
            }
            return true;
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