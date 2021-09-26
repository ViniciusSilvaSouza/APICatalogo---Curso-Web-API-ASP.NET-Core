using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork context, IConfiguration config, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uof = context;
            _configuration = config;
            _mapper = mapper;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            var conexao = _configuration["ConnectionStrings:DefaultConnection"];
            return $"Autor: {autor} Conexao: {conexao}";
        }
        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                var categorias  = _uof.CategoriaRepository.GetCategorias(categoriasParameters);

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
                
                var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);
                return categoriasDTO;
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
        public ActionResult<CategoriaDTO> Get(int id)
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
                    var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                    return categoriaDTO;
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDTO);
                _uof.CategoriaRepository.Add(categoria);
                _uof.Commit();

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
                return new CreatedAtRouteResult("GetCategoria", new { id = categoria.CategoriaId }, categoriaDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar uma nova categoria");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                if (id != categoriaDTO.CategoriaId)
                    return BadRequest($"Não foi possível atualizar a categoria com id={id}");
                var categoria = _mapper.Map<Categoria>(categoriaDTO);
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
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

                if (categoria == null)
                    return NotFound($"A categoria com id={id} não foi encontrada");

                _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return categoriaDTO;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar alterar categoria com id={id}");
            }
        }
    }
}


