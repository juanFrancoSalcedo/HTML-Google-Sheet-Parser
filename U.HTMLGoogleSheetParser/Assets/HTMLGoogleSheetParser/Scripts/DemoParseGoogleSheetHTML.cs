using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
#if PARSER_HTML_GOOGLE_SHEET
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System.Linq;
#endif

public class DemoParseGoogleSheetHTML : MonoBehaviour, ITableDownloadListener,IExtractTable
{
    [SerializeField] GameObject[] tableInjectable;
    Dictionary<string, List<string>> dicTagsClass = new Dictionary<string, List<string>>();

    public async void DonwloadComplete()
    {
        await Task.Delay(1000);
        string path = Path.Combine(Application.persistentDataPath, "GoogleSheet.html");
        string htmlContent = System.IO.File.ReadAllText(path);

        htmlContent = CSSDeclarationsFinder.ReplaceDeclarations(htmlContent);
#if PARSER_HTML_GOOGLE_SHEET
        var domHtmlTupla = await HTMLDocumentCreator.GetDom(htmlContent);
        CSSDeclarationsFinder.DefaultStylesDeclarations(ref dicTagsClass);
        //var str = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "font-style", "italic");
        //InsertTags(str);
        //var str1 = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "font-weight", "bold");
        //InsertTags(str1);
        //var str2 = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "text-decoration", "line-through");
        //InsertTags(str2);
        ExtracTable(domHtmlTupla.Item1, dicTagsClass);
#endif

#if UNITY_WEBGL
        Debug.LogError("it is possible that the code below won't work properly on the web");
#endif
    }

    private void InsertTags((string, string) str)
    {
        if (!dicTagsClass.ContainsKey(str.Item1))
        {
            dicTagsClass[str.Item1] = new List<string>();
            dicTagsClass[str.Item1].Add(str.Item2);
        }
        else
            dicTagsClass[str.Item1].Add(str.Item2);
    }

#if PARSER_HTML_GOOGLE_SHEET

    public (string,string)FidnCSSPropertyClass(IDocument dom, IBrowsingContext context, string propertyName, string propertyValue)
    {
        var styleElements = dom.QuerySelectorAll("style");

        foreach (var styleElement in styleElements)
        {
            // Parsear el contenido CSS
            var cssParser = context.GetService<ICssParser>();
            var cssSheet = cssParser.ParseStyleSheet(styleElement.TextContent);

            // Iterar sobre las reglas CSS
            foreach (var rule in cssSheet.Rules)
            {
                if (rule is ICssStyleRule styleRule)
                {
                    // Verificar si la regla contiene la propiedad buscada
                    var style = styleRule.Style;
                    var value = style.GetPropertyValue(propertyName);

                    if (!string.IsNullOrEmpty(value) && value.Equals(propertyValue))
                    {
                        //line middle";
                        if (propertyName.Equals("text-decoration") && propertyValue.Equals("line-through"))
                            return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"s");
                        //bold;
                        if (propertyName.Equals("font-weight") && propertyValue.Equals("bold"))
                        return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"b");
                        //Italic
                        if (propertyName.Equals("font-style") && propertyValue.Equals("italic"))
                           return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"i");
                    }
                }
            }
        }
        return (string.Empty, string.Empty);
    }

    public void ExtracTable(IDocument dom, Dictionary<string, List<string>> dicto)
    {
        List<List<string>> rowsColumns = new List<List<string>>();
        var trNodes = dom.QuerySelectorAll("tr");
        if (trNodes != null)
        {
            foreach (var tr in trNodes)
            {
                var tdNodes = tr.QuerySelectorAll("td");

                if (tdNodes != null)
                {
                    List<string> rowsTemp = new List<string>();
                    string rowData = "";
                    foreach (var td in tdNodes)
                    {
                        var classAttribute = td.GetAttribute("class");
                        var tdContent = td.InnerHtml.Trim();
                        var div = td.QuerySelector("div");
                        if (div != null)
                        {
                            // Si hay un div, obtenemos su texto y lo usamos como tdContent
                            tdContent = div.TextContent.Trim();
                        }

                        foreach (var item in dicTagsClass)
                        {
                            foreach (var tagsInner in item.Value)
                            {
                                tdContent = CSSDeclarationsFinder.Tagged(tdContent, classAttribute,item.Key,tagsInner);
                            }

                        }
                        rowsTemp.Add(tdContent);
                        rowData += tdContent + "\t";
                    }
                    rowsColumns.Add(rowsTemp);
                    //Debug.Log($"Fila: {rowData}");
                }
            }
        }
        else
            Debug.LogError("No se encontraron etiquetas <tr> en el archivo HTML");

        foreach (var item in tableInjectable)
        {
            if (item.TryGetComponent<ITableInjectable>(out var compo))
            {
                compo.Configure(rowsColumns);
            }
            else
            { 
                Debug.LogError("you are about to configure a table into an object that doesn't have ITableInjectable.");
            }
        }
    }

#endif

}
