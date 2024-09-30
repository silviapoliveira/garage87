using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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