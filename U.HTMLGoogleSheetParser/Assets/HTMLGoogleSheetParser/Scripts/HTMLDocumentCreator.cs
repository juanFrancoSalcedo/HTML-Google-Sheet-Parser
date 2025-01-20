using System.Threading.Tasks;
#if PARSER_HTML_GOOGLE_SHEET
using AngleSharp;
using AngleSharp.Dom;
public static class HTMLDocumentCreator
{ 
    public static async Task<(IDocument, IBrowsingContext)> GetDom(string htmlDocument)
    {
        // configuración para analizar HTML y CSS
        var config = Configuration.Default.WithCss();
        var context = BrowsingContext.New(config);

        // Load dom
        var document = await context.OpenAsync(req => req.Content(htmlDocument));
        return (document, context);
    }

}
#endif