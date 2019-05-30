using System;

namespace AskodOnline.Editor.Business.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RequiredSqlLiteDbConnectionAttribute : Attribute
	{
	}
}