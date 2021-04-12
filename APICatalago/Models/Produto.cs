using APICatalago.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Models
{
    [Table("Produtos")]
    public class Produto/* : /*IValidatableObject*/
    {
        [Key] //data anotacion
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(80,ErrorMessage = "O nome deve ter entre 5 e 20 caracteres", MinimumLength = 5)]
       //    [PrimeiraLetraMaiuscula] // atributo customizado 
        public string Nome { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "A descrição deve ter no maximo 40 caracteres", MinimumLength = 5)]
        public string Descricao { get; set; }

        [Required]
        [DataType(DataType.Currency)] // permite definir o dado 
        [Column(TypeName ="decimal(8,2)")] // define o tamnho do dado
        [Range(1,10000, ErrorMessage ="O preço deve estar entre {1} e {2}" )]
        public decimal Preco { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }


        //lucas ideia, criar uma classe e essa classe ser herdada aqui para chamar um metodo de validação para desacoplar o codigo
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) // validação a nivel de modelo 
        //{
        //    if (!string.IsNullOrEmpty(this.Nome))
        //    {
        //        var primeiraLetra = this.Nome[0].ToString();
        //        if (primeiraLetra != primeiraLetra.ToUpper())
        //        {
        //            yield return new
        //                ValidationResult("A primeira letra do produto deve ser maiuscula",
        //                new[]
        //                {
        //                    nameof(this.Nome)
        //                });
        //        }
        //    }

        //    if (this.Estoque <= 0)
        //    {
        //        yield return new
        //              ValidationResult("O estoque deve ser maior que zero",
        //              new[]
        //              {
        //                    nameof(this.Estoque)
        //              });
        //    }


        //    if (this.Descricao.Length > 40)
        //    {
        //        yield return new
        //              ValidationResult("Descrição maior que 40 caracteres",
        //              new[]
        //              {
        //                    nameof(this.Descricao)
        //              });
        //    }
        //}
    }
}
