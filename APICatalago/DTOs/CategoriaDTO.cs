using APICatalago.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Repository.DTOs
{
    public class CategoriaDTO // DTO = DATA TRANSFER OBJECT, DADOS QUE O CLIENTE QUER VER, NAO MOSTRANDO TUDO QUE EXISTE NO BANCO
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string ImagemUrl { get; set; }
        public ICollection<ProdutoDTO> Produtos { get; set; }
    }
}
