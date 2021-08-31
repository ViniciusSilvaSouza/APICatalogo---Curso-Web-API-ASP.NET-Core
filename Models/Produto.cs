using APICatalogo.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : IValidatableObject
    {
        [Key]
        public int ProdutoId { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} e no mínimo {2} caracteres", MinimumLength = 5)]
        [PrimeiraLetraMaiuscula]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageURL { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.Nome))
            {
                var primeiraLetra = this.Nome[0].ToString();
                if (primeiraLetra != primeiraLetra.ToUpper())
                    yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula", new[] { nameof(this.Nome) });
            }

            if (this.Estoque <= 0)
            {
                yield return new ValidationResult("O estoque deve ser maior que zero", new[] { nameof(this.Estoque) });
            }
        }
    }
}
