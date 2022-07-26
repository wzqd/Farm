using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("组件")]
    [SerializeField] private FSM playerFsm;
    private Rigidbody2D rb;
        
    [Header("移动参数")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float horizontalDirection;
    [SerializeField] private float verticalDirection;
    private Vector2 moveVector;

    [Header("动画参数")]
    [SerializeField] private SpriteRenderer holdItemSpriteRenderer;
    private Animator[] animators; //0身体 1头发 2手臂
    //身体部位特殊动画
    [SerializeField]private AnimatorOverrideController Arm;
    [SerializeField]private AnimatorOverrideController Arm_Hold;
    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //得刚体
        animators = GetComponentsInChildren<Animator>();
        playerFsm = gameObject.AddComponent<FSM>(); //加状态机

        holdItemSpriteRenderer.enabled = false; //关闭举起的图片
        
        playerFsm.CurrentStateType = E_States.Player_Idle;
        
        InputMgr.Instance.SwitchAllButtons(true);
        
        //按键触发事件监听
        EventMgr.Instance.AddEventListener<KeyCode>("KeyIsPressed", keyPressed);
        EventMgr.Instance.AddEventListener<KeyCode>("KeyIsHeld", keyHeld);
        EventMgr.Instance.AddEventListener<KeyCode>("KeyIsReleased", keyReleased);

        //其他事件监听
        EventMgr.Instance.AddEventListener<ItemDetails>("PlayerLiftItem", Lift);
        EventMgr.Instance.AddEventListener("PlayerCancelLiftItem", CancelLift);
        
        EventMgr.Instance.AddEventListener<Vector3>("Teleport", Teleport);
    }
    

    void Start()
    {
        //状态事件监听
        EventMgr.Instance.AddEventListener("PlayerMove", Move);
        EventMgr.Instance.AddEventListener("PlayerIdle", Idle);

        
    }


    void Update()
    {
        horizontalDirection = Input.GetAxisRaw("Horizontal");
        verticalDirection = Input.GetAxisRaw("Vertical");
        if (horizontalDirection != 0 && verticalDirection != 0) //斜向移动补正
        {
            horizontalDirection *= 0.7f; //2* 0.7^2 = 1
            verticalDirection *= 0.7f;
        }
        
        moveVector = new Vector2(horizontalDirection, verticalDirection);

        if (moveVector == Vector2.zero) //检测站定状态
        {
            playerFsm.ChangeState(E_States.Player_Idle);
        }
    }

    #region 所有按键触发事件
    #endregion
    
    private void keyPressed(KeyCode key)
    {
        
    }
    private void keyHeld(KeyCode key)
    {
        if (key == InputMgr.Instance.KeySet["up"] || key == InputMgr.Instance.KeySet["down"] || key == InputMgr.Instance.KeySet["left"] || key == InputMgr.Instance.KeySet["right"] )
        {
            playerFsm.ChangeState(E_States.Player_Move);
        }
    }
    private void keyReleased(KeyCode key)
    {
        
    }


    #region 所有状态

    #endregion
    private void Move()
    {
        rb.MovePosition(rb.position + moveVector * (moveSpeed * Time.deltaTime)); //移动
        foreach(Animator anim in animators)
        {
            anim.SetFloat("InputX", horizontalDirection);
            anim.SetFloat("InputY", verticalDirection); //设置blender参数
            
            anim.Play("Move");
        }
    }

    private void Idle()
    {
        foreach(Animator anim in animators)
        {
            anim.Play("Idle");
        }
    }



    #region 其他事件
    #endregion
    
    private void Lift(ItemDetails itemDetails)
    {
        holdItemSpriteRenderer.sprite = itemDetails.itemSpriteInWorld;
        holdItemSpriteRenderer.enabled = true;

        animators[2].runtimeAnimatorController = Arm_Hold;
    }

    private void CancelLift()
    {
        holdItemSpriteRenderer.enabled = false;
        animators[2].runtimeAnimatorController = Arm;
    }

    private void Teleport(Vector3 destination)
    {
        this.transform.position = destination;
    }

    #region 所有碰撞触发
    #endregion
    private void OnTriggerEnter2D(Collider2D other)
    {
        EventMgr.Instance.EventTrigger("TriggerFadeIn",other.gameObject.name); //触发事件，让遮住的物体变淡
        EventMgr.Instance.EventTrigger("PickUpItems",other.gameObject); //触发事件，捡起道具
        EventMgr.Instance.EventTrigger("TriggerTeleport",other.gameObject.name); //触发事件，传送
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EventMgr.Instance.EventTrigger("TriggerFadeOut",other.gameObject.name); //触发事件，让变淡的物体还原
    }

}
