using Microsoft.AspNetCore.Identity;
using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGroup("/identity/").MapIdentityApi<IdentityUser>();

        endpointRouteBuilder.MapGet("/pratos/{pratoid:int}", (int pratoid) => $"O prato {pratoid} é delicioso")
            .WithOpenApi(operation =>           // Microsoft.AspNetCore.OpenApi
            {
                operation.Deprecated = true;
                return operation;
            })
            .WithSummary("Este endpoint está deprecated e será descontinuado na versão 2 desta API")
            .WithDescription("Por favor utilize /rangos/{rangoid}");

        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos")
            .RequireAuthorization();
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");

        var rangosComIdAndLockFilterEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
            .RequireAuthorization("RequireAdminFromBrazil")
            .RequireAuthorization()
            .AddEndpointFilter(new RangoIsLockedFilter(15)) // se o primeiro cai, não executa o segundo com o Invoke.next (ganho de performance)
            .AddEndpointFilter(new RangoIsLockedFilter(5));
        // Chain of responsability  -  Corrente de responsabilidade

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync)
            .WithOpenApi()
            .WithSummary("Esta rota retornará todos os Rangos");

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangosComIdAsync).WithName("GetRangos")
            .AllowAnonymous();

        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();

        rangosComIdAndLockFilterEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);



        rangosComIdAndLockFilterEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
            
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientesEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes")
            .RequireAuthorization();

        ingredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        ingredientesEndpoints.MapPost("", () =>
        {
            throw new NotImplementedException();
        });

    }

}

