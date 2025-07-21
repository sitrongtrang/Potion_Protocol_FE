using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarsDisplay : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite _brightStarSprite;
    [SerializeField] private Sprite _darkStarSprite;

    [Header("References")]
    [SerializeField] private RectTransform _container;
    [SerializeField] private GameObject _starImagePrefab;

    [Header("Config")]
    [SerializeField] private int _maxStars = 3;

    private readonly List<Image> _starImages = new List<Image>();

    private void Awake()
    {
        for (int i = 0; i < _maxStars; i++)
        {
            GameObject go = Instantiate(_starImagePrefab, _container);
            Image img = go.GetComponent<Image>();
            _starImages.Add(img);
        }
    }

    private void Start()
    {
        RenderStars();
    }

    private void RenderStars()
    {
        int stars = GameManager.Instance != null
                    ? GameManager.Instance.Star
                    : 0;

        for (int i = 0; i < _starImages.Count; i++)
        {
            _starImages[i].sprite = (i < stars)
                ? _brightStarSprite
                : _darkStarSprite;
        }
    }
}