using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    [Table("tbsolicitacaoviagem")]
    public class SolicitacaoViagem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("datapartida")]
        public DateTime DataPartida { get; set; }

        [Column("datachegadaprevista")]
        public DateTime DataChegadaPrevista { get; set; }

        [Column("idtransporte")]
        public int        TransporteId { get; set; }
        public Transporte Transporte { get; set;}

        [Column("idorigem")]
        public int   OrigemId { get; set; }
        public Local Origem { get; set; }

        [Column("iddestino")]
        public int   DestinoId { get; set; }
        public Local Destino   { get; set; }

        [Column("idsolicitante")]
        public int EmpregadId { get; set; }
        public Empregado Empregado { get; set; }
        
             
        public ICollection<ViajanteSolicitacao>  ViajanteSolicitacao  { get; set; }



        public ICollection<AprovadorSolicitacao> AprovadorSolicitacao { get; set; }

    }
}