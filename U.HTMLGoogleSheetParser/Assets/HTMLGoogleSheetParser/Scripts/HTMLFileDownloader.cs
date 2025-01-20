using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
#if PARSER_HTML_GOOGLE_SHEET
public class HTMLFileDownloader
{
    string url = "";
    string nameFile = "";
    public HTMLFileDownloader(string url, string nameFile)
    {
        this.url = url;
        this.nameFile = nameFile;
    }

    public async void MakeRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // Esperar hasta que la solicitud se complete
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error en la solicitud: {webRequest.error}");
            }

            string htmlContent = webRequest.downloadHandler.text;
            SaveToFile(htmlContent);
        }
    }

    private void SaveToFile(string content)
    {
        string path = Path.Combine(Application.persistentDataPath, nameFile+".html");

        if (File.Exists(path))
        {
            string existingContent = File.ReadAllText(path);
            if (existingContent == content)
            {
                Debug.Log("el contenido del archivo no cambió, no se sobrescribe.");
                return;
            }
        }

        File.WriteAllText(path, content);
        Debug.Log($"Archivo guardado en: {path}");
    }
}


#endif