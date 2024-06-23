using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProdutosApi.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected bool OperacaoValida()
    {
        return true;
    }

    protected ActionResult CustomResponse(object result = null)
    {
        if (OperacaoValida())
        {
            return new ObjectResult(result);
        }

        return BadRequest(new
        {
            // errors = Obter erros
        });
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid) { } // Notificar erros
        return CustomResponse();
    }

    protected void NotificarErro(string mensagem)
    {

    }
}