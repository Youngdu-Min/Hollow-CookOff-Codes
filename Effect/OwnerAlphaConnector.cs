using MoreMountains.CorgiEngine;
using UnityEngine;

public class OwnerAlphaConnector : MonoBehaviour
{
    private Weapon weapon;
    private SpriteRenderer[] weaponSprites;
    private SpriteRenderer ownerSprite;

    void Start()
    {
        weapon = GetComponent<Weapon>();
        weaponSprites = weapon.GetComponentsInChildren<SpriteRenderer>();
        ownerSprite = weapon.Owner.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponSprites.ForEach(x => x.color = new Color(1, 1, 1, ownerSprite.color.a));
    }
}
