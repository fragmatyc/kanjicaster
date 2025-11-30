using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Animator playerHpAnimator;
    public ParticleSystem playerHpEffect;
    public TextMeshProUGUI playerHpText;
    public Animator enemyHpAnimator;
    public ParticleSystem enemyHpEffect;
    public TextMeshProUGUI enemyHpText;

    [Header("Combat")]
    public float delayBetweenTurns = 2f;
    public float delayBeforeEnemyAttack = 1f;
    public float delayBeforeReturnToExploration = 3f;

    [Header("Veil")]
    public Animator veilAnimator;

    [Header("Damage Text")]
    public TextMeshProUGUI playerDamageText;
    public TextMeshProUGUI enemyDamageText;

    [Header("UI")]
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI turnText;
    public Animator turnTextAnimator;
    public TextMeshProUGUI comboText;
    public GameObject comboDisplayGameObject;
    public float playedComboNameDuration = 1f;

    [Header("Scene Transition")]
    public SceneTransitionManager sceneTransitionManager;
    private EnemyData enemy;

    [Header("References")]
    public GameObject playerGameObject;
    public GameObject enemyGameObject;
    public Image enemyHealthBar;
    public Image playerHealthBar;
    public Animator playerAnimator;
    public HandManager handManager;

    public GameObject endTurnButton;

    public GameObject comboIndicator;
    public GameObject firstCardComboIndicator;
    public GameObject secondCardComboIndicator;
    public GameObject thirdCardComboIndicator;


    void Start()
    {
        var ctx = CombatContext.Instance;
        enemy = ctx.enemyData;
        if (enemy == null) return;
        var player = ctx.playerData;
        if (player == null) return;

        // Initialize enemy deck
        if (ctx.enemyDeck == null || ctx.enemyDeck.Count == 0)
        {
            ctx.enemyDeck = new System.Collections.Generic.List<CardData>(enemy.deck);
        }

        enemyGameObject = EnemyPrefabFactory.instance.CreateEnemy(enemy);
        enemy.currentHP = enemy.maxHP;
        enemyNameText.text = $"{enemy.enemyName} (Lvl. {enemy.level})";
        enemyHealthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, enemyHealthBar.rectTransform.sizeDelta.y);

        player.currentHP = player.maxHP;
        playerNameText.text = $"{player.ingameName} (Lvl. {player.level})";
        playerHealthBar.rectTransform.sizeDelta = new Vector2((float)player.currentHP / player.maxHP * 7.859985f, playerHealthBar.rectTransform.sizeDelta.y);

        if (ctx.playerData.initiative >= ctx.enemyData.initiative)
        {
            ctx.SetTurnState(TurnState.Player);
        }
        else
        {
            ctx.SetTurnState(TurnState.Enemy);
        }

        var enemyPos = enemyGameObject.transform.position;
        var enemyTranform = enemyGameObject.GetComponent<SpriteRenderer>();
        enemyPos.y += enemyTranform.bounds.extents.y;
        enemyDamageText.transform.position = enemyPos;

        StartTurn();
    }

    void Update()
    {
        if (enemy == null) return;
        enemyHealthBar.rectTransform.sizeDelta = new Vector2((float)enemy.currentHP / enemy.maxHP * 7.859985f, enemyHealthBar.rectTransform.sizeDelta.y);
        enemyHpText.text = $"{enemy.currentHP}/{enemy.maxHP}";

        var player = CombatContext.Instance.playerData;
        if (player == null) return;
        playerHealthBar.rectTransform.sizeDelta = new Vector2((float)player.currentHP / player.maxHP * 7.859985f, playerHealthBar.rectTransform.sizeDelta.y);
        playerHpText.text = $"{player.currentHP}/{player.maxHP}";
    }

    public void TakeDmg()
    {
        playerHpEffect.Play();
        playerHpAnimator.SetTrigger("Hit");
        veilAnimator.SetTrigger("Hit");
        var player = CombatContext.Instance.playerData;
        var enemy = CombatContext.Instance.enemyData;

        var playedCard = ComboManager.Instance.GetBestCardToPlay(CombatContext.Instance.enemyHand);
        CombatContext.Instance.enemyHand.Clear();

        int damage = 0;
        if (playedCard.attack > 0)
        {
            damage = playedCard.attack;
            player.currentHP -= damage;
            StartCoroutine(ShowDamageText(playerDamageText, damage, Color.red));
            playerAnimator.SetTrigger("Hurt");
            enemyGameObject.GetComponent<Animator>().SetTrigger("Attack");
            StartCoroutine(ShowComboText(comboText, playedCard));
        }

        var dead = false;
        if (player.currentHP <= 0)
        {
            player.currentHP = 0;
            playerGameObject.GetComponentInChildren<ParticleSystem>().Play();
            turnText.text = "YOU LOSE !";
            turnTextAnimator.SetTrigger("Show");
            dead = true;
        }

        if (dead)
        {
            Invoke(nameof(HidePlayer), .3f);
            Invoke(nameof(ReturnToExploration), delayBeforeReturnToExploration);
        }
        else
        {
            CombatContext.Instance.NextTurn();
            Invoke(nameof(StartTurn), delayBetweenTurns);
        }
    }

    private IEnumerator ShowComboText(TextMeshProUGUI comboText, CardData cardPlayed)
    {
        var ctx = CombatContext.Instance;
        string name;
        if (ctx.GetTurnState() == TurnState.Player)
        {
            name = ctx.playerData.ingameName;
        }
        else
        {
            name = ctx.enemyData.enemyName;
        }

        comboText.text = $"{name} played {cardPlayed.cardName}";
        Color kanjiColor;
        switch (cardPlayed.element)
        {
            case CardElement.Fire:
                ColorUtility.TryParseHtmlString("#C4232F", out kanjiColor);
                break;
            case CardElement.Water:
                ColorUtility.TryParseHtmlString("#0069AA", out kanjiColor);
                break;
            case CardElement.Wood:
                ColorUtility.TryParseHtmlString("#32984A", out kanjiColor);
                break;
            case CardElement.Normal:
                ColorUtility.TryParseHtmlString("#adadad", out kanjiColor);
                break;
            default:
                kanjiColor = Color.white;
                break;
        }
        comboText.color = kanjiColor;
        comboDisplayGameObject.SetActive(true);
        yield return new WaitForSeconds(playedComboNameDuration);
        comboDisplayGameObject.SetActive(false);
    }

    public void MakeDmg()
    {
        endTurnButton.SetActive(false);
        var enemy = CombatContext.Instance.enemyData;
        var playerData = CombatContext.Instance.playerData;

        var rawCombo = CombatContext.Instance.GetCombo();
        // Process combos
        var combo = ComboManager.Instance != null ? ComboManager.Instance.ProcessCombos(rawCombo) : rawCombo;
        Debug.Log($"Raw combo: {string.Join(", ", rawCombo.Select(c => c.cardName))}");
        Debug.Log($"Combo: {string.Join(", ", combo.Select(c => c.cardName))}");
        var cardPlayed = combo[0];
        StartCoroutine(ShowComboText(comboText, cardPlayed));

        int dmg = cardPlayed.attack;
        int heal = cardPlayed.health;

        playerData.currentHP += heal;

        if (playerData.currentHP > playerData.maxHP)
        {
            playerData.currentHP = playerData.maxHP;
        }

        bool isProjectile = false;

        if (dmg > 0 || heal > 0)
        {
            foreach (var card in combo)
            {
                if (card == null) continue;

                if (card.animationType == SpellAnimationType.AoEOnTarget)
                {
                    Debug.Log("AoEOnTarget");
                    var animInstance = Instantiate(card.animationPrefab, enemyGameObject.transform.position, Quaternion.identity);
                    Destroy(animInstance, card.animationDuration);
                }
                else if (card.animationType == SpellAnimationType.AoEOnCaster)
                {
                    Debug.Log("AoEOnCaster");
                    var animInstance = Instantiate(card.animationPrefab, playerGameObject.transform.position, Quaternion.identity);
                    Destroy(animInstance, card.animationDuration);
                }
                else if (card.animationType == SpellAnimationType.CasterToTargetProjectile)
                {
                    Debug.Log("CasterToTargetProjectile");
                    isProjectile = true;
                    var playerPos = playerGameObject.transform.position;
                    playerPos.x += .25f;
                    playerPos.y += .25f;
                    var animInstance = Instantiate(card.animationPrefab, playerPos, Quaternion.identity);
                    animInstance.GetComponent<Rigidbody2D>().linearVelocity = (enemyGameObject.transform.position - playerPos).normalized * 10;

                    var proj = animInstance.AddComponent<ProjectileBehavior>();
                    proj.Initialize(dmg, enemyGameObject, this);
                }
            }
        }

        if (heal > 0)
        {
            enemyHpEffect.Play();
            playerHpAnimator.SetTrigger("Heal");
            // Afficher les dégâts
            StartCoroutine(ShowDamageText(playerDamageText, 0 - heal, Color.green));
        }

        foreach (var indicator in GameObject.FindGameObjectsWithTag("ComboIndicator"))
        {
            indicator.SetActive(false);
        }

        comboIndicator.SetActive(false);

        if (!isProjectile)
        {
            ResolveDamage(dmg);
        }
    }

    public void OnProjectileHit(int damage)
    {
        ResolveDamage(damage);
    }

    private void ResolveDamage(int dmg)
    {
        var enemy = CombatContext.Instance.enemyData;
        enemy.currentHP -= dmg;

        if (dmg > 0)
        {
            enemyHpEffect.Play();
            enemyHpAnimator.SetTrigger("Hit");
            enemyGameObject.GetComponent<Animator>().SetTrigger("Hurt");
            // Afficher les dégâts
            StartCoroutine(ShowDamageText(enemyDamageText, dmg, Color.red));
        }

        var won = false;
        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            var particles = enemyGameObject.GetComponentInChildren<ParticleSystem>();
            if (particles != null) { particles.Play(); }
            turnText.text = "YOU WIN !";
            turnTextAnimator.SetTrigger("Show");
            won = true;
        }

        if (won)
        {
            Invoke(nameof(HideEnemy), .3f);
            Invoke(nameof(ReturnToExploration), delayBeforeReturnToExploration);
        }
        else
        {
            CombatContext.Instance.NextTurn();
            Invoke(nameof(StartTurn), delayBetweenTurns);
        }
    }

    private void HideEnemy()
    {
        enemyGameObject.SetActive(false);
    }

    private void HidePlayer()
    {
        playerGameObject.SetActive(false);
    }

    public void ReturnToExploration()
    {
        sceneTransitionManager.FadeToScene(CombatContext.Instance.returnSceneName);
    }

    public void StartTurn()
    {
        var ctx = CombatContext.Instance;
        if (ctx.GetTurnState() == TurnState.Player)
        {
            turnTextAnimator.SetTrigger("Show");
            turnText.text = "YOUR TURN !";
            handManager.Draw();
        }
        else
        {
            turnTextAnimator.SetTrigger("Show");
            turnText.text = "ENEMY TURN !";
            handManager.Draw();
            Invoke(nameof(TakeDmg), delayBeforeEnemyAttack);
        }
    }

    private System.Collections.IEnumerator ShowDamageText(TextMeshProUGUI damageText, int damage, Color color)
    {
        // Configurer le texte
        if (damage > 0)
        {
            damageText.text = $"-{damage}";
        }
        else
        {
            damageText.text = $"+{Math.Abs(damage)}";
        }

        damageText.color = color;
        damageText.gameObject.SetActive(true);

        // Sauvegarder la position initiale
        Vector3 startPos = damageText.rectTransform.anchoredPosition;
        float duration = 1.5f; // Durée totale de l'animation
        float elapsedTime = 0f;

        // Animation: fade in rapide, monter, puis fade out
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Déplacer vers le haut
            damageText.rectTransform.anchoredPosition = startPos + Vector3.up * (progress * 50f);

            // Fade out progressif (commence après 30% de l'animation)
            if (progress > 0.3f)
            {
                float fadeProgress = (progress - 0.3f) / 0.7f;
                Color c = damageText.color;
                c.a = 1f - fadeProgress;
                damageText.color = c;
            }

            yield return null;
        }

        // Désactiver et réinitialiser
        damageText.gameObject.SetActive(false);
        damageText.rectTransform.anchoredPosition = startPos;
        Color resetColor = damageText.color;
        resetColor.a = 1f;
        damageText.color = resetColor;
    }
}
