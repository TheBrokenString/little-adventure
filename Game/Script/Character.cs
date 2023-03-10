using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 5f;
    private Vector3 _movementvelocity;//水平速度
    private Playerinput _playerInput;
    private float _vertivalvelocity;//垂直速度
    public float Gravity = -9.8f;
    private Animator _animator;

    public int Coin;
    //是否玩家
    public bool IsPlayer = true;
    public bool IsBoss = true;
    private UnityEngine.AI.NavMeshAgent _navMesAgent;
    private Transform TargetPlayer;

    //Health
    private Health _health;

    //Damage Caster
    private DamageCaster _damageCaster;

    
    private float attackStartTime;
    public float AttackSlideDuration = 0.4f; //攻击滑行过度时间
    public float AttackSlideSpeed = 0.06f;
    
    public Vector3 impactOnCharacter;//击退player

    public bool IsInvincible;//是否无敌
    public float invincibleDuration=2f; //无敌持续时间
                 
    public float attackAnimationDuration;//攻击动画
    public float SlideSpeed = 9f;
     

    //角色状态
    public enum CharacterState { Normal,Attacking,Dead,BeingHit,Slide,Spawn}
    public CharacterState CurrentState;//当前状态

    public float SpawnDuration = 3f;//敌人生成速度
    private float currentSpwanTime; 

    //敌人材质属性
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    //掉落物
    public GameObject ItemToDrop;


    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _playerInput = GetComponent<Playerinput>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if (!IsPlayer)
        {
            _navMesAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMesAgent.speed = MoveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<Playerinput>();
        }
    }

    //计算player移动
    private void CalculatePlayerMovement()
    {
        if(_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking); //满足条件角色进入攻击状态
            return;
        }
        else if(_playerInput.SpaceKeyDown&&_cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);//按下空格翻滚
            return;
        }

        _movementvelocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.verticalInput); //锟斤拷锟矫达拷直锟斤拷锟斤拷为0锟斤拷止锟斤拷色锟斤拷Y锟斤拷锟斤拷锟斤拷锟狡讹拷
        _movementvelocity.Normalize(); //锟芥范锟斤拷锟劫度ｏ拷锟斤拷然锟斤拷莫锟斤拷锟斤拷锟斤拷锟斤拷锟??
        _movementvelocity = Quaternion.Euler(0, -45, 0) * _movementvelocity; //匹锟斤拷锟斤拷锟斤拷锟斤拷?锟劫讹拷
        _animator.SetFloat("Speed", _movementvelocity.magnitude);
        _movementvelocity *= MoveSpeed * MoveSpeed * Time.deltaTime; //确锟斤拷锟斤拷每帧锟较碉拷锟狡讹拷锟劫讹拷锟角恒定锟斤拷
        
        if (_movementvelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementvelocity);//攻击朝向玩家
        }
        _animator.SetBool("AirBorne", !_cc.isGrounded);

        //RotateToCursor();
    }

    private void CalculateEnemyMovement()
    {
        
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMesAgent.stoppingDistance)
        {
            _navMesAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {
            _navMesAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed",0f);

            SwitchStateTo(CharacterState.Attacking); //敌人追击player，当距离足够就可以攻击player
        }
    }

    //固定更新
    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (IsPlayer && GameObject.FindGameObjectWithTag("Player"))
                {
                    CalculatePlayerMovement();
                }
                else
                {
                    CalculateEnemyMovement();
                }
                break
                    ;
            case CharacterState.Attacking:
                if (IsPlayer)
                {
                    
                    
                    if(Time.time<attackStartTime+AttackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementvelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);//combo攻击时向前滑行一小段距离
                    }
                    if(_playerInput.MouseButtonDown&&_cc.isGrounded)
                    {
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;//锟斤拷取锟斤拷前锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if(currentClipName != "LittleAdventurerAndie_ATTACK_03"&&attackAnimationDuration>0.5f &&attackAnimationDuration<0.7f)
                        {
                            _playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);

                            //CalculatePlayerMovement();
                        }
                    }
                }
                break;

            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                _movementvelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                currentSpwanTime -= Time.deltaTime;
                if(currentSpwanTime<=0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if (impactOnCharacter.magnitude > 0.2f)
        {
            _movementvelocity = impactOnCharacter * Time.deltaTime; //击退
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);//

        if (IsPlayer)
        {
            if (_cc.isGrounded == false)//

                _vertivalvelocity = Gravity;//防止player因为重力飞起来

            else
                _vertivalvelocity = Gravity * 0.3f;
            _movementvelocity += _vertivalvelocity * Vector3.up * Time.deltaTime;


            _cc.Move(_movementvelocity);
            _movementvelocity = Vector3.zero;
        }
        else
        {
            if(CurrentState!=CharacterState.Normal)
            {
                _cc.Move(_movementvelocity);
                _movementvelocity = Vector3.zero;
            }
        }
        
    }

    //切换状态
    public void SwitchStateTo(CharacterState newState )
    {

        if (IsPlayer)
        {
            //清楚缓存
            _playerInput.ClearCache();


        }

        //退出状态
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                if (_damageCaster !=null)
                {
                    _damageCaster.DisableDamageCaster();
                }
                if (IsPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                IsInvincible = false;
                break;

                
        }

        //进入状态
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(!IsPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);//朝前方攻击
                    transform.rotation = newRotation; 

                }
                _animator.SetTrigger("Attack");

                if (IsPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();//按鼠标朝向进行攻击
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());

                if (!IsPlayer)
                {
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer = 0;//解决敌人死亡遗留的网格
                }

                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");

                if(IsPlayer)
                {
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                IsInvincible = true;
                currentSpwanTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }

        CurrentState = newState;

        if(IsPlayer)
        {
            Debug.Log("Player当前状态" + CurrentState);
        }
        else
            Debug.Log("Enemy当前状态" + CurrentState);
    }



    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        
        SwitchStateTo(CharacterState.Normal);//攻击动画结束后，启动这个事件，结束攻击动画。
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }


    public void ApplyDamage(int damage,Vector3 attackerPos =new Vector3())
    {
        if(IsInvincible)
        {
            return;
        }
        if(_health!=null)
        {
            _health.ApplayDamage(damage);

        }

        if(!IsPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MateriaBlink());
        if(IsPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            ApplyImpact(attackerPos, 10f);//向后推
        }else
        {
            ApplyImpact(attackerPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible()//延迟2秒钟取消角色无敌
    {
        yield return new WaitForSeconds(invincibleDuration);
        IsInvincible = false;
    }

    private void ApplyImpact(Vector3 attackerPos,float force)//
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;

    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }
    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MateriaBlink()//Enmey被攻击时材质闪烁
    {
        _materialPropertyBlock.SetFloat("_blink", 0.5f);//
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);//
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()//Enemy死亡材质溶解
    {
        yield return new WaitForSeconds(3);
        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHight_start = 20f;
        float dissolveHight_target = -10f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        DropItem();
        
    }
    //掉落物
    public void DropItem()
    {
        if(ItemToDrop != null)
        {
             Instantiate(ItemToDrop, transform.position, Quaternion.identity);//在对象死亡处生成血瓶
            //Destroy(drop_Item.gameObject, 60f);
        }
    }
    //拾取掉落物
    public void PickUpItem(PickUp Item)
    {
        switch (Item.Type)
        {
            case PickUp.PickUpType.Heal:
                
                AddHealth(Item.Value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(Item.Value);
                break;
        }
    }
    
    private void AddHealth(int health)//加血
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHealVFX();

    }
    private void AddCoin(int coin)//加硬币s
    {
        Coin += coin;
    }

    public void RotateToTarget()//生成时朝向玩家
    {
        if (CurrentState!= CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }

    }

    //Enemy生成材质动画
    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = SpawnDuration;
        float currentDissolveTime = 0;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissolveHight;
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentDissolveTime<dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target,currentDissolveTime/dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发射射线到光标点位置
        RaycastHit hitResult; //存储鼠标点击数据
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }
    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发射射线到光标点位置
        RaycastHit hitResult; //存储鼠标点击数据
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos-transform.position,Vector3.up);
        }
    }
}
