using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SelectPdf;
namespace BinusZoom.Service.CertificateService;
public class CSCertificateRenderer
{
    // render cshtml to pdf
    public async Task<byte[]> RenderCSHtmlToPdf<TModel>(ControllerContext controllerContext, string templateName, TModel model)
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

            var htmlContent = stringWriter.ToString();
            var converter = new HtmlToPdf();
            var pdfDocument = converter.ConvertHtmlString(htmlContent);
            var pdfBytes = pdfDocument.Save();
            pdfDocument.Close();
            return pdfBytes;
        }
    }
}