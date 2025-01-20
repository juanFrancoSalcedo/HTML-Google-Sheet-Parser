using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public static class CSSDeclarationsFinder
{
#if PARSER_HTML_GOOGLE_SHEET
    public static string Tagged(string contet, string theClass, string classComparation, string theTagResult)
    {
        if (string.IsNullOrEmpty(classComparation))
            return contet;

        if (string.IsNullOrEmpty(theClass))
            return contet;

        if (classComparation.Contains(theClass))
        {
            return $"<{theTagResult}>{contet}</{theTagResult}>";
        }
        return contet;
    }

    /// <summary>
    /// Replace the declarations for unity TextMeshProTags
    /// </summary>
    /// <param name="content"> in html</param>
    /// <returns> sring with rich text Tags</returns>
    public static string ReplaceDeclarations(string content)
    {
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
#endif
    public static (string, string) FidnCSSPropertyClass(IDocument dom, IBrowsingContext context, string propertyName, string propertyValue)
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
                            return (styleRule.SelectorText.Replace(".ritz .waffle ", ""), "s");
                        //bold;
                        if (propertyName.Equals("font-weight") && propertyValue.Equals("bold"))
                            return (styleRule.SelectorText.Replace(".ritz .waffle ", ""), "b");
                        //Italic
                        if (propertyName.Equals("font-style") && propertyValue.Equals("italic"))
                            return (styleRule.SelectorText.Replace(".ritz .waffle ", ""), "i");
                    }
                }
            }
        }
        return (string.Empty, string.Empty);
    }

    public static void DefaultStylesDeclarations(ref Dictionary<string, List<string>> dictoTags)
    {
        var stylesConvertions = new (string, string)[]
        {
            ("font-style","italic"),
            ("font-weight", "bold"),
            ("text-decoration", "line-through")
        };
        InsertTags(stylesConvertions, ref dictoTags);
    }

    private static void InsertTags((string, string)[] str, ref Dictionary<string, List<string>> dictoTags)
    {
        foreach (var item in str)
        {
            if (!dictoTags.ContainsKey(item.Item1))
            {
                dictoTags[item.Item1] = new List<string>();
                dictoTags[item.Item1].Add(item.Item2);
            }
            else
                dictoTags[item.Item1].Add(item.Item2);
        }
    }
}