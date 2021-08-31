using APICatalogo.Filter;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        public ProdutosController(IUnitOfWork context)
        {
            _uof = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(APILoggingFilter))]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            return _uof.ProdutoRepository.Get().ToList();
        }

        [HttpGet("{id:int:min(1)}", Name = "GetProduto")]
        public ActionResult<Produto> GetAsync(int id)
        {
            var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }
            else
            {
                return produto;
            }
        }

        [HttpPost]
        public ActionResult<Produto> Post([FromBody] Produto produto)
        {
            _uof.ProdutoRepository.Add(produto);
            _uof.Commit();
            return new CreatedAtRouteResult("GetProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            if (id != produto.ProdutoId)
                return BadRequest();

            _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
                return NotFound();

            _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();
            return produto;
        }

        [HttpGet("menorpreco")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPrecos()
        {
            return _uof.ProdutoRepository.GetProdutosPorPreco().ToList();
        }

    }
}
