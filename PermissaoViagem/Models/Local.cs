﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tblocal")]
    public class Local
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public String Nome{ get; set; }
    }
}