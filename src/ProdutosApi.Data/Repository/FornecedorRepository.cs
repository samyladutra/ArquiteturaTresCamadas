using Microsoft.EntityFrameworkCore;
using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Models;
using ProdutosApi.Data.Context;

namespace ProdutosApi.Data.Repository;

public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
{
    public FornecedorRepository(MeuDbContext context) : base(context) { }


    public virtual async Task<Fornecedor> ObterFornecedorEndereco(Guid id)
    {
        return await Db.Fornecedores.AsNoTracking()
            .Include(c => c.Endereco)
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public virtual async Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id)
    {
        return await Db.Fornecedores.AsNoTracking()
            .Include(c => c.Produtos)
            .Include(c => c.Endereco)
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public virtual async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
    {
        return await Db.Enderecos.AsNoTracking()
            .FirstOrDefaultAsync(f => f.FornecedorId == fornecedorId);
    }

    public virtual async Task RemoverEnderecoFornecedor(Endereco endereco)
    {
        Db.Enderecos.Remove(endereco);
        await SaveChanges();
    }
}