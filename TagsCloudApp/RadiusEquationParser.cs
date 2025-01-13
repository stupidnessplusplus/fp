using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace TagsCloudApp;

public static class RadiusEquationParser
{
    public static Func<double, double> ParseRadiusEquation(string radiusEquationString)
    {
        var parameter = Expression.Parameter(typeof(double), "angle");
        var radiusEquation = DynamicExpressionParser
            .ParseLambda([parameter], typeof(double), radiusEquationString)
            .Compile();
        return (Func<double, double>)radiusEquation;
    }
}
