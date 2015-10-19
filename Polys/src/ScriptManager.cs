using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys
{
    class ScriptManager
    {
        /** Attempts to retrieve a value from a table, returning a default if failed.
          * @param table The table to retrieve a value from
          * @param str The key to retrieve from the table.
          * @param def The default value to return if retrieval fails.
          * The curently supported types to be retrieved are string, int and double. Any other type will return def. */
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

        /** Interprets a script, creating a table for each object parameter. This table is then used to initialise the object.
          * @param scriptPath The path of the script to load.
          * @param objects The objects to initialise.
          */
        public static void initialiseFromScript(string scriptPath, params IScriptInitialisable[] objects)
        {
            //Screate script
            MoonSharp.Interpreter.Script script = new MoonSharp.Interpreter.Script();

            //Create and set tables
            MoonSharp.Interpreter.Table[] tables = new MoonSharp.Interpreter.Table[objects.Length];
            for (int i = 0; i < tables.Length; ++i)
            {
                tables[i] = new MoonSharp.Interpreter.Table(script);
                script.Globals[objects[i].ScriptName()] = tables[i];
            }

            //Fill tables
            script.DoFile(scriptPath);

            //Initialise from tables
            for(int i = 0; i < tables.Length; ++i)
                objects[i].InitialiseFromScript(tables[i]);
        }
    }
}
