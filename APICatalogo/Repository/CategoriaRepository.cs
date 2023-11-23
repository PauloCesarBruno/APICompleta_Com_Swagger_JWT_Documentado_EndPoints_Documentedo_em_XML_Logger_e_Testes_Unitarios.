using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriaParameters)
    {
        return await PagedList<Categoria>.ToPagedList(Get().OrderBy(oc => oc.CategoriaId),
                           categoriaParameters.PageNumber,
                           categoriaParameters.PageSize);
    }

    public async Task<PagedList<Categoria>> GetCategoriasProdutos(CategoriasParameters categoriaParameters)
    {
        return await PagedList<Categoria>.ToPagedList(Get().Include(x => x.Produtos).OrderBy(oc => oc.Nome),
                          categoriaParameters.PageNumber,
                          categoriaParameters.PageSize);
    }
}

