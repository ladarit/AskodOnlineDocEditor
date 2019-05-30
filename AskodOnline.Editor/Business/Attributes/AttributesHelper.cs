using System.Reflection;

namespace AskodOnline.Editor.Business.Attributes
{
    public class AttributesHelper
    {
        public static bool HasDbConnectionAttribute(MethodInfo methodInfo)
        {
            var attr = methodInfo.GetCustomAttribute(typeof(RequiredDbConnectionAttribute));
            return attr != null;
        }

	    public static bool HasSqlLiteDbConnectionAttribute(MethodInfo methodInfo)
	    {
		    var attr = methodInfo.GetCustomAttribute(typeof(RequiredSqlLiteDbConnectionAttribute));
		    return attr != null;
	    }
	}
}