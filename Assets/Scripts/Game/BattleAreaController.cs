using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaController : MonoBehaviour
{
    private List<EnemyController> enemies_ = new List<EnemyController>();

    public void Register(PlayerController player)
    {
        foreach(var enemy in enemies_)
        {
            enemy.OnPlayerEntered(player);
        }
    }

    public void Register(EnemyController enemy)
    {
        enemies_.Add(enemy);
    }

    /// <summary>
    /// 最も近いの敵を返す
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public EnemyController GetNearestEnemy(Vector3 position)
    {
        EnemyController result = null;
        float max_square_distance = float.MaxValue;

        foreach(var enemy in enemies_)
        {
            if(enemy.IsDead == false)
            {
                float square_distance = Vector3.SqrMagnitude(position - enemy.transform.position);
                if (square_distance < max_square_distance)
                {
                    max_square_distance = square_distance;
                    result = enemy;
                }
            }
        }

        return result;
    }
}
