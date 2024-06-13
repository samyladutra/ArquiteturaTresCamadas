using FluentAssertions;
using Moq;
using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Models;
using ProdutosApi.Business.Notificacoes;
using ProdutosApi.Business.Services;

namespace ProdutosApi.Business.Tests
{
    public class FornecedorServiceTests
    {
        [Fact]
        public async void Atualizar_Fornecedor_Deve_Retornar_Sucesso()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("")
                                .ComDocumento("97310212053")
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            var notificacao = new Notificacao("O campo Nome precisa ser fornecido");
            notificador.Setup(n => n.ObterNotificacoes()).Returns(new List<Notificacao>() { notificacao });

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            var teste = notificador.Object.ObterNotificacoes();
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }
    }
}