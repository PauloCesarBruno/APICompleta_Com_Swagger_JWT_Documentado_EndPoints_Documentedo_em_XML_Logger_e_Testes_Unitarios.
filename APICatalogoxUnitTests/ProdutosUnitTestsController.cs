using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogoxUnitTests.TestMokControllers;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogoxUnitTests;

public class ProdutosUnitTestsController
{
    private IUnitOfWork repository;
    private IMapper mapper;

    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    public static string connectionString =
        "Sua String";
                                  
    static ProdutosUnitTestsController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
             .UseSqlServer(connectionString)
             .Options;
    }
   
    public ProdutosUnitTestsController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        
        mapper = config.CreateMapper();
                                       
        var context = new AppDbContext(dbContextOptions);
                                                 
        repository = new UnitOfWork(context);
    }

    // Inicio dos testes Unitário:
    // Casos de teste:
    //=====================================================================================================

    // ===================================================================================================
    // TESTES DE CONSULTAS
    // ===================================================================================================

    /* Testar o método GET do Controlador de CategoriasController:
       Testar se o valor retornado é igual a uma LISTA de objeto Produto.*/
    [Fact]   
    public async void GetProdutos_Return_OkResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        ProdutosParameters param = new ProdutosParameters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        var data = await controller.Get(param);

        Assert.IsType<ProdutoDTO>(data.Value.First());
    }

    //=====================================================================================================

    /* Testar o método GET (FORÇAR UM BADREQUEST) do Controlador de ProdutosController:
    Testar se o valor retornado é igual a um BadRequest - (400).*/
    [Fact]   
    public async void GetProdutos_Return_BadRequestResult()
    { 
        var controller = new ProdutosMockController(repository, mapper);
        ProdutosParameters param = new ProdutosParameters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        var data = await controller.GetBadProd(param);

        Assert.IsType<BadRequestResult>(data.Result);
    }

    //=====================================================================================================

    /* Testar o método GET  do Controlador de ProdutosController:
    retornar alguns dados que tenho no Banco de ados.*/
    [Fact] 
    public async void GetProdutos_Return_MatchResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        ProdutosParameters param = new ProdutosParameters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        var data = await controller.GetBd(param);

        Assert.IsType<List<ProdutoDTO>>(data.Value);
        // Avbaixo atribuo uma lista de categorias no objeto categorias e vou ter uma lista de objetos.
        var cat = data.Value.Should().BeAssignableTo<List<ProdutoDTO>>().Subject;

        Assert.Equal("Coca Cola Zero", cat[0].Nome); // Índice [0] => primeiro registro do meu Db.
        Assert.Equal("Refrigerante de Cola 350 ml", cat[0].Descricao); // Índice [0] => primeiro registro do meu Db.
        Assert.Equal("5,45", Convert.ToDecimal(cat[0].Preco).ToString()); // Índice [0] => primeiro registro do meu Db.
        Assert.Equal("cocacola.jpg", cat[0].ImagemUrl); // Índice [0] => primeiro registro do meu Db.                
        Assert.Equal("1", Convert.ToInt32(cat[0].CategoriaId).ToString()); // Índice [0] => primeiro registro do meu Db.  

        Assert.Equal("Pudim 100 g", cat[2].Nome); // Índice [2] => terceiro registro do meu Db.
        Assert.Equal("Pudim de Leite Condensado ", cat[2].Descricao); // Índice [2] => terceiro registro do meu Db.
        Assert.Equal("6,75", Convert.ToDecimal(cat[2].Preco).ToString()); // Índice [2] => terceiro registro do meu Db.
        Assert.Equal("pudim.jpg", cat[2].ImagemUrl); // Índice [2] => terceiro registro do meu Db.            
        Assert.Equal("3", Convert.ToInt32(cat[2].CategoriaId).ToString()); // Índice [2] => terceiro registro do meu Db.  
    }

    //=====================================================================================================

    /* Testar o método GET  do Controlador de ProdutosController:
    retornar Categoria por um Id - retornatr um ObjetoDTO por Id.*/
    [Fact]  
    public async void GetProdutoById_Return_OkResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        var catId = 4;

        var data = await controller.GetById(catId);

        Assert.IsType<ProdutoDTO>(data.Value);
    }

    //=====================================================================================================

    /* Testar o método GET  do Controlador de ProdutosController:
    Testar se o valor retornado é igual a um NotFound - (404).retornar */
    [Fact] 
    public async void GetProdutoById_Return_NotFoundResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        var catId = 9999;

        var data = await controller.GetById(catId);

        Assert.IsType<NotFoundResult>(data.Result);
    }

    /// ===================================================================================================
    // TESTES DE MANIPULAÇÃO - POST / PUT / DELETE   
    // ===================================================================================================

    /* Testar o método post  do Controlador de ProdutosController:
     tem que retornar um CreatedAtRouteResult - (201) e persistir no Banco de Dados*/
    [Fact]  
    public async void Post_Produto_AddValidData_Return_CreatedResult()
    {
        var controller = new ProdutosMockController(repository, mapper);

        var prod = new ProdutoDTO()
        { Nome = "Teste_inclusao", Descricao = "Testando", Preco = Convert.ToDecimal("9,99"), ImagemUrl = "testprodInclusao.jpg", CategoriaId = Convert.ToInt32("10") };

        var data = await controller.Post(prod);

        Assert.IsType<CreatedAtRouteResult>(data);
    }

    /* Este método Put tera que Alterar um produto
     além de e persistir o dado no Banco de dados*/
    [Fact] 
    public async void Put_Produto_Update_ValidData_Return_OkResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        var prodId = 1; // <- Escolher Id de acordo com o que queira alterar.

        var existingPost = await controller.GetById(prodId);
        var result = existingPost.Value.Should().BeAssignableTo<ProdutoDTO>().Subject;

        var prodDto = new ProdutoDTO();
        prodDto.ProdutoId = prodId;
        prodDto.Nome = "Produto Atualizado - Testes 3000";
        prodDto.Descricao = "Atualizado 3000";
        prodDto.Preco = Convert.ToDecimal("99,99");
        prodDto.ImagemUrl = "Atualizado 3000.png";
        prodDto.CategoriaId = Convert.ToInt32("1");

        var updateData = controller.Put(prodId, prodDto);
                                                               
        Assert.IsType<OkResult>(updateData);
    }

    /* Este método Delete tera que Excluir um Produto,
     retornar o Objeto excluido, além de epersistir o dado no Banco de dados*/
    [Fact] 
    public async void Deletet_Produto_Return_OkResult()
    {
        var controller = new ProdutosMockController(repository, mapper);
        var catId = 1; // <- Escolher Id de acordo com o que queira excluir.

        var data = await controller.Delete(catId);

        Assert.IsType<ProdutoDTO>(data.Value);
    }
}
