using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public interface ICategoriaRepository : IRepository<Categoria>
{
    //Contrato para paginação:
    Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriaParameters);

    Task<PagedList<Categoria>> GetCategoriasProdutos(CategoriasParameters categoriaParameters);
}
