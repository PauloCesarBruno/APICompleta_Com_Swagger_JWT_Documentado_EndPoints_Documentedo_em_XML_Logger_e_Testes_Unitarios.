using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(30, ErrorMessage = "O Nome deve ter entre 05 e 20 caracteres", MinimumLength = 5)]
        [PrimeiraLetraMaiuscula] // Validador Customizado
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(100, ErrorMessage = "A Descrição deve ter entre 15 e 100 caracteres", MinimumLength = 15)]
        [PrimeiraLetraMaiuscula] // Validador Customizado
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 10000, ErrorMessage = "O Proço deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(100, ErrorMessage = "A ImagemURL deve ter entre 15 e 100 caracteres", MinimumLength = 15)]
        public string ImagemUrl { get; set; }

        //[Required(ErrorMessage = "Campo Obrigatório!")]  --> ESTÁ COMENTADO PARA NÃO TRAZER NO DTO MESMO 
        //public float Estoque { get; set; }

        //[Required(ErrorMessage = "Campo Obrigatório!")]  -->  ESTÁ COMENTADO PARA NÃO TRAZER NO DTO MESMO        
        //public DateTime DataCadastro { get; set; }
                                   
        [Required(ErrorMessage = "Campo Obrigatório!")]
        public int CategoriaId { get; set; }
        
    }
}
