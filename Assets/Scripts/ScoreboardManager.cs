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
        // Obtiene toda la información de los usuarios ordenado por la cantidad de Kills
        var DBTask = DBreference.Child("Usuarios").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            // Información ha sido recuperada
            DataSnapshot snapshot = DBTask.Result;

            // Elimina cualquier tabla existente de elementos de puntuación
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            // Itera sobre todos los usuarios UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("Nombre usuario").Value.ToString();
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("Muertes").Value.ToString());
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                // Instancia una nueva entrada de la tabla de puntuaciones
                GameObject scoreboardElement = Instantiate(scoreboardEntryPrefab, scoreboardContent);
                scoreboardElement
                    .GetComponent<ScoreElement>()
                    .NewScoreElement(username, kills, deaths, xp);
            }

            // Ir hasta la nueva pantalla de puntuación
            UIManager.instance.ScoreboardScreen();
        }
    }
}
