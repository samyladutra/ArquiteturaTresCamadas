using Moq;
using ProdutosApi.Business.Models;

namespace ProdutosApi.Business.Tests;

public class FornecedorServiceTests
{
    [Fact]
    public void Atualizar_Fornecedor_Deve_Retornar_Sucesso()
    {
        var fornecedor = new Mock<Fornecedor>();
        var fornecedorService = new Mock<FornecedorService>();

        fornecedorService.Adicionar(fornecedor);

        
    }
}