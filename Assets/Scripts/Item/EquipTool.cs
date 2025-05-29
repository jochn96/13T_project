using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    [Header("Arrow")]
    public GameObject Item_ArrowPrefab;          // 화살 프리팹 (Rigidbody 포함)
    public Transform arrowSpawnPoint;            // 활에 붙은 화살 생성 위치
    public float arrowSpeed = 50f;               // 화살 속도

    private UIInventory inventory;

    private void Awake()
    {
        GameObject root = GameObject.Find("UI"); // 또는 루트 오브젝트 이름
        if (root != null)
        {
            inventory = root.GetComponentInChildren<UIInventory>(true);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (hit.collider.TryGetComponent(out Resource resource))
            {
                if (doesGatherResources)
                {
                    resource.Gather(hit.point, hit.normal);
                }
            }
        }
    }

   
}
