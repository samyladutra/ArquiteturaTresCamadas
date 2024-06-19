using ProdutosApi.Business.Notificacoes;

namespace ProdutosApi.Business.Interfaces;

public interface INotificador
{
    bool TemNotificacao();
    List<Notificacao> ObterNotificacoes();
    void Handle(Notificacao notificacao);
}

