using FluentAssertions;
using Moq;
using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Models;
using ProdutosApi.Business.Models.Validations.Documentos;
using ProdutosApi.Business.Notificacoes;
using ProdutosApi.Business.Services;

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
    }
}
