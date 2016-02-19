using System;

namespace Tools.extensions.enums
{
    public class EnumDisplayAttribute : Attribute
    {
        #region Properties

        public string String { get; set; }

        #endregion Properties

        #region Constructor

        public EnumDisplayAttribute(string value)
        {
            this.String = value;
        }

        #endregion Constructor
    }
}