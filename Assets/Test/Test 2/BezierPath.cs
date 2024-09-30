using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierPath : MonoBehaviour
{
    [SerializeField] private Transform[] _paths;
    [SerializeField] private float _travelSpeed;
    [SerializeField] private Transform _zombie;
    [SerializeField] private Image _progressionBar;
    private float progress;
    [SerializeField] private Animator _anim;
    private Coroutine currentCoroutine;

    void Update()
    {
        _anim = GetComponent<Animator>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(MoveAlongThePath());
        }
    }

    private IEnumerator MoveAlongThePath()
    {
        progress = 0;
        _anim.SetFloat("Move", 1);
        while (progress < 1)
        {
            Vector3 oldPos = _zombie.position;
            // progress += _travelSpeed * Time.deltaTime;
            progress = ComputeProgressByTravelDistance(_travelSpeed * Time.deltaTime);
            _progressionBar.fillAmount = progress;
            Vector3 newPosition = Mathf.Pow(1 - progress, 3) * _paths[0].position
                                  + 3 * Mathf.Pow(1 - progress, 2) * progress * _paths[1].position
                                  + 3 * Mathf.Pow(1 - progress, 2) * Mathf.Pow(progress, 2) * _paths[2].position
                                  + Mathf.Pow(progress, 3) * _paths[3].position;
            _zombie.position = newPosition;
            _zombie.rotation = Quaternion.LookRotation(newPosition - oldPos);
            yield return new WaitForEndOfFrame();
        }
        _anim.SetFloat("Move", 0);
    }

    /// <summary>
    /// The Cubic Bezier curves formula is: B(t)=(1−t)^3 * A + 3*(1−t)^2 *t *B +3 *(1−t) *t^2 *C +t^3 *D.
    /// Derivative the formula => t^2(−3A+9B−9C+3D)+t(6A−12B+6C)+(−3A+3B)
    /// v1 = -3A + 9B - 9C + 3D
    /// v2 = 6A - 12B + 6C
    /// v3 = -3A + 3B
    /// convert distance travel into t progress on the curve
    /// t = t + distance/length(t^2 *v1 + t *v2 + v3)
    /// </summary>
    private float ComputeProgressByTravelDistance(float distance)
    {
        Vector3 v1 = -3f * _paths[0].position + 9f * _paths[1].position - 9f * _paths[2].position +
                     3f * _paths[3].position;
        Vector3 v2 = 6f * _paths[0].position - 12f * _paths[1].position + 6f * _paths[2].position;
        Vector3 v3 = -3f * _paths[0].position + 3f * _paths[1].position;

        return progress + distance / (Mathf.Pow(progress, 2) * v1 + progress * v2 + v3).magnitude;
    }
}