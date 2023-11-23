using APICatalogo.Validations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        public Categoria()
        {
            // Construtor para inicialização da classe Produto
            Produtos = new Collection<Produto>();
        }

        [Key]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório !")]
        [StringLength(50, ErrorMessage = "O Nome deve ter entre 05 e 50 caracteres", MinimumLength = 5)]
        [PrimeiraLetraMaiuscula] // Validador Customizado
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(500)]
        public string ImagemUrl { get; set; }
                    
        public ICollection<Produto> Produtos { get; set; }
    }
}
