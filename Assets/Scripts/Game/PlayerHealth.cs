using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;
    private int _health;
    private int Health { get { return _health; } set { _health = value; _healthBar.OnHealthChanged(_health); } }

    private UIPlayerHealthBar _healthBar;
    private PhotonView _photonView;

    public void TakeDamage(int damage)
    {
        _photonView.RPC("RemoteDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    private void RemoteDamage(int damage)
    {
        if (damage <= 0)
            return;

        Health -= damage;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        _healthBar = GetComponentInChildren<UIPlayerHealthBar>();
        _healthBar.SetMaxHealth(_maxHealth);

        Health = _maxHealth;
    }
}
