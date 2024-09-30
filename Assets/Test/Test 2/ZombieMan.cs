using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class ZombieMan : MonoBehaviour
{
    [SerializeField] private float _maxAngle = 60f;

    [SerializeField] private float _detectRange = 6f;
    [SerializeField] private LayerMask _detectLayer;

    [SerializeField] private Transform _headbone;
    [SerializeField] private Vector3 _headBoneTargetAngleOffset;
    [SerializeField] private Vector3 _headLookDefaultOffset;
    [SerializeField] private Transform _headTarget;
    [SerializeField] private Transform _rootRig;

    private Vector3 defaultLook;
    private Vector3 _lerpTarget;
    [SerializeField ]bool detected = false;

    private void OnEnable()
    {
        defaultLook =  (transform.forward + _headLookDefaultOffset).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRange, _detectLayer);
        //sort by distance
        detected = false;
        System.Array.Sort(hitColliders,
            (x, y) => (x.transform.position - transform.position).magnitude.CompareTo(
                (y.transform.position - transform.position).magnitude));
        if (hitColliders.Length != 0)
        {
            foreach (var t in hitColliders)
            {
                Vector3 dirToTar = (t.transform.position - transform.position).normalized;
                //if the target is inside field of view
                if (Vector3.Angle(transform.forward, dirToTar) < _maxAngle / 2f)
                {
                    detected = true;
                    _lerpTarget = t.transform.position;
                    break;
                }
            }
        }
    }

    private void LateUpdate()
    {
        //Override animation by update for the last 
        if (detected)
        {
            _headTarget.position = Vector3.Lerp(_headTarget.position, _lerpTarget,10f * Time.deltaTime);
        }
        else
        {
            _headTarget.localPosition = Vector3.Lerp(_headTarget.localPosition, defaultLook,10f * Time.deltaTime);
        }
        Vector3 dir = (_headTarget.transform.position - _headbone.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir) * Quaternion.Euler(_headBoneTargetAngleOffset);
        _headbone.rotation = rot;
    }

    private void OnDrawGizmos()
    {
        if (_detectRange > 0.1f) Gizmos.DrawWireSphere(transform.position, _detectRange);
    }
}