using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbstatus")]
    public class Status
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("status")]
        public String Nome { get; set; }

       
    }
}