using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
//using APICatalogo.Services;  // Usado para o exemplo de Saudação no [FromServices].
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CategoriasController : Controller
{
    private readonly IUnitOfWork _context;
    //private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    public CategoriasController(IUnitOfWork context, /*IConfiguration configuration,*/
        ILogger<CategoriasController> logger, IMapper mapper)
    {
        _context = context;
        // _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    /* Criei um Midware no appsettings.json chamado autor, injetei Iconfiguration
                       * p/ obter as informações de autor e connecionString*/
    //[HttpGet("autor")]
    //public string GetAutor()
    //{
    //    var autor = _configuration["autor"];
    //    var conexao = _configuration["ConnectionStrings:DefaultConnection"];
    //    return $"Autor: {autor} Conexao: {conexao}";  
    //}

    //EXEMPLO DE USO DE INTERFACE DE SERVICO (SERVICES) PARA SAUDAÇÃO - FINS DIDÁTICOS
    //[HttpGet("saudacao/{nome}")]
    //public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome)
    //{
    //    return meuServico.Saudacao(nome);
    //}

    //========================================================================================================

    /// <summary>
    /// EndPoint para listar as categorias com os seus respectivos produtos por ordem do nome do produto.
    /// </summary>
    /// <param name="categoriaParameters"></param>
    /// <returns></returns>
    [HttpGet("produtos")]
    public async Task<ActionResult<IList<CategoriaDTO>>>
    GetCategoriasProdutos([FromQuery] CategoriasParameters categoriaParameters)
    {
        var categorias = await _context.CategoriaRepository
                        .GetCategoriasProdutos(categoriaParameters);

        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


        var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
        return categoriasDto;
    }

    // (GetCategoriasProdutos) Sem Paginação:
    //[HttpGet("produtos")]
    //public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
    //{
    //    var categorias = await _context.CategoriaRepository
    //                    .GetCategoriasProdutos();

    //    var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
    //    return categoriasDto;
    //}

    //========================================================================================================

    /// <summary>
    /// Endpoint para listar todas as categorias por ordem do Id.
    /// </summary>
    /// <param name="categoriasParameters"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>>
            Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _context.CategoriaRepository.
                            GetCategorias(categoriasParameters);
        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
        return categoriasDto;
    }

    /// <summary>
    /// EndPoint para listar uma categoria pelo seu ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        //_logger.LogInformation($"======================GET api/categorias/id = {id} =======================");

        var categoria = await _context.CategoriaRepository
                                .GetById(c => c.CategoriaId == id);

        if (categoria == null)
        {
            //_logger.LogInformation($"======================GET api/categorias/id = {id} =======================");

            return NotFound("A categoria de código " + id + " não foi encontrada");
        }

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return categoriaDTO;
    }

    /// <summary>
    /// EndPoint para add. uma nova categoria.
    /// </summary>
    /// <param name="categoriaDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _context.CategoriaRepository.Add(categoria);
        await _context.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return new CreatedAtRouteResult("ObterCategoria",
            new { id = categoria.CategoriaId }, categoriaDTO);
    }

    /// <summary>
    /// EndPoint para alterar uma categoria.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoriaDto"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _context.CategoriaRepository.Update(categoria);
        await _context.Commit();

        return Ok();
    }

    /// <summary>
    /// Endpoint para deletar uma categoria.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _context.CategoriaRepository
                        .GetById(c => c.CategoriaId == id);

        if (categoria == null)
        {
            return NotFound("A Categoria de código " + id + " nõo foi encontrada!");
        }
        _context.CategoriaRepository.Delete(categoria);
        await _context.Commit();

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return categoriaDto;
    }
}
