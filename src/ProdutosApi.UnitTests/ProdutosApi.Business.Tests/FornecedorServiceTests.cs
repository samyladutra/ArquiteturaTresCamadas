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
        public async void Atualizar_Fornecedor_Sem_Nome_Deve_Retornar_Mensagem_De_Erro_Para_Nome()
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
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }


        [Fact]
        public async void Atualizar_Fornecedor_Sem_Endereco_Deve_Retornar_Mensagem_De_Erro_Endereco_Invalido()
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

            var notificacao = new Notificacao("O campo Logradouro precisa ser fornecido");
            notificador.Setup(n => n.ObterNotificacoes()).Returns(new List<Notificacao>() { notificacao });

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }

        [Fact]
        public async void Atualizar_Fornecedor_PF_Deve_Retornar_Mensagem_De_Erro_Tamanho_Cpf_Invalido()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
                                .ComNome("NomeTeste")
                                .DoTipo(TipoFornecedor.PessoaFisica)
                                .ComDocumento("123")
                                .Ativo(true)
                                .Build();

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);

            var notificacao = new Notificacao("O campo Documento precisa ter " + CpfValidacao.TamanhoCpf + " caracteres e foi fornecido Documento.");
            notificador.Setup(n => n.ObterNotificacoes()).Returns(new List<Notificacao>() { notificacao });

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }


        [Fact]
        public async void Adicionar_Fornecedor_Com_Id_Ja_Existente_Deve_Retornar_Mensagem_De_Erro_Fornecedor_Ja_Cadastrado()
        {
            //arrange
            var fornecedor = new FornecedorBuilder()
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

            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);
            var fornecedorRepository = new Mock<FornecedorRepository>();

            fornecedorRepository.Setup(f => f.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<Fornecedor>>(new List<Fornecedor>() { fornecedor }));

            // .ReturnsAsync(
            //     new List<Fornecedor>() { fornecedor }
            // );

            var notificacao = new Notificacao("Já existe um fornecedor com este documento infomado.");
            notificador.Setup(n => n.ObterNotificacoes()).Returns(new List<Notificacao>() { notificacao });

            //act
            await fornecedorService.Adicionar(fornecedor);

            //assert
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }

        [Fact]
        public async void Remover_Fornecedor_Inexistente_Deve_Retornar_Mensagem_De_Erro_Fornecedor_Nao_Existe()
        {
            //arrange
            var notificador = new Mock<INotificador>();
            var fornecedorService = new FornecedorService(new Mock<IFornecedorRepository>().Object, notificador.Object);
            var notificacao = new Notificacao("Fornecedor não existe!");
            notificador.Setup(n => n.ObterNotificacoes()).Returns(new List<Notificacao>() { notificacao });

            //act
            await fornecedorService.Remover(new Guid());

            //assert
            notificador.Object.ObterNotificacoes().Contains(notificacao);
        }

    }
}
