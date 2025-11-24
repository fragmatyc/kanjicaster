using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Animator animator;
    public Image healthBar;
    public ParticleSystem healthEffect;

    [Header("UI")]
    public TextMeshProUGUI enemyNameText;

    [Header("Audio")]
    public AudioClip music;
    [Header("Scene Transition")]
    public SceneTransitionManager sceneTransitionManager;
    private EnemyData enemy;

    void Start()
    {
        enemy = CombatContext.Instance.enemyData;
        if (enemy == null) return;

        enemy.currentHP = enemy.maxHP;
        enemyNameText.text = $"{enemy.enemyName} (Lvl. {enemy.level})";
        healthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, healthBar.rectTransform.sizeDelta.y);

        if (music != null)
        {
            AudioSource.PlayClipAtPoint(music, Camera.main.transform.position);
        }
    }

    void Update()
    {
        if (enemy == null) return;
        healthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, healthBar.rectTransform.sizeDelta.y);
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
