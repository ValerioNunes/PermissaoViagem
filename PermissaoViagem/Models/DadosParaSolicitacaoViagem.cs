using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Models
{
    public class DadosParaSolicitacaoViagem
    {
        public int Aprovador; 
        public int Origem;
        public int Destino;
        public int Solicitante;
        public int Transporte;

        public String Chegada;
        public String Partida;

        public List<String> Contatos;
        public List<int> Viajantes;

    }
}