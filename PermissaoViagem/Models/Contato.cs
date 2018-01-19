using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbcontato")]
    public class Contato
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("telefone")]
        public String  Telefone { get; set;}

        [Column("idsolicitacaoviagem")]
        public int SolicitacaoViagemId { get; set; }
        public SolicitacaoViagem SolicitacaoViagem { get; set; }


    }
}