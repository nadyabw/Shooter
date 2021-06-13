using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables

    [Header("Shooting")] [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootDelayMin = 1f;
    [SerializeField] private float shootDelayMax = 2f;

    [Header("Health")] [SerializeField] private float healthMax;
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private Transform healthBarPosition;

    [Header("Animation")] [SerializeField] private Animator animator;

    private float shootDelay;

    private HealthBar healthBar;
    private float currentHealth;

    private Player player;

    public bool IsDied { get; private set; }

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
    }

    private void Start()
    {
        currentHealth = healthMax;
        healthBar = Instantiate(healthBarPrefab);
        healthBar.SetParentAndOffset(gameObject, healthBarPosition.localPosition);

        player = Player.Instance;
        shootDelay = Random.Range(shootDelayMin, shootDelayMax);

        StartCoroutine(UpdateShoot());
    }

    private void Update()
    {
        UpdateShootAim();
    }

    #endregion


    #region Event handlers

    private void HandlePlayerDeath()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Private methods

    private void UpdateShootAim()
    {
        if (!IsDied && !player.IsDied)
        {
            Vector3 dir = (player.transform.position - transform.position);
            transform.up = -(Vector2) dir;
        }
    }


    private void Shoot()
    {
        CreateBullet();
        PlayShootAnimation();
    }

    private void CreateBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
    }

    private IEnumerator UpdateShoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootDelay);

            Shoot();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null)
        {
            HandleDamage(dd.Damage);
        }
    }

    private void HandleDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateHealthState(currentHealth / healthMax);
    }

    private void Die()
    {
        StopAllCoroutines();
        IsDied = true;

        PlayDeathAnimation();

        Destroy(GetComponent<Collider2D>());
        Destroy(healthBar.gameObject);
    }

    private void PlayShootAnimation()
    {
        animator.SetTrigger(AnimationIdHelper.GetId(AnimationState.Shoot));
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger(AnimationIdHelper.GetId(AnimationState.Death));
    }

    #endregion
}