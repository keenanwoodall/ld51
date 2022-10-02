using UnityEngine;

public class Enemy : Character
{
    public EnemyInput enemyInput;
    public override CharacterInput CharacterInput => enemyInput;
}

public class EnemyInput : CharacterInput
{
}