using UnityEngine;

public class ObjectJiggler : MonoBehaviour
{
    private Vector3 _baseLocalPosition;
    [SerializeField] private float minJiggleAmount = 0.01f;
    [SerializeField] private float maxJiggleAmount = 0.11f;
    private float minZjiggle = 0;
    private float maxZjiggle = 0;

    private void Awake()
    {
        _baseLocalPosition = transform.localPosition;
        minZjiggle = transform.localPosition.z;
        maxZjiggle = transform.localPosition.z * 2f;
    }

    private void Update()
    {
        //jiggleAmount = Random.Range(minJiggleAmount, maxJiggleAmount);

        JiggleXY();
    }

    private void JiggleXY()
    {
        var realJiggleAmt = minJiggleAmount + ((maxJiggleAmount - minJiggleAmount) * TableCameraLook.CrazyAngle);
        float x = Random.Range(-realJiggleAmt, realJiggleAmt);
        float y = Random.Range(-realJiggleAmt, realJiggleAmt);
        float z = Random.Range(minZjiggle, maxZjiggle);

        transform.localPosition = _baseLocalPosition + new Vector3(x, y, z);
    }

}
