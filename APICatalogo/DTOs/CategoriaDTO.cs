using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "Campo Obrigatório !")]
    [StringLength(50, ErrorMessage = "O Nome deve ter entre 05 e 50 caracteres", MinimumLength = 5)]
    [PrimeiraLetraMaiuscula] // Validador Customizado
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo Obrigatório!")]
    [StringLength(500)]
    public string ImagemUrl { get; set; }
               
    public ICollection<ProdutoDTO> Produtos { get; set; }
}
