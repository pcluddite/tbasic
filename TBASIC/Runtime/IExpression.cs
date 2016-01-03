
namespace Tbasic.Runtime
{
    /// <summary>
    /// This interface is the base for all of the expression evaluation objects.
    /// </summary>
    internal interface IExpression
    {
        string Expression { get; set; }
        object Evaluate();
        Executer CurrentExecution { get; set; }
    }
}
