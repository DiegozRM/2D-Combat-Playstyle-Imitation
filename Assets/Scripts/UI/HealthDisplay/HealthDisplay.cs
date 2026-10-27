using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridLayoutGroup))]
public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    private readonly List<Image> heartImages = new();

    private bool hasStarted;

    private void Awake()
    {
        if (health == null ||
            fullHeartSprite == null ||
            emptyHeartSprite == null)
        {
            Debug.LogError(
                "HealthDisplay requires a Health reference, " +
                "a full heart sprite and an empty heart sprite.",
                this);

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (health == null)
        {
            return;
        }

        health.HealthChanged += RefreshDisplay;

        if (hasStarted)
        {
            RefreshDisplay(health.CurrentHealth, health.MaxHealth);
        }
    }

    private void Start()
    {
        hasStarted = true;

        RefreshDisplay(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.HealthChanged -= RefreshDisplay;
        }
    }

    private void RefreshDisplay(int currentHealth, int maxHealth)
    {
        UpdateHeartCount(maxHealth);

        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].sprite = i < currentHealth
                ? fullHeartSprite
                : emptyHeartSprite;
        }
    }

    private void UpdateHeartCount(int maxHealth)
    {
        while (heartImages.Count < maxHealth)
        {
            heartImages.Add(CreateHeartImage());
        }

        while (heartImages.Count > maxHealth)
        {
            int lastIndex = heartImages.Count - 1;
            Image lastHeart = heartImages[lastIndex];

            heartImages.RemoveAt(lastIndex);

            lastHeart.gameObject.SetActive(false);
            Destroy(lastHeart.gameObject);
        }
    }

    private Image CreateHeartImage()
    {
        GameObject heartObject = new GameObject(
            $"HealthIcon_{heartImages.Count + 1}",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        heartObject.transform.SetParent(transform, false);

        Image heartImage = heartObject.GetComponent<Image>();

        heartImage.type = Image.Type.Simple;
        heartImage.preserveAspect = true;
        heartImage.raycastTarget = false;
        heartImage.color = Color.white;

        return heartImage;
    }
}