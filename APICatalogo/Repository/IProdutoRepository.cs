using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    //Contrato para paginação:
    Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters);
    Task<PagedList<Produto>> GetProdutosPorPreco(ProdutosParameters produtosParameters);
}
