using ProdutosApi.Business.Interfaces;
using ProdutosApi.Business.Notificacoes;
using ProdutosApi.Business.Services;
using ProdutosApi.Data.Context;
using ProdutosApi.Data.Repository;

namespace ProdutosApi.Configurations;

public static class DependencyInjectionConfig
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        // Data
        services.AddScoped<MeuDbContext>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();

        // Business
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<INotificador, Notificador>();

        return services;
    }
}