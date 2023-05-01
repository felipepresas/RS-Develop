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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("ScoreboardManager instance created.");
        }
        else if (instance != this)
        {
            Debug.LogWarning("Another instance of ScoreboardManager found, destroying it.");
            Destroy(gameObject);
            
        }
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

                Debug.Log(
                    $"Entrada {entryCount}: Nombre de usuario: {username}, kills: {kills}, Muertes: {deaths}, xp: {xp}"
                );

                GameObject scoreboardElement = Instantiate(
                    scoreboardEntryPrefab,
                    scoreboardContent
                );
                scoreboardElement
                    .GetComponent<ScoreElement>()
                    .NewScoreElement(username, kills, deaths, xp);

                entryCount++;
            }
        }

        Debug.Log("LoadScoreboardData - Fin");
    }

    public void UpdatePlayerExperience(string userId, int experienceToAdd)
    {
        StartCoroutine(UpdatePlayerExperienceCoroutine(userId, experienceToAdd));
    }

    private IEnumerator UpdatePlayerExperienceCoroutine(string userId, int experienceToAdd)
    {
        var DBTask = DBreference.Child("Usuarios").Child(userId).Child("xp").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(
                message: $"Failed to get current experience value: {DBTask.Exception}"
            );
        }
        else
        {
            int currentExperience = int.Parse(DBTask.Result.Value.ToString());
            int newExperience = currentExperience + experienceToAdd;

            var updateTask = DBreference
                .Child("Usuarios")
                .Child(userId)
                .Child("xp")
                .SetValueAsync(newExperience);
            yield return new WaitUntil(predicate: () => updateTask.IsCompleted);

            if (updateTask.Exception != null)
            {
                Debug.LogWarning(
                    message: $"Failed to update experience value: {updateTask.Exception}"
                );
            }
            else
            {
                Debug.Log("Experience updated successfully");
            }
        }
    }
}
