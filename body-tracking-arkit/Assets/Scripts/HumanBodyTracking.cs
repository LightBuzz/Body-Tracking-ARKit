using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTracking : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    private ARHumanBodyManager humanBodyManager;

    [SerializeField] private GameObject jointPrefab;

    [SerializeField] private GameObject lineRendererPrefab;

    private Dictionary<JointIndices3D, Transform> bodyJoints;

    private LineRenderer[] lineRenderers;
    private Transform[][] lineRendererTransforms;

    void OnEnable()
    {
        Debug.Assert(humanBodyManager != null, "human body manager is required");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        Debug.Assert(humanBodyManager != null, "human body manager is required");
        humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    private void InitialiseObjects(Transform arBodyT)
    {
        if (bodyJoints == null)
        {
            bodyJoints = new Dictionary<JointIndices3D, Transform>
            {
                { JointIndices3D.head_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.neck_1_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_arm_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_arm_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_forearm_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_forearm_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_hand_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_hand_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_upLeg_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_upLeg_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_leg_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_leg_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.left_foot_joint, GetNewJointPrefab(arBodyT) },
                { JointIndices3D.right_foot_joint, GetNewJointPrefab(arBodyT) }
            };

            // Create line renderers
            lineRenderers = new LineRenderer[]
            {
                Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // head neck
                Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // upper
                Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // lower
                Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // right
                Instantiate(lineRendererPrefab).GetComponent<LineRenderer>() // left
            };

            lineRendererTransforms = new Transform[][]
            {
                new Transform[] { bodyJoints[JointIndices3D.head_joint], bodyJoints[JointIndices3D.neck_1_joint] },
                new Transform[] { bodyJoints[JointIndices3D.right_hand_joint], bodyJoints[JointIndices3D.right_forearm_joint], bodyJoints[JointIndices3D.right_arm_joint], bodyJoints[JointIndices3D.left_arm_joint], bodyJoints[JointIndices3D.left_forearm_joint], bodyJoints[JointIndices3D.left_hand_joint]},
                new Transform[] { bodyJoints[JointIndices3D.right_foot_joint], bodyJoints[JointIndices3D.right_leg_joint], bodyJoints[JointIndices3D.right_upLeg_joint], bodyJoints[JointIndices3D.left_upLeg_joint], bodyJoints[JointIndices3D.left_leg_joint], bodyJoints[JointIndices3D.left_foot_joint] },
                new Transform[] { bodyJoints[JointIndices3D.right_arm_joint], bodyJoints[JointIndices3D.right_upLeg_joint] },
                new Transform[] { bodyJoints[JointIndices3D.left_arm_joint], bodyJoints[JointIndices3D.left_upLeg_joint] }
            };

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].positionCount = lineRendererTransforms[i].Length;
            }
        }
    }

    private Transform GetNewJointPrefab(Transform arBodyT)
    {
        return Instantiate(jointPrefab, arBodyT).transform;
    }

    void UpdateBody(ARHumanBody arBody)
    {
        if (jointPrefab == null)
        {
            Debug.Log("no prefab found");
            return;
        }

        Transform arBodyT = arBody.transform;

        if (arBodyT == null)
        {
            Debug.Log("no root transform found for ARHumanBody");
            return;
        }

        InitialiseObjects(arBodyT);

        /// Update joint placement
        NativeArray<XRHumanBodyJoint> joints = arBody.joints;
        foreach (KeyValuePair<JointIndices3D, Transform> item in bodyJoints)
        {
            UpdateJointTransform(item.Value, joints[(int)item.Key]);
        }

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].SetPositions(lineRendererTransforms[i]);
        }
    }

    private void UpdateJointTransform(Transform jointT, XRHumanBodyJoint bodyJoint)
    {
        jointT.localScale = bodyJoint.anchorScale;
        jointT.localRotation = bodyJoint.anchorPose.rotation;
        jointT.localPosition = bodyJoint.anchorPose.position;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        foreach (ARHumanBody humanBody in eventArgs.added)
        {
            UpdateBody(humanBody);
        }

        foreach (ARHumanBody humanBody in eventArgs.updated)
        {
            UpdateBody(humanBody);
        }

        //Debug.Log("Bodies: " + (eventArgs.added.Count + eventArgs.updated.Count).ToString());
    }
}