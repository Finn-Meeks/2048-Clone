using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject labelObject;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 200;
    [SerializeField] float stretchAmount;

    public int Value = 1024;
    public bool JustMerged = false;
    public bool scheduleDestruction = false;

    public Vector3 MovingToPosition;
    public float MovingToLabelAlpha = 1f;
    public Vector3 MovingToScale = new Vector3(1, 1, 0);
    public int NewValue = 0;
    public GameObject MovingToGameObject = null;

    private TextMeshProUGUI tmpro;

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        tmpro = labelObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        tmpro.text = Value.ToString();
        JustMerged = false;

        if (MovingToGameObject != null)
        {
            MovingToPosition = MovingToGameObject.transform.localPosition;
        }

        // animate position
        Vector3 tempPosition = transform.localPosition;
        tempPosition.x = transform.localPosition.x + (MovingToPosition.x - transform.localPosition.x) * Time.deltaTime * moveSpeed;
        tempPosition.y = transform.localPosition.y + (MovingToPosition.y - transform.localPosition.y) * Time.deltaTime * moveSpeed;
        Vector3 positionDiff = (tempPosition - transform.localPosition) / Time.deltaTime;
        transform.localPosition = tempPosition;

        // animate scale
        Vector3 tempScale = transform.localScale;
        tempScale.x = transform.localScale.x + (MovingToScale.x - transform.localScale.x) * Time.deltaTime * moveSpeed * 0.5f;
        tempScale.y = transform.localScale.y + (MovingToScale.y - transform.localScale.y) * Time.deltaTime * moveSpeed * 0.5f;
        // add squash and stretch
        tempScale.x += (Mathf.Abs(positionDiff.x) - 2 * Mathf.Abs(positionDiff.y)) * stretchAmount;
        tempScale.y += (Mathf.Abs(positionDiff.y) - 2 * Mathf.Abs(positionDiff.x)) * stretchAmount;
        // set scale
        transform.localScale = tempScale;

        float tempAlpha = tmpro.color.a;
        tempAlpha = tmpro.color.a + (MovingToLabelAlpha - tmpro.color.a) * Time.deltaTime * moveSpeed * 0.6f;
        tmpro.color = new Vector4(1, 1, 1, tempAlpha);

        if (tmpro.color.a <= 0.1)
        {
            MovingToLabelAlpha = 1f;
            Value = NewValue;
        }

        if (scheduleDestruction && Mathf.Abs(MovingToPosition.x - transform.localPosition.x) < 0.01 && Mathf.Abs(MovingToPosition.y - transform.localPosition.y) < 0.01)
        {
            Destroy(gameObject);
        }
    }
}
