public class EnemyMotor : CharacterMotor
{
    public EnemyControl enemyControl;
    
    public override CharacterControl CharacterInput => enemyControl;
}