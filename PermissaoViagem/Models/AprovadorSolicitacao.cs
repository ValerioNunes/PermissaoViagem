using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbaprovador_solicitacaoviagem")]
    public class AprovadorSolicitacao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("idsolicitacaoviagem")]
        public int SolicitacaoViagemId { get; set; }
        public SolicitacaoViagem SolicitacaoViagem { get; set; }

        [Column("datastatus")]
        public DateTime  DataStatus { get; set; }

        [Column("idstatus")]
        public int StatusId  { get; set; }
        public Status Status { get; set; }

        [Column("idaprovador")]
        public int AprovadorId { get; set; }
        public Aprovador Aprovador { get; set;}

    }
}