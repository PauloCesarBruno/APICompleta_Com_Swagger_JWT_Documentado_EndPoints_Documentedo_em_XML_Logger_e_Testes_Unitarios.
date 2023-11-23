using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.TestMokControllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasMockController : ControllerBase
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public CategoriasMockController(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // ===================================================================================================
    // TESTES DE CONSULTAS
    // ===================================================================================================
     
    [HttpGet] // Para teste de retorno de uma coleção de objetos
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>>
            Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        try
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

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("badrequest")] // Para teste (FORÇAR UM BADREQUEST) como retorto
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>>
               GetBad([FromQuery] CategoriasParameters categoriasParameters)
    {
        try
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

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            throw new Exception();
            // return categoriasDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("bd")] // Teste para retornar o 1º e o 3º Registro do meu Banco de Dados.
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>>
              GetBd([FromQuery] CategoriasParameters categoriasParameters)
    {
        try
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

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }

    [HttpGet("{id}", Name = "ObterCategoria")] /*Este método "Mock" serve paara testar 2 (duas)
    situações, uma é se vai retornar um result de um Objeto pelo seu Id e a outra situação é ver
    se vai retornar um "NotFopud" se for passado um Id Inexistente.*/
    public async Task<ActionResult<CategoriaDTO>> GetById(int id)
    {
        var categoria = await _context.CategoriaRepository
                                .GetById(c => c.CategoriaId == id);
        if (categoria == null)
        {

            return NotFound();
        }

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return categoriaDTO;
    }

    /// ===================================================================================================
    // TESTES DE MANIPULAÇÃ - POST / PUT / DELETE   
    // ===================================================================================================

    [HttpPost] /* Este método Post tera que retornar no teste um CreatedAtRouteResult,
     além de persistir o dado no Banco de dados*/
    public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _context.CategoriaRepository.Add(categoria);
        await _context.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return new CreatedAtRouteResult("ObterCategoria",
            new { id = categoria.CategoriaId }, categoriaDTO);
    }



    [HttpPut("{id}")]  /* Este método Put tera que Alterar uma categoria,
     retornar um StatusCode 200, além de persistir o dado no Banco de dados*/
    public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _context.CategoriaRepository.Update(categoria);
        _context.Commit();

        return Ok();
    }

    [HttpDelete("{id}")] /* Este método Delete tera que Excluir uma categoria,
     retornar o Objeto excluido, além de persistir o dado no Banco de dados*/
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _context.CategoriaRepository
                        .GetById(c => c.CategoriaId == id);

        if (categoria == null)
        {
            return NotFound();
        }
        _context.CategoriaRepository.Delete(categoria);
        await _context.Commit();

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return categoriaDto;
    }
}


