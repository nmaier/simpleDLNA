using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Utilities
{
  public class ReflectionHelper
  {
    static readonly Regex _parser = new Regex("(?<Token>[^=;]*)((=\"(?<Value>[^\"]*)\")|(=(?<Value>[^;]*)))?", RegexOptions.IgnoreCase);

    public static Dictionary<string,string> StringToDictionary(string param) {
      return _parser
        .Matches(param)
        .OfType<Match>()
        .Where(m => m.Success && (m.Length > 0))
        .ToDictionary(
          m => m.Groups["Token"].ToString().Trim()
          ,m => m.Groups["Value"].ToString()
        );
    }

    public static Type FindType(string parameter) {
      var dparam = StringToDictionary(parameter);
      if(!dparam.ContainsKey(AssemblyParameter)) {
        return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(dparam["type"])).FirstOrDefault();
      }
      else {
        var a = Assembly.Load(dparam[AssemblyParameter]);
        if (a == null) throw new Exception(string.Format("Failed to load assembly [{0}]",dparam[AssemblyParameter]));
        var t = a.GetType(dparam[TypeParameter]);
        if (t == null) throw new Exception(string.Format("Failed to get type [{0}]",dparam[TypeParameter]));
        return t;
      }
    }

    public const string AssemblyParameter = "assembly";
    public const string TypeParameter = "type";

    public static object Create(string parameter) {
      var t = FindType(parameter);
      var constr = t.GetConstructor(Type.EmptyTypes);
      if (constr == null) throw new Exception(string.Format("Can't find a default constructor for type:[{0}]", t.Name));
      return constr.Invoke(null);
    }
  }
}
