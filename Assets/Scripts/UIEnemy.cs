using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIEnemy : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI enemyHealthText;

    [SerializeField]
    private Slider enemyHealthSlider;

    private EnemyController enemyController;

    private void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        UpdateEnemyHealthText();
        UpdateEnemyHealthSlider();
    }

    public void UpdateEnemyHealthText()
    {
        enemyHealthText.text = "Vida enemigo: " + enemyController.GetHealth();
    }

    public void UpdateEnemyHealthSlider()
    {
        enemyHealthSlider.maxValue = enemyController.GetMaxHealth();
        enemyHealthSlider.value = enemyController.GetHealth();
    }
}
