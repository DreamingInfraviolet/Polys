namespace Polys
{
    /** A class that can be initialised from script may implement this interface to allow easy initialisation through the script manager. */
    interface IScriptInitialisable
    {
        /** Returns the desired table name in the script to be created and used as initialiser. */
        string ScriptName();

        /** Initialises the object from the table, which was filled from within the script. */
        void InitialiseFromScript(MoonSharp.Interpreter.Table table);
    }
}
