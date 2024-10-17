using garage87.Enums.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace garage87.Enums
{
    public class EnumHelper
    {
        public static string GetHeading(Type type, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                MemberInfo[] memInfo = type.GetMember(value);

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(HeadingAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((HeadingAttribute)attrs[0]).Heading;
                    }
                }
            }
            return value;
        }

        public static List<EnumModel> GetModelList(Type type)
        {
            var list = new List<EnumModel>();

            var values = System.Enum.GetNames(type);

            foreach (var val in values)
            {
                var obj = new EnumModel();

                obj.Key = val;

                obj.Heading = GetHeading(type, val);

                obj.Id = (int)System.Enum.Parse(type, val);

                list.Add(obj);
            }

            return list;
        }
    }
}
