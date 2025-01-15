using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RunTimeTableLoader : MonoBehaviour
{
    [SerializeField] private string googleSheetUrl = "https://docs.google.com/spreadsheets/u/0/d/e/2PACX-1vS060kaaA2anfxm0d_pqa6fXJBLk2xMFd8Y_8dazb19_iZJMDF1ttkO3y_nsLyyhEqCY9kCDOlpVpm6/pubhtml?gid=0&single=true&pli=1";
    [SerializeField] GameObject tableParser;
    #if PARSER_HTML_GOOGLE_SHEET
    #endif
    IEnumerator Start()
    {
        yield return DownloadGoogleSheet();
        if (tableParser.TryGetComponent<ITableDownloadListener>(out var tableDownloadListener))
        {
            tableDownloadListener.DonwloadComplete();
        }
    }

    private IEnumerator DownloadGoogleSheet()
    {
        UnityWebRequest request = UnityWebRequest.Get(googleSheetUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string htmlContent = request.downloadHandler.text;
            SaveToFile(htmlContent);
        }
        else
            Debug.LogError($"Error al descargar: {request.error}");
    }

    private void SaveToFile(string content)
    {
        string path = Path.Combine(Application.persistentDataPath, "GoogleSheet.html");

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
