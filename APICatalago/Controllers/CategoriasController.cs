using APICatalago.Context;
using APICatalago.Models;
using APICatalago.Pagination;
using APICatalago.Repository;
using APICatalago.Repository.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Controllers
{
    [Produces("application/json")] // define como padrão de retorno no swagger o formato json para este controlador
    //[Authorize(AuthenticationSchemes = "Bearer")] // restringe o acessa apenas a usuarios autenticados a nossa api
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        //private readonly AppDbContext _context; //injeção de dependencia
        private readonly IUnitOfWork _context; //Unit of work, substitui o DBContext, esta em serviço
       // private readonly IConfiguration _configuration; // injeção de dependdecia do appsettings.json
        private readonly ILogger _logger; // criando log
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork contexto, /*IConfiguration config,*/ /*ILogger<CategoriasController> logger, por causa dos teste */ IMapper mapper) // injeção de dependencia
        {
            _context = contexto; // contexto do banco de dados
           // _configuration = config;
            //_logger = logger;
            _mapper = mapper;
        }

        //[HttpGet("autor")]
        //public string GetAutor() // retornar meu nome do appsettings.json
        //{
        //    var autor = _configuration["autor"];
        //    var conexao = _configuration["ConnectionStrings:DefaultConnection"];
        //    var log = _configuration["Logging:LogLevel:Microsoft"];


        //    return $" Autor: {autor} \n Conexao: {conexao}  \n Log: {log}";
        //}

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome) // roteamento a partir de um serviço
        {
            return meuServico.Saudacao(nome);
        }


        //COM PAGINAÇÃO ASSINCRONA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters) // Retorna uma lista de categorias
        {
            _logger.LogInformation("====================  GET api/Categorias ========================");

            try
            {
                // return _context.Catergorias.AsNoTracking().ToList(); // AsNoTracking melhora o desempenho, somente pode ser usado em consultas ao banco
                var categoria = await _context.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new
                {
                    categoria.TotalCount,
                    categoria.PageSize,
                    categoria.CurrentPage,
                    categoria.TotalPage,
                    categoria.HasNext,
                    categoria.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);
                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                "Erro ao tentar obetr as cateorias do banco de dados");
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get() // Retorna uma lista de categorias
        //{
        //    _logger.LogInformation("====================  GET api/Categorias ========================");

        //    try
        //    {
        //        // return _context.Catergorias.AsNoTracking().ToList(); // AsNoTracking melhora o desempenho, somente pode ser usado em consultas ao banco
        //        var categoria = await _context.CategoriaRepository.Get().ToListAsync();

        //        var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);
        //        return categoriaDTO;
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //                        "Erro ao tentar obetr as cateorias do banco de dados");
        //    }
        //}

        [HttpGet("produtos")]//define um novo end point para este metodo api/Categoria/produtos
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos() // Retorna uma lista de categorias
        {
            _logger.LogInformation("====================  GET api/Categorias/produtos  ========================");

            var categoria = await _context.CategoriaRepository.GetCategoriasProdutos();
            var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);
            return categoriaDTO;// Include retorna as categorias e seus produtos, por causa do relacionamento que existe entre as entidades
        }

        /// <summary>
        /// Obtem uma Categoria pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(ProdutoDTO),StatusCodes.Status200OK)]// MAPEAR TIPOS DE RETORNO PARA O SWAGGER NO CASO O METODO SO RETORNA O 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]// MAPEAR TIPOS DE RETORNO PARA O SWAGGER NO CASO O METODO SO RETORNA O 200 OK
        public async Task<ActionResult<CategoriaDTO>> Get(int id) // retorna uma categoria
        {
            try
            {
                var categoria = await _context.CategoriaRepository.GetById(c => c.CategoriaId == id); //Usando linq para encontrar o primeiro produto da tabela de categorias

                _logger.LogInformation($"====================  GET api/Categorias/id = {id}  ========================");

                if (categoria == null)
                {
                    _logger.LogInformation($"==================== GET api/Categorias/id = {id} NOT FOUND ========================");

                    return NotFound($"A catergoria com id={id} não foi encontrada"); // 404 NOT FOUND
                }

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao tentar obetr as cateorias do banco de dados");
            }
        }

        /// <summary>
        /// Incluiu uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     Post api/categoria
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>O objeto Categoria Incluida</returns>
        /// <remarks>Retorna o objeto </remarks>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        //[ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status201Created)]// MAPEAR TIPOS DE RETORNO PARA O SWAGGER NO CASO O METODO SO RETORNA O 201 OK
        //[ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status400BadRequest)]// MAPEAR TIPOS DE RETORNO PARA O SWAGGER NO CASO O METODO SO RETORNA O 400 OK
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto) //FromBody significa que no corpo da requisição vai conter as informações da categoria que sera postado
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState); //Modelstate faz verificação e validação das informações enviadas no body 
            //}

            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _context.CategoriaRepository.Add(categoria);
                await _context.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", // rota onde encontra=ra o categoria
                        new { id = categoria.CategoriaId }, categoriaDTO); // parametros da action que eh o ID    e no corpo da action o categoria
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Erro ao tentar criar nova cateoria ");
            }

        }

        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Put))] // APLICA AUTOMATICAMENTO 
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                if (id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"Não foi possivel atualziar a categoria com id={id}"); // verifica se o ID existe
                }

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _context.CategoriaRepository.Update(categoria); // modifica o estado da categoria
                await _context.Commit(); // salva

                return Ok($"Categoria com id={id} foi atualizada com sucesso"); // HTTP 200 STATUS
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       $"Erro ao tentar atualizar a categoria com id={id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            try
            {
                var categoria= await _context.CategoriaRepository.GetById(c => c.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound($"A categoria com id=`{id} não foi encontrada"); // verifica se o ID existe
                }

                _context.CategoriaRepository.Delete(categoria); // deleta o categoria
               await _context.Commit(); // salva

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return categoriaDTO; // retorna o produto removido
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        $"Erro ao excluir a categoria de id={id}");
            }

        }
    }
}
