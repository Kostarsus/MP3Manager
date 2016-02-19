using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MP3ManagerBase.manager
{
    /// <summary>
    /// Gibt die zusätzliche Priorität beim suchen von Duplikaten an
    /// </summary>
    public enum EDuplicateCheckPriority
    {
        NONE,
        CREATIONDATE
    }
}