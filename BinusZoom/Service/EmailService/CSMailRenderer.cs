using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BinusZoom.Template.MailTemplate;

public class CSMailRenderer
{
    public async Task<string> RenderCSHtmlToString<TModel>(ControllerContext controllerContext, string templateName, TModel model)
    {
        var viewEngine = controllerContext.HttpContext.RequestServices.GetRequiredService(typeof(IRazorViewEngine)) as
                IRazorViewEngine;
        var viewEngineResult = viewEngine.FindView(controllerContext, templateName, false);
        if (viewEngineResult.View == null)
        {
            throw new Exception("Could not find the View file. Searched locations:\r\n" +
                                viewEngineResult.SearchedLocations.Aggregate((x, y) => x + "\r\n" + y));
        }

        var view = viewEngineResult.View;

        using (var stringWriter = new StringWriter())
        {
            var viewDictionary =
                new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    { Model = model };
            
            var tempDataProvider = controllerContext.HttpContext.RequestServices.GetRequiredService(typeof(ITempDataProvider)) as ITempDataProvider;

            var viewContext = new ViewContext(
                controllerContext,
                view,
                viewDictionary,
                new TempDataDictionary(controllerContext.HttpContext, tempDataProvider),
                stringWriter,
                new HtmlHelperOptions()
            );
            
            await view.RenderAsync(viewContext);

            return stringWriter.ToString();
        }
    }
}