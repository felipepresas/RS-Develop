using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    public static string currentSceneName;
    private string previousSceneName;

    //Firebase variables
    public static FirebaseManager instance;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField xpField;
    public TMP_InputField killsField;
    // public TMP_InputField deathsField;
    // public GameObject scoreElement;
    public Transform scoreboardContent;

    // Agrega una referencia al ScoreboardManager
    public ScoreboardManager scoreboardManager;

    void Awake()
    {
        //Verifica que todas las dependencias de FireBase esten en el sistema
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    //Si estan todas disponibles inicializa Firebase
                    Invoke("InitializeFirebase", 2f); // Agrega tiempo de espera antes de inicializar Firebase
                }
                else
                {
                    Debug.LogError(
                        "No se pudieron inicializar todas las dependiencias de Firebase: "
                            + dependencyStatus
                    );
                }
                currentSceneName = SceneManager.GetActiveScene().name;
            });

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (auth == null)
        {
            InitializeFirebase();
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Iniciando Firebase Auth");
        //Instancia el objeto de autenticacion
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        if (auth != null)
        {
            Debug.Log("Firebase Auth inicializado correctamente");
        }
        else
        {
            Debug.LogError("Error al inicializar Firebase Auth");
        }
    }

    // limpia los registros de los campos
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Funcion para el boton de login
    public void LoginButton()
    {
        if (auth != null)
        {
            // Llama la corrutina de login pasando el correo y la contraseña como parametros
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }
        else
        {
            Debug.LogError("Firebase Auth no está inicializado.");
            warningLoginText.text = "Error de conexión. Por favor, inténtalo de nuevo.";
        }
    }

    public string GetUserId()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            return user.UserId;
        }
        else
        {
            Debug.LogError("User not signed in");
            return null;
        }
    }

    //Funcion para el boton de Regristro

    public void RegisterButton()
    {
        //Llama la corrutina de registro pasando el correo, la contraseña y el usuario como parametros
        StartCoroutine(
            Register(
                emailRegisterField.text,
                passwordRegisterField.text,
                usernameRegisterField.text
            )
        );
    }

    // Funcion de desconexion
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFeilds();
        ClearLoginFeilds();
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        StartCoroutine(UpdateXp(int.Parse(xpField.text)));
        StartCoroutine(UpdateKills(int.Parse(killsField.text)));
        //StartCoroutine(UpdateDeaths(int.Parse(deathsField.text)));
        // Save game data
        StartCoroutine(UpdateGameData());
    }

    public void ScoreboardButton()
    {
        // Delega la carga de datos de la tabla de puntuaciones al script ScoreboardManager
        scoreboardManager.LoadScoreboardData();

        // Cambia a la pantalla de Scoreboard
        UIManager.instance.ScoreboardScreen();
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Llama Firebase auth signin funcion pasando el correo, la contraseña y el usuario
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Espera hasta que la tarea termine
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // Si existe un error se controla
            Debug.LogWarning(message: $"Fallo al registrar {LoginTask.Exception}");
            FirebaseException firebaseEx =
                LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Fallo en el Login!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Falta correo";
                    break;
                case AuthError.MissingPassword:
                    message = "Falta contraseña";
                    break;
                case AuthError.WrongPassword:
                    message = "Contraseña equivocada";
                    break;
                case AuthError.InvalidEmail:
                    message = "correo invalido";
                    break;
                case AuthError.UserNotFound:
                    message = "La cuenta no existe";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //Usuario se encuentra logeado
            //Ahora se obtiene resultados
            User = LoginTask.Result;
            Debug.LogFormat(
                "Usuario registrado correctamente: {0} ({1})",
                User.DisplayName,
                User.Email
            );
            warningLoginText.text = "";
            confirmLoginText.text = "Conectando ...";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            usernameField.text = User.DisplayName;
            PlayerPrefs.SetString("UserName", User.DisplayName);
            UIManager.instance.UserDataScreen(); // Change to user data UI
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //Si el nombre de usuario esta vacio muestra mensaje
            warningRegisterText.text = "Falta nombre Usuario";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //Si la contraseña no coincide con la BD
            warningRegisterText.text = "Contraseña incorrecta!";
        }
        else
        {
            //Llama Firebase auth signin funcion pasando el correo, la contraseña y el usuario
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Espera hasta que complete tareas
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //Si existe un error se controla
                Debug.LogWarning(message: $"Fallo al registrar {RegisterTask.Exception}");
                FirebaseException firebaseEx =
                    RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Registro fallo!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Falta un correo";
                        break;
                    case AuthError.MissingPassword:
                        message = "Falta una contraseña";
                        break;
                    case AuthError.WeakPassword:
                        message = "Contraseña muy devil";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Correo se encuentra en uso";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //Usuario ahora fue creado y ahora obtenemos resultados
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Crea un perfil de usuario y le asigna un nombre de usuario
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Llama Firevase auth y actualiza el perfil de usuario pasando el perfil con el nombre de usuario
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Espera hasta que complete tareas
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //Si existe un error se controla
                        Debug.LogWarning(message: $"Fallo en el registro {ProfileTask.Exception}");
                        FirebaseException firebaseEx =
                            ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Fallo de usuario!";
                    }
                    else
                    {
                        //Usuario ahora esta agregado
                        //Ahora se regresa a la ventana de login
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Crea un perfil de usuario y introduce el nombrede usuario
        UserProfile profile = new UserProfile { DisplayName = _username };

        //llama Firebase auth actualiza el perfil pasando el perfil con el usuario
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //espera hasta que la tarea se complete
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {ProfileTask.Exception}");
        }
        else
        {
            //Auth nombre de usuario actualizado
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Coloca los valores de nombre de usuario en la BD
        var DBTask = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("Nombre usuario")
            .SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            //BD nombre de usuario esta actualizado
        }
    }

    private IEnumerator UpdateXp(int _xp)
    {
        //Agrega el valor de xp al usuario logeado
        var DBTask = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("xp")
            .SetValueAsync(_xp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            //Xp esta ahora actualizado
        }
    }

    private IEnumerator UpdateKills(int _kills)
    {
        //Agrega el valor de kills al usuario logeado
        var DBTask = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("kills")
            .SetValueAsync(_kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }

    private IEnumerator UpdateDeaths(int _deaths)
    {
        //Agrega el valor de muertes al usuario logeado
        var DBTask = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("Muertes")
            .SetValueAsync(_deaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            //Muertes actualizadas
        }
    }

    private IEnumerator UpdateGameData()
    {
        int points = PlayerPrefs.GetInt("Puntos", 0);
        float timeRemaining = PlayerPrefs.GetFloat("TimeRemaining", 0);

        var DBTaskPoints = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("Puntos")
            .SetValueAsync(points);

        var DBTaskTimeRemaining = DBreference
            .Child("Usuarios")
            .Child(User.UserId)
            .Child("TiempoRestante")
            .SetValueAsync(timeRemaining);

        yield return new WaitUntil(
            predicate: () => DBTaskPoints.IsCompleted && DBTaskTimeRemaining.IsCompleted
        );

        if (DBTaskPoints.Exception != null || DBTaskTimeRemaining.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to save game data");
        }
        else
        {
            Debug.Log(message: "Game data saved successfully");
        }
    }

    public IEnumerator LoadUserData()
{
    // Verifica si el objeto User es null antes de continuar
    if (User == null)
    {
        Debug.LogError("User object is null");
        yield break;
    }

    // Obtiene la referencia a la base de datos de Firebase
    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

    // Solicita los datos del usuario actual
    var getUserDataTask = reference.Child("Usuarios").Child(User.UserId).GetValueAsync();
    yield return new WaitUntil(predicate: () => getUserDataTask.IsCompleted);

    if (getUserDataTask.Exception != null)
    {
        Debug.LogError("Error al obtener datos del usuario: " + getUserDataTask.Exception);
    }
    else if (getUserDataTask.IsCompleted)
    {
        DataSnapshot snapshot = getUserDataTask.Result;

        // Aquí es donde actualizas la información del usuario en la interfaz de usuario
        // según los datos recuperados en 'snapshot'

        // Por ejemplo, si tienes campos para xp, kills y deaths en tu UI
        xpField.text = snapshot.Child("xp").Value.ToString();
        killsField.text = snapshot.Child("kills").Value.ToString();
        //deathsField.text = snapshot.Child("deaths").Value.ToString();
    }
}

    private IEnumerator LoadScoreboardData()
    {
        //Obtiene toda la informacion de los usuarios ordenado por la cantidad de Kills
        var DBTask = DBreference.Child("Usuarios").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Fallo al registrar la tarea {DBTask.Exception}");
        }
        else
        {
            //informacion a sido recuperada
            DataSnapshot snapshot = DBTask.Result;

            //Elimina cualquier tabla existente de elementos de puntuacion
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Itera sobre todos los usuarios UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("Nombre usuario").Value.ToString();
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("Muertes").Value.ToString());
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //instancia una nueva tabla de puntuacion
                // GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                // scoreboardElement
                //     .GetComponent<ScoreElement>()
                //     .NewScoreElement(username, kills, deaths, xp);
            }

            // ir hasta la nueva pantalla de puntuacion
            UIManager.instance.ScoreboardScreen();
        }
    }

    public void LoadGameScene()
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("RokkanGame");
    }

    public void AddAchievementToUser(string userId, string achievementId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("LogrosUsuarios")
            .Child(userId)
            .Child(achievementId)
            .SetValueAsync(true);
    }

    public void GetUnlockedAchievements(string userId, Action<Dictionary<string, bool>> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("LogrosUsuarios")
            .Child(userId)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error al obtener logros desbloqueados");
                }
                else
                {
                    DataSnapshot dataSnapshot = task.Result;
                    Dictionary<string, bool> unlockedAchievements =
                        dataSnapshot.Value as Dictionary<string, bool>;
                    callback(unlockedAchievements);
                }
            });
    }

    public void GetAchievementInfo(
        string achievementId,
        Action<Dictionary<string, object>> callback
    )
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Logros")
            .Child(achievementId)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error al obtener información del logro");
                }
                else
                {
                    DataSnapshot dataSnapshot = task.Result;
                    Dictionary<string, object> achievementInfo =
                        dataSnapshot.Value as Dictionary<string, object>;
                    callback(achievementInfo);
                }
            });
    }
}
