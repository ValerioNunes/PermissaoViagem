using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbviajante_solicitacaoviagem")]
    public class ViajanteSolicitacao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("idviajante")]
        public int EmpregadoId { get; set; }
        public Empregado Empregado { get; set; }

        [Column("idsolicitacaoviagem")]
        public int SolicitacaoViagemId { get; set; }
        public SolicitacaoViagem SolicitacaoViagem { get; set; }


    }
}