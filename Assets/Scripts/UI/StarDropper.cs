using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarDropper : MonoBehaviour
{
    [Header("Star Prefab")]
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private RectTransform parentContainer;

    [Header("Spawn Settings")]
    [SerializeField] private int starsToSpawn = 50;
    [SerializeField] private Vector2 sizeRange = new Vector2(50f, 50f);
    [SerializeField] private float minAngle = 60f, maxAngle = 120f;
    [SerializeField] private float minSpeed = 100, maxSpeed = 200;
    [SerializeField] private Vector2 gravity = new Vector2(0, -150);
    [SerializeField] private float duration = 5f;

    private Vector2 spawnAreaSize = new Vector2(200f, 100f);

    public void SpawnStars()
    {
        StartCoroutine(SpawnStarsRoutine());
    }

    private IEnumerator SpawnStarsRoutine()
    {
        for (int i = 0; i < starsToSpawn; i++)
        {
            SpawnOneStar();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnOneStar()
    {
        if (starPrefab == null || parentContainer == null)
        {
            Debug.LogWarning("StarPrefab or ParentContainer not assigned.");
            return;
        }

        var starGO = Instantiate(starPrefab, parentContainer);
        RectTransform rt = starGO.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Star prefab missing RectTransform component.");
            return;
        }

        float randomSize = Random.Range(sizeRange.x, sizeRange.y);
        rt.sizeDelta = new Vector2(randomSize, randomSize);

        Vector2 startPos = (transform as RectTransform)?.anchoredPosition ?? Vector2.zero;
        float offsetX = Random.Range(-spawnAreaSize.x * 0.7f, spawnAreaSize.x * 0.7f);
        float offsetY = Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f);
        Vector2 randomOffset = new Vector2(offsetX, offsetY);

        rt.anchoredPosition = startPos + randomOffset;

        float angleRad = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 initialVelocity = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * speed;

        StartCoroutine(UIMoveParabola(rt, initialVelocity));
    }

    private IEnumerator UIMoveParabola(RectTransform rt, Vector2 initialVelocity)
    {
        Vector2 startPos = rt.anchoredPosition;
        float t = 0f;

        while (t < duration)
        {
            Vector2 displacement = initialVelocity * t + 0.5f * gravity * t * t;
            rt.anchoredPosition = startPos + displacement;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(rt.gameObject);
    }
}
