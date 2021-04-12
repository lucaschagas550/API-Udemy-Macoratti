using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Repository.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        [Required] // se quiser aparecer o que ocorreu de errado fazer a validação pelos DTOS
        [StringLength(40, ErrorMessage = "A descrição deve ter no maximo 40 caracteres", MinimumLength = 5)]
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string ImagemUrl { get; set; }
        public int CategoriaId { get; set; } 
    }
}
