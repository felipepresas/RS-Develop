using System.Collections;
using System.Linq;
using UnityEngine;
using Firebase.Database;

public class ScoreboardManager : MonoBehaviour
{
    public GameObject scoreboardEntryPrefab;
    public Transform scoreboardContent;
    public static ScoreboardManager instance;

    private DatabaseReference DBreference;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ScoreboardManager instance found!");
            return;
        }
        instance = this;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator LoadScoreboardData()
{
    Debug.Log("LoadScoreboardData - Inicio");

    var DBTask = DBreference.Child("Usuarios").OrderByChild("kills").GetValueAsync();

    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
    }
    else
    {
        DataSnapshot snapshot = DBTask.Result;

        Debug.Log($"LoadScoreboardData - Datos recuperados: {snapshot.ChildrenCount} entradas");

        foreach (Transform child in scoreboardContent.transform)
        {
            Destroy(child.gameObject);
        }

        int entryCount = 0;
        foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
        {
            string username = childSnapshot.Child("Nombre usuario").Value.ToString();
            int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
            int deaths = int.Parse(childSnapshot.Child("Muertes").Value.ToString());
            int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

            Debug.Log($"Entrada {entryCount}: Nombre de usuario: {username}, kills: {kills}, Muertes: {deaths}, xp: {xp}");

            GameObject scoreboardElement = Instantiate(scoreboardEntryPrefab, scoreboardContent);
            scoreboardElement
                .GetComponent<ScoreElement>()
                .NewScoreElement(username, kills, deaths, xp);
                
            entryCount++;
        }

        UIManager.instance.ScoreboardScreen();
    }

    Debug.Log("LoadScoreboardData - Fin");
}
}
