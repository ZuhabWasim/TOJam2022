using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] Transform climax;
    [SerializeField] float swoopSpeed = 1.0f;
    [SerializeField] float returnSpeed = 1.0f;
    [SerializeField] bool _flyingRight;
    [SerializeField] float aggroCooldown = 1.0f;
    [SerializeField] AggroTrigger aggroTrigger;

    private bool _isAggroed;

    private Animator _animator;
    private Vector3 _spawnPos;
    private Vector3 _defaultStartPos;
    private Vector3 _defaultEndPos;
    private Vector3 _defaultClimaxPos;
    private bool _defaultFlyingRight;
    private SpriteRenderer _spriteRenderer;
    private float _count = 0.0f;

    private Health _health;

    private WeaponController _weaponController;

    private void Start()
    {
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        _defaultStartPos = start.position;
        _defaultEndPos = end.position;
        _defaultClimaxPos = climax.position;
        _defaultFlyingRight = _flyingRight;
        _animator = gameObject.GetComponentInChildren<Animator>();
        _health = gameObject.GetComponent<Health>();

        _weaponController = gameObject.GetComponent<WeaponController>();
        if(!_weaponController) Debug.LogWarning("No Weapon Controller on, this enemy will not damage the player" + name);

        if (_flyingRight)
        {
            _spawnPos = _defaultStartPos;
        }
        else
        {
            _spawnPos = _defaultEndPos;
        }
        gameObject.transform.position = _spawnPos;
        if (_health)
        {
            _health.Death += OnDeath;
        }
        if(!aggroTrigger){
            Debug.LogWarning("Please assign " + name + " an aggro trigger subobject, it will not function without one");
        }
        else{
            aggroTrigger.Aggro += SetAggro;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.flipX = _flyingRight;
        if (_isAggroed)
        {	
            Swoop();
        }
        else
        {
            Return();
        }
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }

    private void Return()
    {
        _animator.SetBool("isAggroed", false);

        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _spawnPos, returnSpeed * Time.deltaTime);
        _count = 0.0f;
        // When returning, flip the sprite in the other direction
        _flyingRight = !_defaultFlyingRight;
        if (Vector3.Distance(gameObject.transform.position, _spawnPos) < 0.2f)
        {
            _flyingRight = _defaultFlyingRight;
        }
    }

    private void Swoop()
    {
        Debug.DrawLine(_defaultStartPos, _defaultClimaxPos);
        Debug.DrawLine(_defaultClimaxPos, _defaultEndPos);
        _animator.SetBool("isAggroed", true);
		if (gameObject.transform.position == _defaultStartPos || gameObject.transform.position == _defaultEndPos){
			FindObjectOfType<SoundManager>().PlayBird();
		}
        if (_flyingRight)
        {
            if (_count < 1.0f)
            {
                _count += 1.0f * Time.deltaTime * swoopSpeed;
                Vector3 m1 = Vector3.Lerp(_defaultStartPos, _defaultClimaxPos, _count);
                Vector3 m2 = Vector3.Lerp(_defaultClimaxPos, _defaultEndPos, _count);
                gameObject.transform.position = Vector3.Lerp(m1, m2, _count);
                Debug.DrawLine(m1, m2, Color.green);
            }
            else
            {
                _flyingRight = false;
                _count = 0.0f;
            }
        }
        else
        {
            if (_count < 1.0f)
            {
                _count += 1.0f * Time.deltaTime * swoopSpeed;
                Vector3 m1 = Vector3.Lerp(_defaultEndPos, _defaultClimaxPos, _count);
                Vector3 m2 = Vector3.Lerp(_defaultClimaxPos, _defaultStartPos, _count);
                gameObject.transform.position = Vector3.Lerp(m1, m2, _count);
                Debug.DrawLine(m2, m1, Color.green);
            }
            else
            {
                _flyingRight = true;
                _count = 0.0f;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        _weaponController.Attack(); // Weapon controller will determine if anything can be hit
    }

    public async void SetAggro(bool aggro){
        if(!aggro) await System.Threading.Tasks.Task.Delay(Mathf.CeilToInt(aggroCooldown * 1000f)); // Just await the delay finishing
        _isAggroed = aggro;
    }
}
