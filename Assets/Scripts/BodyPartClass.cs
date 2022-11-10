using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable]
public class BodyPartContainer: MonoBehaviour
{
        public BodyPartContainer bodycontainers;

        [Serializable]
        class BodyPartsClass: MonoBehaviour
        { 
                public BodyPart BP;
                public float RestingAngle = 0f;
                public float AppliedForce = 750f;

        }
       
}
