using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Models;
using ProdutosApi.Business.Models.Validations.Documentos;
using ProdutosApi.Business.Notificacoes;
using ProdutosApi.Business.Services;
using ProdutosApi.Data.Repository;

namespace ProdutosApi.Business.Tests
{
    public class FornecedorServiceTests
    {
        [Fact]
        public async void Adicionar_Fornecedor_Com_Nome_Invalido_Deve_Retornar_2_Notificacoes()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("")
                                .ComDocumento("97310212053")
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }


        [Fact]
        public async void Adicionar_Fornecedor_Com_Endereco_Vazio_Deve_Retornar_3_Notificacoes()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("NomeTeste")
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco())
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(3));

        }

        [Theory]
        [InlineData(TipoFornecedor.PessoaFisica, "123")]
        [InlineData(TipoFornecedor.PessoaJuridica, "1234")]
        public async void Adicionar_Fornecedor_Com_CPF_Ou_CNPJ_Invalido_Deve_Retornar_2_Notificacoes(TipoFornecedor tipoFornecedor, string documento)
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("NomeTeste")
                                .DoTipo(tipoFornecedor)
                                .ComDocumento(documento)
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }


        [Fact]
        public async void Adicionar_Fornecedor_Com_Dados_Validos_Deve_Efetivar_Inclusao_Na_Base_De_Dados()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .Ativo(true)
                                .Build();

            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, new Mock<INotificador>().Object);

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            fornecedorRepository.Verify(f => f.Adicionar(fornecedor), Times.Once);
        }

        [Fact]
        public async void Adicionar_Fornecedor_Com_Id_Ja_Existente_Nao_Deve_Incluir_Fornecedor_Na_Base_De_Dados()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .Ativo(true)
                                .Build();

            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, new Mock<INotificador>().Object);

            fornecedorRepository.Setup(f => f.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                            .Returns(Task.FromResult<IEnumerable<Fornecedor>>(new List<Fornecedor>() { fornecedor }));

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            fornecedorRepository.Verify(r => r.Adicionar(It.IsAny<Fornecedor>()), Times.Never);
        }

        [Fact]
        public async void Remover_Fornecedor_Inexistente_Deve_Retornar_Alerta_Fornecedor_Nao_Existe()
        {
            //arrange
            var notificador = new Mock<INotificador>();
            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, notificador.Object);

            fornecedorRepository.Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                            .ReturnsAsync((Fornecedor)null);

            //act
            await fornecedorService.Remover(new Guid());

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(1));
            fornecedorRepository.Verify(r => r.Remover(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async void Remover_Fornecedor_Com_Produto_Deve_Retornar_Alerta_Fornecedor_Possui_Produtos()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .ComProdutos(new List<Produto>(){
                                    new Produto(){ Nome="NomeProduto"}
                                })
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, notificador.Object);

            fornecedorRepository.Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                            .ReturnsAsync(fornecedor);

            //act
            await fornecedorService.Remover(new Guid());

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(1));
            fornecedorRepository.Verify(r => r.Remover(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async void Remover_Fornecedor_Com_Endereco_Deve_Retornar_Alerta_Fornecedor_Possui_Endereco()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .ComProdutos(new List<Produto>() { })
                                .Ativo(true)
                                .Build();

            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, new Mock<INotificador>().Object);

            fornecedorRepository.Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                            .ReturnsAsync(fornecedor);

            fornecedorRepository.Setup(r => r.ObterEnderecoPorFornecedor(fornecedor.Id))
                                     .ReturnsAsync(fornecedor.Endereco);

            //act
            await fornecedorService.Remover(fornecedor.Id);

            //assert
            fornecedorRepository.Verify(r => r.RemoverEnderecoFornecedor(fornecedor.Endereco), Times.Once);
        }


        [Fact]
        public async void Atualizar_Fornecedor_Com_Nome_Invalido_Deve_Retornar_2_Notificacoes()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("")
                                .ComDocumento("97310212053")
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            //act
            await fornecedorService.Atualizar(fornecedor);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData(TipoFornecedor.PessoaFisica, "123")]
        [InlineData(TipoFornecedor.PessoaJuridica, "1234")]
        public async void Atualizar_Fornecedor_Com_CPF_Ou_CNPJ_Invalido_Deve_Retornar_2_Notificacoes(TipoFornecedor tipoFornecedor, string documento)
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("NomeTeste")
                                .DoTipo(tipoFornecedor)
                                .ComDocumento(documento)
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            //act
            await fornecedorService.Atualizar(fornecedor);

            //assert
            notificador.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Exactly(2));
        }


        [Fact]
        public async void Atualizar_Fornecedor_Com_Dados_Validos_Deve_Efetivar_Alteracao_Na_Base_De_Dados()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .Ativo(true)
                                .Build();

            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, new Mock<INotificador>().Object);

            //act
            await fornecedorService.Atualizar(fornecedor);

            //assert
            fornecedorRepository.Verify(f => f.Atualizar(fornecedor), Times.Once);
        }

        [Fact]
        public async void Atualizar_Fornecedor_Com_Id_Ja_Existente_Nao_Deve_Incluir_Fornecedor_Na_Base_De_Dados()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComId(Guid.NewGuid())
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("97310212053")
                                .ComEndereco(new Endereco()
                                {
                                    Logradouro = "rua",
                                    Cep = "30421031",
                                    Numero = "2"
                                })
                                .Ativo(true)
                                .Build();

            var fornecedorRepository = new Mock<IFornecedorRepository>();
            var fornecedorService = new FornecedorService(fornecedorRepository.Object, new Mock<INotificador>().Object);

            fornecedorRepository.Setup(f => f.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                            .Returns(Task.FromResult<IEnumerable<Fornecedor>>(new List<Fornecedor>() { fornecedor }));

            //act
            await fornecedorService.Atualizar(fornecedor);

            //assert
            fornecedorRepository.Verify(r => r.Atualizar(It.IsAny<Fornecedor>()), Times.Never);
        }
    }
}
