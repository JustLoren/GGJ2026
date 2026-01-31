using UnityEngine;

public class ObjectJiggler : MonoBehaviour
{
    private Vector3 _baseLocalPosition;
    [SerializeField] private float minJiggleAmount = 0.04f;
    [SerializeField] private float maxJiggleAmount = 0.09f;
    [SerializeField] private float jiggleAmount = 0.05f;
    [SerializeField] private float jiggleChangeRate = .02f;

    private void Awake()
    {
        _baseLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        JiggleXY();

        if (Random.value < jiggleChangeRate)
            jiggleAmount = Random.Range(minJiggleAmount, maxJiggleAmount);
    }

    private void JiggleXY()
    {
        float x = Random.Range(-jiggleAmount, jiggleAmount);
        float y = Random.Range(-jiggleAmount, jiggleAmount);

        transform.localPosition = _baseLocalPosition + new Vector3(x, y, 0f);
    }

}
