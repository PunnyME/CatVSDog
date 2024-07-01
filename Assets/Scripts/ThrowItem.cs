using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
    private Transform startPosition;
    private Transform destinationPosition;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private bool isPowerThrow;

    private float startX;
    private float destinationX;

    private float distance;
    private float nextX;
    private float baseY;
    private float height;

    private Player player;

    private bool isDoubleThrow;

    private float smallDamage;
    private float normalDamage;
    private float powerDamage;
    private float doubleDamage;

    public void Init(Transform startPosition, Transform destinationPosition, Player player, bool isDoubleThrow)
    {
        this.startPosition = startPosition;
        this.destinationPosition = destinationPosition;
        this.player = player;
        this.isDoubleThrow = isDoubleThrow;

        this.smallDamage = float.Parse(GameManager.Instance.GameData._GameDataList.Data[4].Damage);
        this.normalDamage = float.Parse(GameManager.Instance.GameData._GameDataList.Data[5].Damage);
        this.powerDamage = float.Parse(GameManager.Instance.GameData._GameDataList.Data[6].Damage);
        this.doubleDamage = float.Parse(GameManager.Instance.GameData._GameDataList.Data[7].Damage);
    }
    void Update()
    {
        startX = startPosition.position.x;
        destinationX = destinationPosition.position.x;

        distance = destinationX - startX;
        nextX = Mathf.MoveTowards(transform.position.x, destinationX, speed * Time.deltaTime);
        baseY = Mathf.Lerp(startPosition.position.y, destinationPosition.position.y, (nextX - startX) / distance);
        height = 3 * (nextX - startX) * (nextX - destinationX) / (-0.25f * distance * distance);

        Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
        
        transform.position = movePosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter enemy = collision.gameObject.GetComponentInParent<PlayerCharacter>();

        if (enemy != null)
        {
            if (enemy.Player != player)
            {
                if(isPowerThrow)
                {
                    enemy.DealDamage(powerDamage);
                    enemy.SetAnimation("Moody UnFriendly");
                    if (!isDoubleThrow)
                    {
                        GameManager.Instance.TurnEnd();
                    }
                    Destroy(this.gameObject);
                }
                else
                {
                    if (collision.gameObject.CompareTag("Head"))
                    {
                        if (isDoubleThrow)
                        {
                            enemy.DealDamage(doubleDamage);

                        }
                        else
                        {
                            enemy.DealDamage(normalDamage);
                        }
                        enemy.SetAnimation("Moody UnFriendly");
                        if (!isDoubleThrow)
                        {
                            GameManager.Instance.TurnEnd();
                        }
                        Destroy(this.gameObject);
                    }
                    else if (collision.gameObject.CompareTag("Body"))
                    {
                        if (isDoubleThrow)
                        {
                            enemy.DealDamage(doubleDamage);

                        }
                        else
                        {
                            enemy.DealDamage(smallDamage);
                        }
                        enemy.SetAnimation("Moody Friendly");
                        if (!isDoubleThrow)
                        {
                            GameManager.Instance.TurnEnd();
                        }
                        Destroy(this.gameObject);
                    }
                }
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Border"))
            {
                GameManager.Instance.MissAnimation(player);
                if (!isDoubleThrow)
                {
                    GameManager.Instance.TurnEnd();
                }
                Destroy(this.gameObject);
            }
        }
    }
}
