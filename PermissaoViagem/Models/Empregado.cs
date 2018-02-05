using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbempregado")]
    public class Empregado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key, Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public String Nome { get; set; }
        [Column("email")]
        public String Email  { get; set; }
        [Column("gerencia")]
        public String Gerencia { get; set; }
        [Column("supervisao")]
        public String Supervisao { get; set; }
        [Column("managereriallevel")]
        public String NivelGerencial  { get; set; }

        //public ICollection<ViajanteSolicitacao> ViajanteSolicitacao { get; set; }
    }
}