
namespace RangoAgil.API.EndpointFilters;

public class RangoIsLockedFilter : IEndpointFilter
{
    public readonly int _lockedRangoId;

    public RangoIsLockedFilter(int lockedRantoId)
    {
          _lockedRangoId = lockedRantoId;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        
        int rangoId;
        if (context.HttpContext.Request.Method == "PUT")
        {
            rangoId = context.GetArgument<int>(2); //  terceiro argumento do método 
        }
        else if (context.HttpContext.Request.Method == "DELETE")
        {
            rangoId = context.GetArgument<int>(1); //  segundo argumento do método         }
        }
        else
        {
            throw new NotSupportedException("This filter is not suported for this scenario.");
        }

       // var tropeiroId = 15;

        if (rangoId == _lockedRangoId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Rango já é perfeito, você não precisa modificar nem deletar nada aqui.",
                Detail = "Você não pode modificar nem deletar esta receita."
            });
        }
        var result = await next.Invoke(context);
        return result;
    }
}
