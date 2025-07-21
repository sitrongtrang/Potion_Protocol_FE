using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class StarAnim : MonoBehaviour
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
    private readonly List<Vector2> _targetPositions = new List<Vector2>();

    private void Awake()
    {
        // Instantiate tất cả sao và disable chúng ngay
        for (int i = 0; i < _maxStars; i++)
        {
            GameObject go = Instantiate(_starImagePrefab, _container);
            go.SetActive(false);
            _starImages.Add(go.GetComponent<Image>());
        }

        // Ép LayoutGroup (nếu có) tính xong, rồi lưu vị trí đích
        LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
        foreach (var img in _starImages)
            _targetPositions.Add(img.rectTransform.anchoredPosition);
    }

    private void Start()
    {
        int starsCount = GameManager.Instance != null
                         ? GameManager.Instance.Star
                         : 0;

        // Gán sprite trước khi animate
        for (int i = 0; i < _starImages.Count; i++)
        {
            _starImages[i].sprite = (i < starsCount)
                                     ? _brightStarSprite
                                     : _darkStarSprite;
        }

        StartCoroutine(AnimateStarsSequentially(starsCount));
    }

    private IEnumerator AnimateStarsSequentially(int starsCount)
    {
        for (int i = 0; i < _starImages.Count; i++)
        {
            Image img = _starImages[i];
            RectTransform rt = img.rectTransform;
            GameObject go = img.gameObject;

            // Enable star trước khi hiển thị
            go.SetActive(true);

            // Đặt đúng vị trí đích
            rt.anchoredPosition = _targetPositions[i];

            if (i < starsCount)
            {
                rt.localScale = Vector3.one * 6f;
                yield return rt
                    .DOScale(Vector3.one * 2f, 0.5f)
                    .SetEase(Ease.OutBack)
                    .WaitForCompletion();
            }
            else
            {
                rt.localScale = Vector3.one * 2f;
            }

            yield return new WaitForSeconds(0.8f);
        }
    }
}
