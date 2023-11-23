using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogoxUnitTests;


// Classe para inicializar Um outro banco de dados 
// somente para os testes ou usar dadsos inmémory.
// NÃO ESTOU FAZENDO ASIM, ESTOU USANDO MEU BANCO ORIGINAL PARA OS TESTES.
//======================================================================================
public class VersaoDbUnitTestsMockInitialize
{
    public VersaoDbUnitTestsMockInitialize()
    { }

    public void Seed(AppDbContext context)
    {
        context.Categorias.Add
        (new Categoria { CategoriaId = 999, Nome = "Bebida999", ImagemUrl = "Bebida999.Jpg" });

        context.Categorias.Add
        (new Categoria { CategoriaId = 2, Nome = "Sucos", ImagemUrl = "Sucos1.Jpg" });

        context.Categorias.Add
        (new Categoria { CategoriaId = 3, Nome = "Doces", ImagemUrl = "Doces1.Jpg" });

        context.Categorias.Add
        (new Categoria { CategoriaId = 4, Nome = "Salgados", ImagemUrl = "Salgados1.Jpg" });

        context.Categorias.Add
        (new Categoria { CategoriaId = 5, Nome = "Tortas", ImagemUrl = "Tortas1.Jpg" });

        context.Categorias.Add
        (new Categoria { CategoriaId = 6, Nome = "Bolos", ImagemUrl = "Bolo1.Jpg" });
    }
}