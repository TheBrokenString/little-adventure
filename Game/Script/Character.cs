using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 5f;
    private Vector3 _movementvelocity;//ˮƽ�ٶ�
    private Playerinput _playerInput;
    private float _vertivalvelocity;//��ֱ�ٶ�
    public float Gravity = -9.8f;
    private Animator _animator;

    public int Coin;
    //�Ƿ����
    public bool IsPlayer = true;
    public bool IsBoss = true;
    private UnityEngine.AI.NavMeshAgent _navMesAgent;
    private Transform TargetPlayer;

    //Health
    private Health _health;

    //Damage Caster
    private DamageCaster _damageCaster;

    
    private float attackStartTime;
    public float AttackSlideDuration = 0.4f; //�������й���ʱ��
    public float AttackSlideSpeed = 0.06f;
    
    public Vector3 impactOnCharacter;//����player

    public bool IsInvincible;//�Ƿ��޵�
    public float invincibleDuration=2f; //�޵г���ʱ��
                 
    public float attackAnimationDuration;//��������
    public float SlideSpeed = 9f;
     

    //��ɫ״̬
    public enum CharacterState { Normal,Attacking,Dead,BeingHit,Slide,Spawn}
    public CharacterState CurrentState;//��ǰ״̬

    public float SpawnDuration = 3f;//���������ٶ�
    private float currentSpwanTime; 

    //���˲�������
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    //������
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

    //����player�ƶ�
    private void CalculatePlayerMovement()
    {
        if(_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking); //����������ɫ���빥��״̬
            return;
        }
        else if(_playerInput.SpaceKeyDown&&_cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);//���¿ո񷭹�
            return;
        }

        _movementvelocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.verticalInput); //���ô�ֱ����Ϊ0��ֹ��ɫ��Y�������ƶ�
        _movementvelocity.Normalize(); //�淶���ٶȣ���Ȼ��Ī���������??
        _movementvelocity = Quaternion.Euler(0, -45, 0) * _movementvelocity; //ƥ��������?�ٶ�
        _animator.SetFloat("Speed", _movementvelocity.magnitude);
        _movementvelocity *= MoveSpeed * MoveSpeed * Time.deltaTime; //ȷ����ÿ֡�ϵ��ƶ��ٶ��Ǻ㶨��
        
        if (_movementvelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementvelocity);//�����������
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

            SwitchStateTo(CharacterState.Attacking); //����׷��player���������㹻�Ϳ��Թ���player
        }
    }

    //�̶�����
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
                        _movementvelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);//combo����ʱ��ǰ����һС�ξ���
                    }
                    if(_playerInput.MouseButtonDown&&_cc.isGrounded)
                    {
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;//��ȡ��ǰ������������������
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
            _movementvelocity = impactOnCharacter * Time.deltaTime; //����
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);//

        if (IsPlayer)
        {
            if (_cc.isGrounded == false)//

                _vertivalvelocity = Gravity;//��ֹplayer��Ϊ����������

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

    //�л�״̬
    public void SwitchStateTo(CharacterState newState )
    {

        if (IsPlayer)
        {
            //�������
            _playerInput.ClearCache();


        }

        //�˳�״̬
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

        //����״̬
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(!IsPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);//��ǰ������
                    transform.rotation = newRotation; 

                }
                _animator.SetTrigger("Attack");

                if (IsPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();//����곯����й���
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());

                if (!IsPlayer)
                {
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer = 0;//���������������������
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
            Debug.Log("Player��ǰ״̬" + CurrentState);
        }
        else
            Debug.Log("Enemy��ǰ״̬" + CurrentState);
    }



    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        
        SwitchStateTo(CharacterState.Normal);//����������������������¼�����������������
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
            ApplyImpact(attackerPos, 10f);//�����
        }else
        {
            ApplyImpact(attackerPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible()//�ӳ�2����ȡ����ɫ�޵�
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

    IEnumerator MateriaBlink()//Enmey������ʱ������˸
    {
        _materialPropertyBlock.SetFloat("_blink", 0.5f);//
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);//
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()//Enemy���������ܽ�
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
    //������
    public void DropItem()
    {
        if(ItemToDrop != null)
        {
             Instantiate(ItemToDrop, transform.position, Quaternion.identity);//�ڶ�������������Ѫƿ
            //Destroy(drop_Item.gameObject, 60f);
        }
    }
    //ʰȡ������
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
    
    private void AddHealth(int health)//��Ѫ
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHealVFX();

    }
    private void AddCoin(int coin)//��Ӳ��s
    {
        Coin += coin;
    }

    public void RotateToTarget()//����ʱ�������
    {
        if (CurrentState!= CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }

    }

    //Enemy���ɲ��ʶ���
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//��������������ߵ�����λ��
        RaycastHit hitResult; //�洢���������
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }
    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//��������������ߵ�����λ��
        RaycastHit hitResult; //�洢���������
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos-transform.position,Vector3.up);
        }
    }
}
