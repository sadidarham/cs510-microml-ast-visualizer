namespace MicroMLASTVisualizer.Models;

public abstract class AstNode
{
    public string Type { get; protected set; }
    public List<AstNode> Children { get; } = new List<AstNode>();
}

public class LiteralNode : AstNode
{
    public object Value { get; }
    
    public LiteralNode(object value)
    {
        Type = "Literal";
        Value = value;
    }
}

public class VariableNode : AstNode
{
    public string Name { get; }
    
    public VariableNode(string name)
    {
        Type = "Variable";
        Name = name;
    }
}

public class BinaryOperationNode : AstNode
{
    public string Operator { get; }
    
    public BinaryOperationNode(string op, AstNode left, AstNode right)
    {
        Type = "BinaryOp";
        Operator = op;
        Children.Add(left);
        Children.Add(right);
    }
}

public class FunctionNode : AstNode
{
    public string Parameter { get; }
    
    public FunctionNode(string param, AstNode body)
    {
        Type = "Function";
        Parameter = param;
        Children.Add(body);
    }
}

public class ApplicationNode : AstNode
{
    public ApplicationNode(AstNode func, AstNode arg)
    {
        Type = "Application";
        Children.Add(func);
        Children.Add(arg);
    }
}

public class LetNode : AstNode
{
    public string Variable { get; }
    
    public LetNode(string var, AstNode value, AstNode body)
    {
        Type = "Let";
        Variable = var;
        Children.Add(value);
        Children.Add(body);
    }
}