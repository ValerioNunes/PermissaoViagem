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
                new Status {Id = 1, Nome = "AGUARDANDO"},
                new Status {Id = 2, Nome = "NAO_APROVADO"},
                new Status {Id = 3, Nome = "APROVADO"}
            };

            var TransporteDefault = new List<Transporte>
            {
                 new Transporte{ Id   = 1,Nome = "CARRO" },
                 new Transporte{ Id   = 2,Nome = "ÔNIBUS" },
                 new Transporte{ Id   = 3,Nome = "TREM" },
                 new Transporte{ Id   = 4,Nome = "MAQUINA DE VIA"},
                 new Transporte{ Id   = 5,Nome = "A PÉ"}
            };

            var LocalDefault = new List<Local>
            {
                new Local{ Id= 1, Nome = "Açailândia"},
                new Local{ Id= 2, Nome = "Altamira"},
                new Local{ Id= 3, Nome = "Alto Alegre"},
                new Local{ Id= 4, Nome = "Auzilândia"},
                new Local{ Id= 5, Nome = "Itainopólis"},
                new Local{ Id= 6, Nome = "Marabá"},
                new Local{ Id= 7, Nome = "Mineirinho"},
                new Local{ Id= 8, Nome = "Nova Vida"},
                new Local{ Id= 9, Nome = "Parauapebas"},
                new Local{ Id= 10, Nome = "Presa de Porco"},
                new Local{ Id= 11, Nome = "Santa Inês"},
                new Local{ Id= 12, Nome = "São Luis"},
                new Local{ Id= 13, Nome = "São Pedro Água Branca"},
                new Local{ Id= 14, Nome = "Vitória do Mearim"},
                new Local{ Id= 15, Nome = "Arari"},
                new Local{ Id= 16, Nome = "Vila Pindaré"}
            };
                
            StatusDefault.ForEach(x => context.Status.Add(x));
            TransporteDefault.ForEach(x => context.Transportes.Add(x));
            LocalDefault.ForEach(x => context.Locais.Add(x));

            context.SaveChanges();
        }
    }
}