using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
namespace JWTExample.Filters
{
	public class ExceptioResultFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			var exception = context.Exception.GetType();
			context.Result = new ObjectResult($"{exception.Name} : {context.Exception.Message}")
			{
				StatusCode = 500,
				DeclaredType = typeof(string),
			};
			context.ExceptionHandled = true;
		}
	}
}

