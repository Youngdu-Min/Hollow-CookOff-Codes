using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon",menuName = "Weapon/Range Weapon")]
public class WeaponData : ScriptableObject
{
    public Sprite WeaponImage;
    public Sprite BulletImage;

    [SerializeField, Tooltip("손 위치")]
    private Vector2 hand1Pos,hand2Pos;
    [SerializeField, Tooltip("총구 위치")]
    private Vector2 muzzlePos;


    [Tooltip("자동")]
    public bool autofire;
    [Tooltip("동시발사시 각도")]
    public float damage;
    [Tooltip("초당 발사속도")]
    public float speed;
    [Tooltip("사거리")]
    public float reach;

    [Tooltip("동시발사갯수")]
    public int multishot;
    [SerializeField,Tooltip("동시발사시 각도")]
    private float multishotDegree;
    [SerializeField, Tooltip("탄퍼짐 각도")]
    private float bulletSpreadDegree;


}
