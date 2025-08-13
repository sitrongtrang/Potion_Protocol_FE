using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorTexture;
    [SerializeField] private GameObject _clickEffectPrefab;
    [SerializeField] private Canvas _cursorCanvas;

    public static CursorManager Instance { get; private set; }

    private int _poolSize = 10;
    private float _effectLifeTime = 0.5f;
    private Vector2 _cursorHotspot;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    private Texture2D _initialCursorTexture;
    private Vector2 _initialHotspot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        _cursorHotspot = new Vector2(_cursorTexture.width / 2, _cursorTexture.height / 2);
        _initialHotspot = _cursorHotspot;
        Cursor.SetCursor(_cursorTexture, _cursorHotspot, CursorMode.Auto);

        _initialCursorTexture = _cursorTexture;
        Cursor.SetCursor(_initialCursorTexture, _cursorHotspot, CursorMode.Auto);

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_clickEffectPrefab, _cursorCanvas.transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnEffect(Input.mousePosition);
        }
    }

    void SpawnEffect(Vector3 screenPosition)
    {
        if (_pool.Count > 0)
        {
            GameObject fx = _pool.Dequeue();
            fx.transform.position = screenPosition;
            fx.SetActive(true);
            StartCoroutine(DisableAfterTime(fx, _effectLifeTime));
        }
    }

    private IEnumerator DisableAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }

    public void SetCursorTexture(Texture2D newTexture)
    {
        Vector2 hotspot = new Vector2(newTexture.width / 2f, newTexture.height / 2f);
        Cursor.SetCursor(newTexture, hotspot, CursorMode.Auto);
    }

    public void ResetToInitialCursor()
    {
        Cursor.SetCursor(_initialCursorTexture, _initialHotspot, CursorMode.Auto);
    }
}
