using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Animator animator;
    public ParticleSystem healthEffect;

    [Header("UI")]
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI playerNameText;

    [Header("Scene Transition")]
    public SceneTransitionManager sceneTransitionManager;
    private EnemyData enemy;

    [Header("References")]
    public GameObject playerGameObject;
    public GameObject enemyGameObject;
    public Image enemyHealthBar;
    public Image playerHealthBar;

    void Start()
    {
        enemy = CombatContext.Instance.enemyData;
        if (enemy == null) return;
        var player = CombatContext.Instance.playerData;
        if (player == null) return;

        enemy.currentHP = enemy.maxHP;
        enemyNameText.text = $"{enemy.enemyName} (Lvl. {enemy.level})";
        enemyHealthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, enemyHealthBar.rectTransform.sizeDelta.y);

        player.currentHP = player.maxHP;
        playerNameText.text = $"{player.ingameName} (Lvl. {player.level})";
        playerHealthBar.rectTransform.sizeDelta = new Vector2((float)player.currentHP / player.maxHP * 7.859985f, playerHealthBar.rectTransform.sizeDelta.y);
    }

    void Update()
    {
        if (enemy == null) return;
        enemyHealthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, enemyHealthBar.rectTransform.sizeDelta.y);
        var player = CombatContext.Instance.playerData;
        if (player == null) return;
        playerHealthBar.rectTransform.sizeDelta = new Vector2((float)player.currentHP / player.maxHP * 7.859985f, playerHealthBar.rectTransform.sizeDelta.y);
    }

    public void TakeDmg()
    {
        healthEffect.Play();
        animator.SetTrigger("Hit");
        enemy.currentHP -= 1;
        if (enemy.currentHP <= 0)
        {
            var ctx = CombatContext.Instance;
            ctx.enemyDefeated = true;
            enemy.currentHP = 0;
            sceneTransitionManager.FadeToScene(CombatContext.Instance.returnSceneName);
        }

    }
}
