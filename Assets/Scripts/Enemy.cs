public class Enemy : CharacterMotor
{
    public EnemyInput enemyInput;
    
    public override CharacterInput CharacterInput => enemyInput;
}