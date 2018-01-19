using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using PermissaoViagem.Models;

namespace PermissaoViagem.DAL
{
    public class PermissaoViagemContext : DbContext
    {
        public PermissaoViagemContext() : base("name=PermissaoViagemContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public virtual IDbSet<Transporte> Transportes { get; set; }   
        public virtual IDbSet<Status> Status { get; set; }      
        public virtual IDbSet<ViajanteSolicitacao>  ViajanteSolicitacao  { get; set; }
        public virtual IDbSet<AprovadorSolicitacao> AprovadorSolicitacao { get; set; }
        public IDbSet<PermissaoViagem.Models.Contato> Contatos { get; set; }
        public IDbSet<PermissaoViagem.Models.SolicitacaoViagem> SolicitacoesViagem { get; set; }
        public System.Data.Entity.DbSet<PermissaoViagem.Models.Local> Locais { get; set; }
        public System.Data.Entity.DbSet<PermissaoViagem.Models.Aprovador> Aprovadores { get; set; }
        public System.Data.Entity.DbSet<PermissaoViagem.Models.Empregado> Empregados { get; set; }
    }
}