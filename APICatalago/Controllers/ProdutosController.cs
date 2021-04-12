using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Pagination;
using APICatalago.Repository;
using APICatalago.Repository.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APICatalago.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))] // Define o tipo de retorno 200,201,400 para metodo action do controller
    [Produces("application/json")] // define como padrão de retorno no swagger o formato json para este controlador
    // [Authorize(AuthenticationSchemes = "Bearer")] // necessario para utilizar o toker JWT para usar o controller
    [Route("api/[Controller]")] // faz mapeamento dos metodos action e roteia conforme o metodo action
    [ApiController] // permite controlar automatico toda api,       Modelstate faz verificação e validação das informações enviadas no body 
    [EnableCors("PermitirApiRequest")] // todos metodos action GET desse controller, vai atender essa requisiçao externa
    public class ProdutosController : ControllerBase // faz a classe ser controller
    {
        private readonly IUnitOfWork _uof; //injeção de dependencia
        private readonly IMapper _mapper;
        public ProdutosController(IUnitOfWork contexto, IMapper mapper)
        {
            _uof = contexto; // contexto do banco de dados
            _mapper = mapper;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos() // retorna uma lista de produtos ordenado por preço
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco(); // vem todas as info de produtos
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos); // e aqui são mapeadas para o DTO
            return produtosDTO;
        }

        [HttpGet("primeiro")]
        [HttpGet("/primeiro")]
       // [HttpGet("{valor:alpha:lenght(5)}")] // Restrição de A - Z alphanumeric
        public ActionResult<ProdutoDTO> Get2()
        {
            var produtos = _uof.ProdutoRepository.Get().FirstOrDefault();   // vem todas as info de produtos
            var produtosDTO = _mapper.Map<ProdutoDTO>(produtos);         // e aqui são mapeadas para o DTO
            return produtosDTO;

            //return _uof.ProdutoRepository.Get().FirstOrDefault();
        }


        //PAGINAÇÃO
        // SEMPRE QUE ACESSAR BANCO DE DADOS, UM CALCULO COMPLEXO OU UMA OPERAÇÃO I.O, JUSTIFICA O METODO ASYNC
        //api/produtos?pageNumber=1&pageSize=2
        //api/produtos
        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtosParameters"></param>
        /// <returns>Retonar uma lista de objetos Produto</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        //[EnableCors("PermitirApiRequest")] // para apenas um metodo
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters) // Retorna uma lista de Produto
        {
            try
            {
                // paginação
                var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters); // vem todas as info de produtos

                var metadata = new
                {
                    produtos.TotalCount,
                    produtos.PageSize,
                    produtos.CurrentPage,
                    produtos.TotalPage,
                    produtos.HasNext,
                    produtos.HasPrevious,
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata)); // adiciona no response do headers a paginação

                var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos); // e aqui são mapeadas para o DTO
                return produtosDTO;

                // return _uof.ProdutoRepository.Get().AsNoTracking().ToList(); // AsNoTracking melhora o desempenho, somente pode ser usado em consultas ao banco
                //AWAIT os dados usados nessa operação podem ser usados para processar outra operação, enquanto esta não terminar
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obetr as produtos do banco de dados");
            }
        }


        //api/produtos/1
        // [HttpGet("{id:int:min(1)}/{Nome?}", Name = "ObterProduto")] // ? deixa o parametro opcional

        // [HttpGet("{id}", Name = "ObterProduto")]
        // api/produtos/1?id=2 considera o valor 2 para a consulta [FromQuery]
        //public async Task<ActionResult<Produto>> GetAsync([FromQuery] int id) // retorna um produto,  BindRequired obriga que seja passado valor no parametro
        //{
        //    try
        //    {
        //        var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id); //Usando linq para encontrar o primeiro produto da tabela de produtos

        //        if (produto == null)
        //        {
        //            return NotFound($"A catergoria com id={id} não foi encontrada"); // 404 NOT FOUND
        //        }
        //        return produto;
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Erro ao tentar obetr as produtos do banco de dados");
        //    }
        //}

        //[HttpGet]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        //public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get() // Retorna uma lista de Produto
        //{
        //    try
        //    {
        //        // paginação
        //        var produtos = await _uof.ProdutoRepository.Get().ToListAsync(); // vem todas as info de produtos

        //        var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos); // e aqui são mapeadas para o DTO
        //        return produtosDTO;

        //        // return _uof.ProdutoRepository.Get().AsNoTracking().ToList(); // AsNoTracking melhora o desempenho, somente pode ser usado em consultas ao banco
        //        //AWAIT os dados usados nessa operação podem ser usados para processar outra operação, enquanto esta não terminar
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Erro ao tentar obetr as produtos do banco de dados");
        //    }
        //}


        /// <summary>
        /// Obtem um produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto produto</returns>
        [HttpGet("{id}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id) // retorna um produto,  BindRequired obriga que seja passado valor no parametro
        {
            try
            {
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id); //Usando linq para encontrar o primeiro produto da tabela de produtos

                if (produto == null)
                {
                    return NotFound($"O produto com id={id} não foi encontrada"); // 404 NOT FOUND
                }

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                return produtoDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obetr as produtos do banco de dados");
            }
        }


        //api/produtos
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto) //FromBody significa que no corpo da requisição vai conter as informações do produtos que sera postado
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState); //Modelstate faz verificação e validação das informações enviadas no body 
            //}

            //try
            //{

                var produto = _mapper.Map<Produto>(produtoDto);

                _uof.ProdutoRepository.Add(produto);
                await _uof.Commit();

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                return new CreatedAtRouteResult("ObterProduto", // rota onde encontra=ra o produto
                        new { id = produto.ProdutoId }, produtoDTO); // parametros da action que eh o ID    e no corpo da action o produto
            //}
            //catch (Exception)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError,
            //                           "Erro ao tentar criar novo produto");
            //}
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto) // VERIFICA SE O ID DA REQUISIÇÃO EH IGUAL DO PRODUTO QUE VEM NO CORPO DA REQUISIÇÃO
        {
            //try
            //{
                if (id != produtoDto.ProdutoId)
                {
                    return BadRequest($"Não foi possivel atualziar o produto com id={id}"); // verifica se o ID existe
                }

                var produto = _mapper.Map<Produto>(produtoDto);

                _uof.ProdutoRepository.Update(produto); // modifica o estado do produto
                await _uof.Commit(); // salva
                return Ok(); // HTTP 200 STATUS
            //}
            //catch (Exception)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError,
            //          $"Erro ao tentar atualizar o produto com id={id}");
            //}
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            //try
            //{
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto == null)
                {
                    return NotFound($"A categoria com id=`{id} não foi encontrada"); // verifica se o ID existe
                }

                _uof.ProdutoRepository.Delete(produto); // deleta o produto
                await _uof.Commit(); // salva

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                return produtoDTO; // retorna o produto removido
            //}
            //catch (Exception)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError,
            //            $"Erro ao excluir o produto de id={id}");
            //}
        }
    }
}
