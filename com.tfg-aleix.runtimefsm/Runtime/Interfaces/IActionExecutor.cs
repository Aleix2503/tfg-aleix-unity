using System.Collections.Generic;

namespace RuntimeFSM.Interfaces
{
    /// <summary>
    /// Ejecuta acciones definidas en el JSON.
    /// El Core FSM no conoce implementaciones concretas.
    /// </summary>
    public interface IActionExecutor
    {
        void Execute(string actionName, Dictionary<string, string> parameters);
    }
}