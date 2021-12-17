using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using api.Controllers;
using api.Dtos;
using api.Extension;
using AutoMapper;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.V1.Controllers
{
    [Authorize]
	[ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
	public class ProdutosController : MainController
	{
		private readonly IProdutoRepository _produtoRepository;
		private readonly IProdutoService _produtoService;
		private readonly IMapper _mapper;
		public ProdutosController(INotificador notificador,
		                          IProdutoRepository produtoRepository,
								 IProdutoService produtoService,
								 IMapper mapper,
								 IUser user ) : base(notificador, user)
		{
			_produtoRepository = produtoRepository;
			_produtoService = produtoService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<ProdutoDto>> ObterTodos()
		{

			return _mapper.Map<IEnumerable<ProdutoDto>>(await _produtoRepository.ObterProdutosFornecedores());
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ProdutoDto>> ObterPorId(Guid id) {

			var produtoDto = _mapper.Map<ProdutoDto>(await _produtoRepository.ObterProdutoFornecedor(id));

			if(produtoDto == null) return NotFound();

			return produtoDto; 
		}
        
		[ClaimsAuthorize("Produtos","Remover")]
		[HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoDto>> Excluir(Guid id) {

			var produtoDto = _mapper.Map<ProdutoDto>(await _produtoRepository.ObterProdutoFornecedor(id));

			if(produtoDto == null) return NotFound();

			await _produtoService.Remover(id);

			return CustomResponse(produtoDto);
		}
       
	    [ClaimsAuthorize("Produtos","Adicionar")]
		[HttpPost]
		public async Task<ActionResult<ProdutoDto>> Adicionar(ProdutoDto produtoDto){
           
		    if (!ModelState.IsValid) return CustomResponse(ModelState);

			var imgNome = Guid.NewGuid() + "_" + produtoDto.Imagem;

            if (!UploadArquivo(produtoDto.ImagemUpload, imgNome)){
                
				return CustomResponse();
			}

			produtoDto.Imagem = imgNome;
			await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDto));

			return CustomResponse(produtoDto);


		}
        
		[ClaimsAuthorize("Produtos","Adicionar")]
		[HttpPost("Adicionar")]
		public async Task<ActionResult<ProdutoDto>> AdicionarAlternativo(ProdutoImagemDto produtoDto){
           
		    if (!ModelState.IsValid) return CustomResponse(ModelState);

			var imgPrefixo = Guid.NewGuid() + "_";

            if (!await UploadArquivoAlternativo(produtoDto.ImagemUpload, imgPrefixo)){
                
				return CustomResponse();
			}

			produtoDto.Imagem = imgPrefixo + produtoDto.ImagemUpload.FileName;
			await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDto));

			return CustomResponse(produtoDto);


		}

         //ate mesmo o tipo IFormFile tem limite default para o arquivo
		 //[RequestSizeLimit(400000000)] setando o tamanho aceito do aquivo
		// [DisableRequestSizeLimit]
        // [HttpPost("imagem")]
		// public async Task<ActionResult> AdicionarImagem(IFormFile file){
           
		//        return Ok(file);
		// }

        [ClaimsAuthorize("Produtos","Atualizar")]
		[HttpPut("{id:guid}")]
		public async Task<ActionResult> Atualizar(Guid id, ProdutoDto produtoDto){

          if(id != produtoDto.Id) return NotFound();

		  var produtoAtualizacao =  _mapper.Map<ProdutoImagemDto>(await _produtoRepository.ObterProdutoFornecedor(id));
		  produtoDto.Imagem = produtoAtualizacao.Imagem;
		  if(!ModelState.IsValid) return CustomResponse(ModelState);
		  
		  if(produtoDto.ImagemUpload != null){

			  var imagemNome = Guid.NewGuid() + "_" + produtoDto.Imagem;
			  if(!UploadArquivo(produtoDto.ImagemUpload, imagemNome)){

				  return CustomResponse(ModelState);
			  }

			 produtoAtualizacao.Imagem = imagemNome;
		  }

		  produtoAtualizacao.Nome = produtoDto.Nome;
		  produtoAtualizacao.Descricao = produtoDto.Descricao;
		  produtoAtualizacao.Valor = produtoDto.Valor;
		  produtoAtualizacao.Ativo = produtoDto.Ativo;

		  await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

		  return CustomResponse(produtoAtualizacao);

		}

		private async  Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo){
		
			if(arquivo == null || arquivo.Length == 0){
                 
				 NotificarErro("Forneça uma imagem para esse produto!");
				 return false;
			}

			var  filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgPrefixo + arquivo.FileName);

			if (System.IO.File.Exists(filePath))
			{
				NotificarErro("Já existe um arquivo com esse nome!");
				return false;
			}

			using (var stream = new FileStream(filePath, FileMode.Create)){
               await arquivo.CopyToAsync(stream);
			}

			return true;
		} 

		private bool UploadArquivo(string arquivo, string imgNome){
		
			if(string.IsNullOrEmpty(arquivo)){
                 
				 NotificarErro("Forneça uma imagem para esse produto!");
				 return false;
			}

			var imageDataByteArray = Convert.FromBase64String(arquivo);


			var  filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgNome);

			if (System.IO.File.Exists(filePath))
			{
				NotificarErro("Já existe um arquivo com esse nome!");
				return false;
			}

			System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

			return true;
		} 

	}

}