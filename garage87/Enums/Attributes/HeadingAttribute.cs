using System;

namespace garage87.Enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HeadingAttribute : Attribute
    {
        internal HeadingAttribute(string heading)
        {
            this.Heading = heading;
        }
        public string Heading { get; private set; }
    }
}