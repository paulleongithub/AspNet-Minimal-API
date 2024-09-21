using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");

        var rangosComIdAndLockFilterEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
            .AddEndpointFilter(new RangoIsLockedFilter(15)) // se o primeiro cai, não executa o segundo com o Invoke.next (ganho de performance)
            .AddEndpointFilter(new RangoIsLockedFilter(5));
        // Chain of responsability  -  Corrente de responsabilidade

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangosComIdAsync).WithName("GetRangos");

        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();

        rangosComIdAndLockFilterEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);



        rangosComIdAndLockFilterEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
            
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientesEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");

        ingredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        ingredientesEndpoints.MapPost("", () =>
        {
            throw new NotImplementedException();
        });

    }

}

