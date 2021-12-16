using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Extension;
using AutoMapper;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
	[Authorize]
	[Route("api/fornecedores")]
	public class FornecedoresController : MainController
	{
		private readonly IFornecedorRepository _fornecedorRepository;
		private readonly IFornecedorService _fornecedorService;
		private readonly IEnderecoRepository _enderecoRepository;
		private readonly IMapper _mapper;

		public FornecedoresController(IFornecedorRepository fornecedorRepository,
									  IMapper mapper,
									  IFornecedorService fornecedorService,
									  INotificador notificador,
									  IEnderecoRepository enderecoRepository) : base(notificador)
		{
			_fornecedorRepository = fornecedorRepository;
			_mapper = mapper;
			_fornecedorService = fornecedorService;
			_enderecoRepository = enderecoRepository;
		}


		[HttpGet]
		public async Task<IEnumerable<FornecedorDto>> ObterTodos()
		{
			var fornecedor = _mapper.Map<IEnumerable<FornecedorDto>>(await _fornecedorRepository.ObterTodos());
			return fornecedor;
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<FornecedorDto>> ObterPorId(Guid id)
		{
			var fornecedor = _mapper.Map<FornecedorDto>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

			if (fornecedor == null) return NotFound();

			return fornecedor;
		}

		[HttpGet("obter-endereco/{id:guid}")]
		public async Task<ActionResult<EnderecoDto>> ObterEnderecoPorId(Guid id)
		{

			return _mapper.Map<EnderecoDto>(await _enderecoRepository.ObterPorId(id));
		}

        [ClaimsAuthorize("Fornecedores","Atualizar")]
		[HttpPut("atualizar-endereco/{id:guid}")]
		public async Task<ActionResult<EnderecoDto>> AtualizarEndereco(Guid id, EnderecoDto enderecoDto)
		{

			if (id != enderecoDto.Id)
			{

				NotificarErro("Id do endereco não é o mesmo passado na query");
				return CustomResponse(enderecoDto);
			}

			if (!ModelState.IsValid) return CustomResponse(ModelState);

			await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoDto));

			return CustomResponse(enderecoDto);

		}
        
		[ClaimsAuthorize("Fornecedores","Adicionar")]
		[HttpPost]
		public async Task<ActionResult<FornecedorDto>> Adcionar(FornecedorDto fornecedorDto)
		{

			if (!ModelState.IsValid) return CustomResponse(ModelState);

			await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorDto));

			return CustomResponse(fornecedorDto);
		}

        [ClaimsAuthorize("Fornecedores","Atualizar")]
		[HttpPut("{id:guid}")]
		public async Task<ActionResult<FornecedorDto>> Atualizar(Guid id, FornecedorDto fornecedorDto)
		{

			if (id != fornecedorDto.Id) return BadRequest();

			if (!ModelState.IsValid) return CustomResponse(ModelState);

			var result = await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorDto));

			return CustomResponse(fornecedorDto);
		}

        [ClaimsAuthorize("Fornecedores","Remover")]
		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<FornecedorDto>> Excluir(Guid id)
		{

			var fornecedorDto = _mapper.Map<FornecedorDto>(await _fornecedorRepository.ObterFornecedorEndereco(id));

			if (fornecedorDto == null) return NotFound();

			await _fornecedorService.Remover(id);

			return CustomResponse(fornecedorDto);
		}



	}
}
