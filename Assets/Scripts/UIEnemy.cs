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
    private HealthController enemyHealthController;

    private void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        enemyHealthController = enemyController.GetComponent<HealthController>();
        
        if (enemyHealthController != null)
        {
            enemyHealthController.OnHealthChanged += UpdateEnemyHealthUI;
            UpdateEnemyHealthUI();
        }
    }

    private void OnDestroy()
    {
        if (enemyHealthController != null)
        {
            enemyHealthController.OnHealthChanged -= UpdateEnemyHealthUI;
        }
    }

    public void UpdateEnemyHealthUI()
    {
        UpdateEnemyHealthText();
        UpdateEnemyHealthSlider();
    }

    private void UpdateEnemyHealthText()
    {
        enemyHealthText.text = "Vida enemigo: " + enemyHealthController.GetHealth();
    }

    private void UpdateEnemyHealthSlider()
    {
        enemyHealthSlider.maxValue = enemyHealthController.GetMaxHealth();
        enemyHealthSlider.value = enemyHealthController.GetHealth();
    }
}
