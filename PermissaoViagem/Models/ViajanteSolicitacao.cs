using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbviajante_solicitacaoviagem")]
    public class ViajanteSolicitacao
    {     
        [Key]
        [Column("idviajante", Order = 1)]
        public int EmpregadoId { get; set; }
        public Empregado Empregado { get; set; }

        [Key]
        [Column("idsolicitacaoviagem", Order = 2)]
        public int SolicitacaoViagemId { get; set; }
        public SolicitacaoViagem SolicitacaoViagem { get; set; }

    }
}