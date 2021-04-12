using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Validations
{
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute //attributte para indentificar que sera um atributo
    {
        protected override ValidationResult IsValid(object value, //Value é o valor da propriedade no caso o NOME
                                                    ValidationContext validationContext) // ValidationContext é no caso a classe produto
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return base.IsValid(value, validationContext);
            }

            var primeiraLetra = value.ToString()[0].ToString(); // pega a primeira letra 
            if(primeiraLetra != primeiraLetra.ToUpper())
            {
                return new ValidationResult("A primeira letra do nome do produto deve ser maiuscula");
            }

            return ValidationResult.Success;
        }
    }
}
