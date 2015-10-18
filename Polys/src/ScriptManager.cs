using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys
{
    class ScriptManager
    {
        public static T retrieveValue<T>(MoonSharp.Interpreter.Table table, String str, T def)
        {
            MoonSharp.Interpreter.DynValue value = table.Get(str);
            if (value.IsNilOrNan())
                return def;
            else
            {
                //Do some type checking
                if (def is string && value.Type == MoonSharp.Interpreter.DataType.String)
                    return (T)Convert.ChangeType(value.String, typeof(T));
                if (def is int && value.Type == MoonSharp.Interpreter.DataType.Number)
                    return (T)Convert.ChangeType((int)value.Number, typeof(T));
                if (def is double && value.Type == MoonSharp.Interpreter.DataType.Number)
                    return (T)Convert.ChangeType(value.Number, typeof(T));
                else
                    return def;
            }
        }
    }
}
