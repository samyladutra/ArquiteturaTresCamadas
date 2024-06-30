using System.Linq.Expressions;
using Moq;
using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Models;
using ProdutosApi.Business.Notificacoes;
using ProdutosApi.Business.Services;

namespace ProdutosApi.Business.Tests
{
    public class ProdutoServiceTests
    {
        [Fact]
        public async void Adicionar_Produto_Com_Descricao_Invalida_Deve_Retornar_2_Notificacoes()
        {
            //arrange
            var produto = new ProdutoBuilder()
                                .ComNome("Algum nome favorável")
                                .ComDescricao("")
                                .ComValor((decimal)25.74)
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var produtoService = new ProdutoService(new Mock<IProdutoRepository>().Object, notificador.Object);

            //act
            await produtoService.Adicionar(produto);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }

        [Fact]
        public async void Adicionar_Produto_Com_Nome_Invalido_Deve_Retornar_2_Notificacoes()
        {
            //arrange
            var produto = new ProdutoBuilder()
                                 .ComNome("")
                                 .ComDescricao("Alguma descrição favorável")
                                 .ComValor((decimal)20.74)
                                 .Ativo(true)
                                 .Build();


            var notificador = new Mock<INotificador>();
            var produtoService = new ProdutoService(new Mock<IProdutoRepository>().Object, notificador.Object);

            //act
            await produtoService.Adicionar(produto);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }

        [Fact]
        public async void Adicionar_Produto_Com_Valor_Invalido_Deve_Retornar_1_Notificacoes()
        {
            //arrange
            var produto = new ProdutoBuilder()
                                 .ComNome("Algum nome favorável")
                                 .ComDescricao("Alguma descrição favorável")
                                 .ComValor(0)
                                 .Ativo(true)
                                 .Build();


            var notificador = new Mock<INotificador>();
            var produtoService = new ProdutoService(new Mock<IProdutoRepository>().Object, notificador.Object);

            //act
            await produtoService.Adicionar(produto);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Once);
        }

        [Fact]
        public async void Adicionar_Produto_Com_Dados_Validos_Deve_Incluir_Registro_Na_Base_De_Dados()
        {
            //arrange
            var produto = new ProdutoBuilder()
                                .ComId(Guid.NewGuid())
                                 .ComNome("Algum nome favorável")
                                 .ComDescricao("Alguma descrição favorável")
                                 .ComValor((decimal)25.5)
                                 .Ativo(true)
                                 .Build();


            var produtoRepository = new Mock<IProdutoRepository>();
            var produtoService = new ProdutoService(produtoRepository.Object, new Mock<INotificador>().Object);

            //act
            await produtoService.Adicionar(produto);

            //assert
            produtoRepository.Verify(n => n.Adicionar(produto), Times.Once);
        }

        [Fact]
        public async void Adicionar_Produto_Com_Id_Ja_Existente_Nao_Deve_Incluir_Produto_Na_Base_De_Dados()
        {
            //arrange
            var produto = new ProdutoBuilder()
                                .ComId(Guid.NewGuid())
                                 .ComNome("Algum nome favorável")
                                 .ComDescricao("Alguma descrição favorável")
                                 .ComValor((decimal)90.8)
                                 .Ativo(true)
                                 .Build();

            var notificador = new Mock<INotificador>();
            var produtoRepository = new Mock<IProdutoRepository>();
            var produtoService = new ProdutoService(produtoRepository.Object, notificador.Object);

            produtoRepository.Setup(f => f.ObterPorId(It.IsAny<Guid>()))
                            .ReturnsAsync(produto);

            //act
            await produtoService.Adicionar(produto);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Once);
            produtoRepository.Verify(r => r.Adicionar(It.IsAny<Produto>()), Times.Never);
        }

        [Fact]
        public async void Remover_Produto_Deve_Efetivar_Operacao_Na_Base_De_Dados()
        {
            //arrange
            var notificador = new Mock<INotificador>();
            var produtoRepository = new Mock<IProdutoRepository>();
            var produtoService = new ProdutoService(produtoRepository.Object, notificador.Object);

            //act
            await produtoService.Remover(new Guid());

            //assert
            produtoRepository.Verify(r => r.Remover(It.IsAny<Guid>()), Times.Once);
        }
    }
}