using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(AppDbContext context) : base(context) { }

        public PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            return PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                produtosParameters.PageNumber, produtosParameters.PageSize);

            //return Get()
            //    .OrderBy(on => on.Nome)
            //    .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //    .Take(produtosParameters.PageSize)
            //    .ToList();
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
            return Get().OrderBy(p => p.Preco).ToList();
        }
    }
}
