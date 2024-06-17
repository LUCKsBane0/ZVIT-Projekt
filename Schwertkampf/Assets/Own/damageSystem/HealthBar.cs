using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private float _maxValue;
    private float _value;
    private Coroutine _coroutine;

    [SerializeField] private Image BottomBar;
    [SerializeField] private Image TopBar;
    [SerializeField] private float AnimationSpeed = 1;


    public void Initialize(float maxValue, float value)
    {
        _maxValue = maxValue;
        _value = value;
    }

    public void Change(float delta)
    {
        _value = Mathf.Clamp(_value + delta, 0, _maxValue);
        if(_coroutine != null ) {
            StopCoroutine(_coroutine);
                }
        _coroutine = StartCoroutine(ChangeBars(delta));
    }

    private IEnumerator ChangeBars(float delta)
    {
        var directChangeBar = delta >= 0 ? BottomBar : TopBar;
        var animateChangeBar = delta >= 0 ? TopBar : BottomBar;

        var targetValue = _value / _maxValue;

        directChangeBar.fillAmount = targetValue;

        while(Mathf.Abs(animateChangeBar.fillAmount - targetValue) > 0.01f)
        {
            animateChangeBar.fillAmount = Mathf.MoveTowards(animateChangeBar.fillAmount, targetValue, Time.deltaTime * AnimationSpeed);
            yield return null;
        }
        animateChangeBar.fillAmount = targetValue;
    }


    public float GetCurrentValue()
    {
        return _value;
    }
}

