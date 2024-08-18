
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class BreakableDoor : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public Sprite[] brokenSprites;

    public GameObject rigidbodyGameObject;

    [SortingLayer] public int brokenSortingLayer;

    public StudioEventEmitter break_sound;

    public ObjectSize requiredCatSize = ObjectSize.Big;

    public void Smash()
    {
        rigidbodyGameObject.SetActive(false);
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = brokenSprites[i];
            spriteRenderers[i].sortingLayerID = brokenSortingLayer;
            break_sound.Play();
        }
    }
}
