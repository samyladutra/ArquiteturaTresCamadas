using ProdutosApi.Business.Models;

public class ProdutoBuilder
{
    private Produto _produto;
    public ProdutoBuilder()
    {
        _produto = new Produto();
    }

    public ProdutoBuilder ComId(Guid id)
    {
        _produto.Id = id;
        return this;
    }

    public ProdutoBuilder ComNome(string nome)
    {
        _produto.Nome = nome;
        return this;
    }

    public ProdutoBuilder ComDescricao(string descricao)
    {
        _produto.Descricao = descricao;
        return this;
    }

    public ProdutoBuilder ComValor(decimal valor)
    {
        _produto.Valor = valor;
        return this;
    }

    public ProdutoBuilder ComDataCadastro(DateTime dataCadastro)
    {
        _produto.DataCadastro = dataCadastro;
        return this;
    }

    public ProdutoBuilder Ativo(bool ativo)
    {
        _produto.Ativo = ativo;
        return this;
    }

    public Produto Build()
    {
        return _produto;
    }
}