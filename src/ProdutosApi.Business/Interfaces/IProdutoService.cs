using ProdutosApi.Business.Models;

namespace ProdutosApi.Business.Interfaces;
public interface IProdutoService : IDisposable
{
    Task Adicionar(Produto produto);
    Task Atualizar(Produto produto);
    Task Remover(Guid id);
}
