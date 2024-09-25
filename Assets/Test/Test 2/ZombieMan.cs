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

    [SerializeField] private Rig _headRig;
    [SerializeField] private Rig _bodyRig;
    [SerializeField] private Transform _headbone;
    [SerializeField] private Transform _headTarget;
    [SerializeField] private Transform _rootRig;

    private Vector3 _lerpTarget;
    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRange, _detectLayer);
        Debug.Log("Found: " + hitColliders.Length);
        bool detected = false;
        //sort by distance
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
                    _headRig.weight = 1;
                    _bodyRig.weight = 1;
                    _headTarget.transform.position = t.transform.position;
                    _lerpTarget = t.transform.position;
                    break;
                }
            }

            if (!detected)
            {
                _headRig.weight = 0;
                _bodyRig.weight = 0;
            }
        }
        else
        {
            _headRig.weight = 0;
            _bodyRig.weight = 0;
        }
        _headTarget.transform.position = Vector3.Lerp(_headTarget.transform.position, _lerpTarget, Time.deltaTime * 10f);

    }
    private void OnDrawGizmos()
    {
        if (_detectRange > 0.1f) Gizmos.DrawWireSphere(transform.position, _detectRange);
    }
}