namespace MicroMLASTVisualizer.Models;

public class MicroMLParser
{
    private string input;
    private int position;
    
    public AstNode Parse(string code)
    {
        input = code;
        position = 0;
        return ParseExpression();
    }
    
    private AstNode ParseExpression()
    {
        SkipWhitespace();
        
        if (position >= input.Length)
            throw new Exception("Unexpected end of input");
            
        if (input[position] == '(')
        {
            position++;
            var node = ParseParenthesized();
            SkipWhitespace();
            if (position >= input.Length || input[position++] != ')')
                throw new Exception("Expected closing parenthesis");
            return node;
        }
        
        if (char.IsDigit(input[position]))
            return ParseNumber();
            
        if (input[position] == '\\')
            return ParseFunction();
            
        if (input[position] == 'l' && position + 2 < input.Length && input[position+1] == 'e' && input[position+2] == 't')
            return ParseLet();
            
        return ParseVariable();
    }
    
    private AstNode ParseParenthesized()
    {
        SkipWhitespace();
        var first = ParseExpression();
        SkipWhitespace();
        
        // Check if it's a binary operation
        if (position < input.Length && "+-*/".Contains(input[position]))
        {
            var op = input[position++].ToString();
            var second = ParseExpression();
            return new BinaryOperationNode(op, first, second);
        }
        
        // Otherwise it's function application
        var secondExpr = ParseExpression();
        return new ApplicationNode(first, secondExpr);
    }
    
    private AstNode ParseNumber()
    {
        var start = position;
        while (position < input.Length && char.IsDigit(input[position]))
            position++;
            
        var numStr = input.Substring(start, position - start);
        return new LiteralNode(int.Parse(numStr));
    }
    
    private AstNode ParseVariable()
    {
        var start = position;
        while (position < input.Length && !char.IsWhiteSpace(input[position]) && input[position] != ')')
            position++;
            
        var name = input.Substring(start, position - start);
        return new VariableNode(name);
    }
    
    private AstNode ParseFunction()
    {
        position++; // skip '\'
        SkipWhitespace();
        
        var param = ParseVariable() as VariableNode;
        SkipWhitespace();
        
        if (position >= input.Length || input[position++] != '.')
            throw new Exception("Expected '.' in lambda");
            
        var body = ParseExpression();
        return new FunctionNode(param.Name, body);
    }
    
    private AstNode ParseLet()
    {
        position += 3; // skip 'let'
        SkipWhitespace();
        
        var variable = ParseVariable() as VariableNode;
        SkipWhitespace();
        
        if (position >= input.Length || input[position++] != '=')
            throw new Exception("Expected '=' in let expression");
            
        var value = ParseExpression();
        SkipWhitespace();
        
        if (position >= input.Length || input[position++] != 'i')
            throw new Exception("Expected 'in' in let expression");
        if (position >= input.Length || input[position++] != 'n')
            throw new Exception("Expected 'in' in let expression");
            
        var body = ParseExpression();
        return new LetNode(variable.Name, value, body);
    }
    
    private void SkipWhitespace()
    {
        while (position < input.Length && char.IsWhiteSpace(input[position]))
            position++;
    }
}