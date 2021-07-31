using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private int startHealth = 20;
    public int currentEnemyHealth;

    private void OnEnable() 
    {
        currentEnemyHealth = startHealth;
    }

  public void TakeDamage(int damage)
  {
      currentEnemyHealth = currentEnemyHealth - damage;
      if(currentEnemyHealth <= 0)
      {
          Debug.Log("dead : " + currentEnemyHealth);
          Destroy(this.gameObject);
      }
  }

}
