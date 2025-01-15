﻿using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if PARSER_HTML_GOOGLE_SHEET
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System.Linq;
#endif

public class ParseGoogleSheetHTML : MonoBehaviour, ITableDownloadListener
{
    [SerializeField] GameObject[] tableInjectable;
    Dictionary<string, List<string>> dicTagsClass = new Dictionary<string, List<string>>();

    public async void DonwloadComplete()
    {
        await Task.Delay(1000);
        string path = Path.Combine(Application.persistentDataPath, "GoogleSheet.html");
        string htmlContent = System.IO.File.ReadAllText(path);

        htmlContent = ReplaceDeclarations(htmlContent);
#if PARSER_HTML_GOOGLE_SHEET
        var domHtmlTupla = await GetDom(htmlContent);
        var str = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "font-style", "italic");
        InsertTags(str);
        var str1 = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "font-weight", "bold");
        InsertTags(str1);
        var str2 = FidnCSSPropertyClass(domHtmlTupla.Item1, domHtmlTupla.Item2, "text-decoration", "line-through");
        InsertTags(str2);
        ExtractTableData(domHtmlTupla.Item1, domHtmlTupla.Item2);
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
    public async Task<(IDocument, IBrowsingContext)> GetDom(string htmlDocument)
    {
        // configuración para analizar HTML y CSS
        var config = Configuration.Default.WithCss();
        var context = BrowsingContext.New(config);

        // cLoad dom
        var document = await context.OpenAsync(req => req.Content(htmlDocument));
        return (document,context);
    }

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
                        Debug.Log($"Regla encontrada con {propertyName}: {propertyValue}");
                        Debug.Log($"Selector: {styleRule.SelectorText}");

                        if (propertyName.Equals("text-decoration") && propertyValue.Equals("line-through"))
                        {
                            //line middle";
                            return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"s");
                        }
                        if (propertyName.Equals("font-weight") && propertyValue.Equals("bold"))
                        {
                            //bold;
                        return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"b");
                        }
                        if (propertyName.Equals("font-style") && propertyValue.Equals("italic"))
                        {
                            //Italic
                           return (styleRule.SelectorText.Replace(".ritz .waffle ", ""),"i");
                        }
                    }
                }
            }
        }
        return (string.Empty, string.Empty);
    }

    private void ExtractTableData(IDocument dom, IBrowsingContext context)
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

                        foreach (var item in dicTagsClass)
                        {

                            foreach (var tagsInner in item.Value)
                            {
                                tdContent = Tagged(tdContent, classAttribute,item.Key,tagsInner);
                            }

                        }
                        rowsTemp.Add(tdContent);
                        rowData += tdContent + "\t";
                    }
                    rowsColumns.Add(rowsTemp);
                    Debug.Log($"Fila: {rowData}");
                }
            }
        }
        else
        {
            Debug.LogError("No se encontraron etiquetas <tr> en el archivo HTML.");
        }

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

    private string Tagged(string contet, string theClass, string classComparation ,string theTagResult) 
    {
        if(string.IsNullOrEmpty(classComparation))
            return contet;

        if(string.IsNullOrEmpty(theClass))
            return contet;

        if (classComparation.Contains(theClass))
        {
            print("PONEMOS "+theTagResult);
            return $"<{theTagResult}>{contet}</{theTagResult}>";
        }
            //if (contet)
        return contet;
    }

    /// <summary>
    /// Replace the declarations for unity TextMeshProTags
    /// </summary>
    /// <param name="content"> in html</param>
    /// <returns> sring with rich text Tags</returns>
    private string ReplaceDeclarations(string content)
    {
        print(content == null);
        string htmlContent = content;

        // expresion regular para capturar el estilo del <span>
        string pattern = @"<span style=""(.*?)"">(.*?)</span>";

        htmlContent = Regex.Replace(htmlContent, pattern, match =>
        {
            // Obtener el contenido capturado
            string styles = match.Groups[1].Value; // Estilos dentro de style=""
            string text = match.Groups[2].Value;   // Texto dentro del <span>

            // Comenzar con el texto original
            string replacement = text;

            // Aplicar reemplazos basados en los estilos detectados
            if (styles.Contains("text-decoration:line-through;"))
            {
                replacement = $"<s>{replacement}</s>";
            }
            if (styles.Contains("font-weight:bold;"))
            {
                replacement = $"<b>{replacement}</b>";
            }
            if (styles.Contains("font-style:italic;"))
            {
                replacement = $"<i>{replacement}</i>";
            }
            return replacement;
        });

        return htmlContent;
    }
}
