using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProdutosController : ControllerBase
{
    //Injeção de dependência:
    private readonly IUnitOfWork _uof;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uof"></param>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger, IMapper mapper)
    {
        _uof = uof;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Endpoint para listar todos os produtos em ordem de "menor preço".
    /// </summary>
    /// <param name="produtosParameters"></param>
    /// <returns></returns>
    [HttpGet("menorpreco")]
    public async Task<ActionResult<IList<ProdutoDTO>>>
    GetProdutosPorPreco([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco(produtosParameters);

        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
        return produtosDto;
    }

    //========================================================================================================

    // (GetProdutosPorPreco) Sem Paginação:
    //[HttpGet("menorpreco")]
    //public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
    //{
    //    var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
    //    var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

    //    return produtosDto;
    //}

    //========================================================================================================

    /// <summary>
    /// Endpoint para listar todos os produtos.
    /// </summary>
    /// <param name="produtosParameters"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>>
           Get([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
        return produtosDto;
    }

    /// <summary>
    /// EndPoint para listar um produto pelo seu ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // Rota Nomeada para obter Status 201 no Post.
    [HttpGet("{id}", Name = "ObterProduto")] // Restrição de rota ->  [HttpGet("{id:int:min(1)}"
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _uof.ProdutoRepository
                            .GetById(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("O Produto de Código " + id + " não foi encontrado !");
        }
        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return produtoDTO;
    }

    /// <summary>
    /// Endpoint para add. um produto.
    /// </summary>
    /// <param name="produtoDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
    {
        var produto = _mapper.Map<Produto>(produtoDto);

        _uof.ProdutoRepository.Add(produto);
        await _uof.Commit();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                   
        return new CreatedAtRouteResult("ObterProduto",
               new { id = produto.ProdutoId }, produtoDTO);
    }

    /// <summary>
    /// EndPoint para alterar um produto.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="produtoDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest();
        }

        var produto = _mapper.Map<Produto>(produtoDto);

        _uof.ProdutoRepository.Update(produto);
        await _uof.Commit();

        return Ok();
    }

    /// <summary>
    /// EndPint para deletar um produto.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if (produto == null)
        {
            return NotFound("Produto de código " + id + " não foi localizado!");
        }
        _uof.ProdutoRepository.Delete(produto);
        await _uof.Commit();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }
}