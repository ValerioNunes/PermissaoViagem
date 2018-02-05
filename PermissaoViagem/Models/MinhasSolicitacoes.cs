using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    public class MinhasSolicitacoes
    {
        public int Id { get; set; }
        public String Aprovador { get; set; }
        public String Origem { get; set; }
        public String Destino { get; set; }
        public String Transporte { get; set; }
        public DateTime Partida { get; set; }
        public DateTime Chegada { get; set; }
        public String Solicitante { get; set; }
        public String Status { get; set; }
    }
}