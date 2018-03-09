using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    public class DadosSolicitarViagemViewModel
    {
        public Empregado Solicitante { get; set; }
        public List<Empregado> Aprovador { get; set; }
        public List<Transporte> Transporte { get; set; }
        public List<Local> Local { get; set; }
        public List<Status> Status { get; set; }
    }
}