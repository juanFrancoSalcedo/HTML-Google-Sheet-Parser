using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

[InitializeOnLoad]
public static class DirectiveAngleSharp
{
    static DirectiveAngleSharp()
    {
        string carpetaPrincipal = @"Assets";
        string nombreArchivo = "AngleSharp.dll";
        string nombreArchivoTwo = "AngleSharp.Css.dll";
        string symboDefinition = "PARSER_HTML_GOOGLE_SHEET";
        bool canAdd = false;

        string[] archivosEncontrados = Directory.GetFiles(carpetaPrincipal, nombreArchivo, SearchOption.AllDirectories);

        if (archivosEncontrados.Length > 0)
        {
            foreach (string ruta in archivosEncontrados)
                canAdd = true;
        }
        else
        {
            Debug.LogError("Debes instalar el paquete NuGet AngleSharp");
            canAdd = false;
        }

        string[] archivosEncontradosDos = Directory.GetFiles(carpetaPrincipal, nombreArchivoTwo, SearchOption.AllDirectories);

        if (archivosEncontradosDos.Length > 0)
        {
            foreach (string ruta in archivosEncontradosDos)
                canAdd = true;
        }
        else
        {
            Debug.LogError("Debes instalar el paquete NuGet AngleSharp.Css");
            canAdd = false;
        }

        if (canAdd)
            AddDefineSymbol(symboDefinition);
        else
            RemoveDefine(symboDefinition);
    }

    private static void AddDefineSymbol(string definesSymbol)
    {
        var targetGroup = NamedBuildTarget.Standalone;
        string defines = PlayerSettings.GetScriptingDefineSymbols(targetGroup);
        if (!defines.Contains(definesSymbol))
        {
            Debug.Log($"Enter ={defines}_");
            PlayerSettings.SetScriptingDefineSymbols(targetGroup, defines + ";" + definesSymbol);
        }
    }

    private static void RemoveDefine(string defineSymbol)
    {
        var targetGroup = NamedBuildTarget.Standalone;
        string defines = PlayerSettings.GetScriptingDefineSymbols(targetGroup);
        if (defines.Contains(defineSymbol))
        {
            PlayerSettings.SetScriptingDefineSymbols(targetGroup, defines.Replace(defineSymbol, "").Replace(";;", ";"));
        }
        Debug.Log("Remueve");
    }
}
