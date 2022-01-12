using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    [SerializeField] protected AnimationCurve _animCurve;
    private Material _material;
    private Color _originalColor;

    public abstract bool CheckCondition();

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _originalColor = _material.color;
    }

    protected void StopPingPongColor()
    {
        StopAllCoroutines();
        _material.color = _originalColor;
    }

    public void Mistake()
    {
        StartCoroutine(PingPongColor());
    }
    
    private IEnumerator PingPongColor()
    {
        float time = 0f;
        while (time < 10f)
        {
            for (float i = 0f; i <= 1f; i += 0.02f)
            {
                _material.color = new Color(
                    Mathf.Lerp(_originalColor.r, 1, _animCurve.Evaluate(i)), 
                    Mathf.Lerp(_originalColor.g, 0, _animCurve.Evaluate(i)), 
                    Mathf.Lerp(_originalColor.b, 0, _animCurve.Evaluate(i)), 
                    1f);
                time += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }

        _material.color = _originalColor;
    }
}
