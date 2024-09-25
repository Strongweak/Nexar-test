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

    private Coroutine currentCoroutine;

    void Update()
    {
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
        while (progress < 1)
        {
            Vector3 oldPos = _zombie.position;
            progress += _travelSpeed * Time.deltaTime;
            _progressionBar.fillAmount = progress;
            //B(t)=(1−t)^3 * p1 + 3*(1−t)^2 *t *p2 +3 *(1−t) *t^2 *p3 +t^3 *p4.
            Vector3 newPosition = Mathf.Pow(1 - progress, 3) * _paths[0].position
                                  + 3 * Mathf.Pow(1 - progress, 2) * progress * _paths[1].position
                                  + 3 * Mathf.Pow(1 - progress, 2) * Mathf.Pow(progress,2) * _paths[2].position
                                  + Mathf.Pow(progress, 3) * _paths[3].position;
            _zombie.position = newPosition;
            _zombie.rotation = Quaternion.LookRotation(newPosition - oldPos);
            yield return new WaitForEndOfFrame();
        }
    }
}