using PermissaoViagem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PermissaoViagem.DAL
{
    public class PermissaoViagemInitializer
    {

        public static void Seed(PermissaoViagemContext context)
        {
            var StatusDefault = new List<Status>
            {
                new Status {Id = 1, Nome = "aguardando"},
                new Status {Id = 2, Nome = "nao_aprovado"},
                new Status {Id = 3, Nome = "aprovado"},
                 new Status{Id = 4, Nome = "encerrado"}
            };

            var TransporteDefault = new List<Transporte>
            {
                 new Transporte{ Id   = 1,Nome = "CARRO" },
                 new Transporte{ Id   = 2,Nome = "ÔNIBUS" },
                 new Transporte{ Id   = 3,Nome = "TREM" },
                 new Transporte{ Id   = 4,Nome = "MAQUINA DE VIA"},
                 new Transporte{ Id   = 5,Nome = "A PÉ"}
            };

           
            StatusDefault.ForEach(x => context.Status.Add(x));
            TransporteDefault.ForEach(x => context.Transportes.Add(x));
           

            context.SaveChanges();
        }
    }
}