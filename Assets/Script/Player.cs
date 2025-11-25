using UnityEngine;

public class Player : MonoBehaviour
{
    public string ingameName;
    public int level = 1;
    public int maxHP = 10;
    public int currentHP = 10;
    public int attack = 2;
    public int defense = 0;
    public int experience = 0;
    public int initiative = 0;
    public int experienceToNextLevel = 10;
    void Start()
    {
        var ctx = CombatContext.Instance;
        if (ctx.playerReturnPosition != Vector3.zero)
        {
            transform.position = ctx.playerReturnPosition;
            ctx.playerReturnPosition = Vector3.zero;

            if (ctx.enemyDefeated)
            {
                experience += 10;
                ctx.Clear();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
