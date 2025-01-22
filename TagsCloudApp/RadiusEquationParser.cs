using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using FluentResults;

namespace TagsCloudApp;

public static class RadiusEquationParser
{
    public static Result<Func<double, double>> ParseRadiusEquation(string radiusEquationString)
    {
        return Result.Try(
            () => ParseOrThrow(radiusEquationString),
            ex => new Error($"'{radiusEquationString}' is not a radius equation. {ex.Message}"));
    }

    private static Func<double, double> ParseOrThrow(string radiusEquationString)
    {
        var parameter = Expression.Parameter(typeof(double), "angle");
        return (Func<double, double>)DynamicExpressionParser
            .ParseLambda([parameter], typeof(double), radiusEquationString)
            .Compile();
    }
}
