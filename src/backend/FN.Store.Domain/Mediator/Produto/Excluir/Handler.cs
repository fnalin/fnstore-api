﻿using FN.Store.Domain.Contracts.Infra.Data;
using FN.Store.Domain.Contracts.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FN.Store.Domain.Mediator.Produto.Excluir
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IMediator _mediator;
        private readonly IProdutoReadRepository _produtoReadRepository;
        private readonly IProdutoWriteRepository _produtoWriteRepository;
        private readonly IUnitOfWork _uow;

        public Handler(
            IMediator mediator,
            IProdutoWriteRepository produtoWriteRepository,
            IProdutoReadRepository produtoReadRepository,
            IUnitOfWork uow)
        {
            _mediator = mediator;
            _produtoReadRepository = produtoReadRepository;
            _produtoWriteRepository = produtoWriteRepository;
            _uow = uow;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var produto = await _produtoReadRepository.GetAsync(request.Id);

            if (produto == null)
            {
                var response = new Response();
                response.AddError($"Produto de id {request.Id} não encontrado");
                return response;
            }

            _produtoWriteRepository.Delete(produto);
            await _uow.CommitAsync();

            await _mediator.Publish(new Notification
            {
                Id = produto.Id,
                Nome = produto.Nome
            });

            return new Response(produto);
        }

    }
}
