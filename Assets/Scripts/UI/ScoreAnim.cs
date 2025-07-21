using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreAnim : MonoBehaviour
{
    [SerializeField] private TMP_Text digitPrefab;
    [SerializeField] private int maxDigits = 6;
    [SerializeField] private float digitFlipDuration = 0.3f;
    [SerializeField] private float delayBetweenDigits = 0.1f;
    [SerializeField] private Transform digitsContainer; // đặt UI LayoutGroup vào đây

    private void Start()
    {
        StartCoroutine(AnimateScore());
    }

    private IEnumerator AnimateScore()
    {
        // 1) Lấy điểm và format thành chuỗi có fixed length
        int score = GameManager.Instance != null
                    ? GameManager.Instance.Score
                    : 0;

        string scoreStr = score.ToString().PadLeft(maxDigits, '0');

        // 2) Xoá các con cũ (nếu có) rồi sinh lại đúng số lượng
        foreach (Transform child in digitsContainer)
            Destroy(child.gameObject);

        TMP_Text[] digits = new TMP_Text[maxDigits];
        for (int i = 0; i < maxDigits; i++)
        {
            // Để units ở cuối container, ta sinh tuần tự từ trái
            digits[i] = Instantiate(digitPrefab, digitsContainer);
            digits[i].text = "0";
            // Đặt pivot ở giữa đáy nếu muốn hiệu ứng lật mượt
            digits[i].rectTransform.pivot = new Vector2(0.5f, 0f);
            digits[i].rectTransform.localRotation = Quaternion.Euler(90, 0, 0); // bắt đầu úp xuống
            yield return null;
        }

        // 3) Animate từ phải (units) sang trái
        for (int idx = maxDigits - 1; idx >= 0; idx--)
        {
            int targetDigit = scoreStr[idx] - '0';
            yield return StartCoroutine(FlipDigit(digits[idx], targetDigit));
            yield return new WaitForSeconds(delayBetweenDigits);
        }
    }

    private IEnumerator FlipDigit(TMP_Text digitText, int target)
    {
        for (int val = 0; val <= target; val++)
        {
            digitText.text = val.ToString();
            float elapsed = 0f;
            digitText.rectTransform.localEulerAngles = new Vector3(90, 0, 0);

            while (elapsed < digitFlipDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / digitFlipDuration);
                float angle = Mathf.Lerp(90f, 0f, t);
                digitText.rectTransform.localEulerAngles = new Vector3(angle, 0, 0);
                yield return null;
            }
            digitText.rectTransform.localEulerAngles = Vector3.zero;

            if (val < target)
                yield return new WaitForSeconds(0.02f);
        }
    }
}
