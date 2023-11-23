using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTests.TestMokControllers;

public class ProdutosMockController : ControllerBase
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public ProdutosMockController(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // ===================================================================================================
    // TESTES DE CONSULTAS
    // ===================================================================================================

    [HttpGet] // Para teste de retorno de uma coleção de objetos
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>>
            Get([FromQuery] ProdutosParameters produtosParameters)
    {
        try
        {
            var produtos = await _context.ProdutoRepository
                          .GetProdutos(produtosParameters);
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("badrequest")] // Para teste (FORÇAR UM BADREQUEST) como retorto
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>>
               GetBadProd([FromQuery] ProdutosParameters produtosParameters)
    {
        try
        {
            var produtos = await _context.ProdutoRepository
                            .GetProdutos(produtosParameters);
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtossDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            throw new Exception();
            // return categoriasDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("bd")] // Teste para retornar o 1º e o 3º Registro do meu Banco de Dados.
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>>
              GetBd([FromQuery] ProdutosParameters produtosParameters)
    {
        try
        {
            var produtos = await _context.ProdutoRepository.
                           GetProdutos(produtosParameters);
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("{id}", Name = "ObterProduto")] /*Este método "Mock" serve paara testar 2 (duas)
    situações, uma é se vai retornar um result de um Objeto pelo seu Id e a outra situação é ver
    se vai retornar um "NotFopud" se for passado um Id Inexistente.*/
    public async Task<ActionResult<ProdutoDTO>> GetById(int id)
    {
        var produto = await _context.ProdutoRepository
                        .GetById(p => p.ProdutoId == id);
        if (produto == null)
        {

            return NotFound();
        }

        var produtosDTO = _mapper.Map<ProdutoDTO>(produto);

        return produtosDTO;
    }

    /// ===================================================================================================
    // TESTES DE MANIPULAÇÃ - POST / PUT / DELETE   
    // ===================================================================================================

    [HttpPost] /* Este método Post tera que retornar no teste um CreatedAtRouteResult,
    além de persistir o dado no Banco de dados*/
    public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
    {
        var produto = _mapper.Map<Produto>(produtoDto);

        _context.ProdutoRepository.Add(produto);
        await _context.Commit();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return new CreatedAtRouteResult("ObterProduto",
            new { id = produto.ProdutoId }, produtoDTO);
    }

    [HttpPut("{id}")]  /* Este método Put tera que Alterar um produto,
    retornar um StatusCode 200, além de persistir o dado no Banco de dados*/
    public ActionResult Put(int id, [FromBody] ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest();
        }

        var produto = _mapper.Map<Produto>(produtoDto);

        _context.ProdutoRepository.Update(produto);
        _context.Commit();

        return Ok();
    }

    [HttpDelete("{id}")] /* Este método Delete tera que Excluir um produto,
    retornar o Objeto excluido, além de persistir o dado no Banco de dados*/
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _context.ProdutoRepository
                        .GetById(p => p.ProdutoId == id);

        if (produto == null)
        {
            return NotFound();
        }
        _context.ProdutoRepository.Delete(produto);
        await _context.Commit();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }
}