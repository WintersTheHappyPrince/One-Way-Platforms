using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    // 枚举类型，设置不同类型的单向平台
    public enum OneWayPlatforms { GoingUp, GoingDown, Both }
    public OneWayPlatforms type = OneWayPlatforms.Both;
    // 一个短暂的延迟，允许玩家再次与平台碰撞
    [SerializeField] private float delay = .5f;
    // 平台的碰撞器
    private Collider2D col;
    // 玩家引用
    private GameObject player;
    // 玩家上的碰撞器引用
    private Collider2D playerCollider;

    private void Start()
    {
        // 获取当前平台上的碰撞器引用
        col = GetComponent<Collider2D>();
        // 较不优化的方式寻找玩家
        // player = GameObject.FindGameObjectWithTag("Player");
        // 更优化的方式寻找玩家；需要某种只在场景中活跃玩家身上存在的脚本
        player = FindObjectOfType<Character>().gameObject;
        playerCollider = player.GetComponent<Collider2D>();
    }

    // 每次有碰撞箱与平台碰撞时，Unity事件会被调用一次
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查与平台碰撞的游戏对象是否为玩家
        if (collision.gameObject == player)
        {
            // 检查玩家是否不在平台上方，以便玩家可以在跳跃时站在平台上，然后检查平台是否允许玩家向上跳跃穿过它
            if (playerCollider.bounds.min.y < col.bounds.center.y && type != OneWayPlatforms.GoingDown)
            {
                // 将玩家设置为忽略平台碰撞器的游戏对象，以便玩家可以穿过
                Physics2D.IgnoreCollision(playerCollider, col, true);
                // 设置跳跃 passingThroughPlatform 布尔值为 true，以便在穿过平台时不会进入地面状态；你的解决方案中可能不需要这一行代码
                player.GetComponent<Jump>().passingThroughPlatform = true;
                // 运行协程，以允许玩家再次与平台碰撞
                StartCoroutine(StopIgnoring());
            }
        }
    }

    // 这个方法处理当玩家站在平台上时向下穿过单向平台
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 检查与平台碰撞的游戏对象是否为玩家
        if (collision.gameObject == player)
        {
            // 检查输入是否允许向下跳跃，并且玩家实际上在单向平台上方
            if (player.GetComponent<Jump>().downJumpPressed && playerCollider.bounds.min.y > col.bounds.center.y && type != OneWayPlatforms.GoingUp)
            {
                // 将玩家设置为忽略平台碰撞器的游戏对象，以便玩家可以穿过
                Physics2D.IgnoreCollision(playerCollider, col, true);
                // 设置跳跃 passingThroughPlatform 布尔值为 true，以便在穿过平台时不会进入地面状态
                player.GetComponent<Jump>().passingThroughPlatform = true;
                // 运行协程，以允许玩家再次与平台碰撞
                StartCoroutine(StopIgnoring());
            }
        }
    }

    // 协程，切换平台上的碰撞器，以允许玩家再次与其碰撞
    private IEnumerator StopIgnoring()
    {
        // 等待在脚本顶部设置的短暂延迟
        yield return new WaitForSeconds(delay);
        // 将玩家设置为应该与平台碰撞器碰撞的游戏对象，以便玩家可以再次站在其上
        Physics2D.IgnoreCollision(playerCollider, col, false);
        // 设置跳跃 passingThroughPlatform 布尔值为 false，以便玩家可以进入地面状态
        player.GetComponent<Jump>().passingThroughPlatform = false;
    }
}
