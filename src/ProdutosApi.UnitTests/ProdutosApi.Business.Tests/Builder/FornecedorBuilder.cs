using ProdutosApi.Business.Models;

public class FornecedorBuilder
{
    private Fornecedor _fornecedor;
    public FornecedorBuilder()
    {
        _fornecedor = new Fornecedor();
    }

    public FornecedorBuilder ComId(Guid id)
    {
        _fornecedor.Id = id;
        return this;
    }

    public FornecedorBuilder ComNome(string nome)
    {
        _fornecedor.Nome = nome;
        return this;
    }

    public FornecedorBuilder ComDocumento(string documento)
    {
        _fornecedor.Documento = documento;
        return this;
    }

    public FornecedorBuilder DoTipo(TipoFornecedor tipoFornecedor)
    {
        _fornecedor.TipoFornecedor = tipoFornecedor;
        return this;
    }

    public FornecedorBuilder ComEndereco(Endereco endereco)
    {
        _fornecedor.Endereco = endereco;
        return this;
    }

    public FornecedorBuilder ComProdutos(List<Produto> produtos)
    {
        _fornecedor.Produtos = produtos;
        return this;
    }

    public FornecedorBuilder Ativo(bool ativo)
    {
        _fornecedor.Ativo = ativo;
        return this;
    }

    public Fornecedor Build()
    {
        return _fornecedor;
    }
}