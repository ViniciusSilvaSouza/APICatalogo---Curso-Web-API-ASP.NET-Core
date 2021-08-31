using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public CategoriasController(IUnitOfWork context, IConfiguration config, ILogger<CategoriasController> logger)
        {
            _uof = context;
            _configuration = config;
            _logger = logger;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            var conexao = _configuration["ConnectionStrings:DefaultConnection"];
            return $"Autor: {autor} Conexao: {conexao}";
        }
        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {

            try
            {
                return _uof.CategoriaRepository.GetCategoriaProdutos().ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuservico,
            string nome)
        {
            return meuservico.Saudacao(nome);
        }


        [HttpGet("{id}", Name = "GetCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(c =>c.CategoriaId ==id);
                if (categoria == null)
                {
                    return NotFound($"A categoria com id={id} não foi encontrada");
                }
                else
                {
                    return categoria;
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("categoriaProduto")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProduto()
        {
            return _uof.CategoriaRepository.GetCategoriaProdutos().ToList();
        }
        [HttpPost]
        public ActionResult<Categoria> Post([FromBody] Categoria categoria)
        {
            try
            {
                _uof.CategoriaRepository.Add(categoria);
                _uof.Commit();
                return new CreatedAtRouteResult("GetCategoria", new { id = categoria.CategoriaId }, categoria);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar uma nova categoria");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.CategoriaId)
                    return BadRequest($"Não foi possível atualizar a categoria com id={id}");
                _uof.CategoriaRepository.Update(categoria);
                _uof.Commit();
                return Ok($"Categoria com id={id} alterada com sucesso");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar alterar categoria com id={id}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

                if (categoria == null)
                    return NotFound($"A categoria com id={id} não foi encontrada");

                _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();
                return categoria;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar alterar categoria com id={id}");
            }
        }
    }
}


