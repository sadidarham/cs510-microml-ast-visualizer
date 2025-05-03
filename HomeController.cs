using Microsoft.AspNetCore.Mvc;
using MicroMLASTVisualizer.Models;

namespace MicroMLASTVisualizer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Parse([FromBody] ParseRequest request)
    {
        try
        {
            var parser = new MicroMLParser();
            var ast = parser.Parse(request.Code);
            return Json(new { success = true, ast = ConvertToJson(ast) });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }
    
    private object ConvertToJson(AstNode node)
    {
        var result = new Dictionary<string, object>();
        result["type"] = node.Type;
        
        switch (node)
        {
            case LiteralNode lit:
                result["value"] = lit.Value;
                break;
            case VariableNode var:
                result["name"] = var.Name;
                break;
            case BinaryOperationNode binOp:
                result["operator"] = binOp.Operator;
                break;
            case FunctionNode func:
                result["parameter"] = func.Parameter;
                break;
            case LetNode let:
                result["variable"] = let.Variable;
                break;
        }
        
        if (node.Children.Count > 0)
        {
            var children = new List<object>();
            foreach (var child in node.Children)
                children.Add(ConvertToJson(child));
            result["children"] = children;
        }
        
        return result;
    }
}

public class ParseRequest
{
    public string Code { get; set; }
}