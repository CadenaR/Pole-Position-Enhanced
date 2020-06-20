using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using UnityEditor.Experimental.UIElements;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public GameObject m_Focus;
    
    [SerializeField] public Vector3 m_offset = new Vector3(10, 10, 10);

    [SerializeField] public CircuitController m_Circuit;
    [SerializeField] private float m_Distance = 10;
    [SerializeField] private float m_Elevation = 8;
    [Range(0, 1)] [SerializeField] private float m_Following = 0.5f;

    [SerializeField] private int m_cameraAceleration = 6;

    [SerializeField] private GameObject endCamera;

    private Vector3 m_Direction = Vector3.zero;
    
    private Camera mainCamera;

    public bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = this.GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Focus != null && !end)
        {
            
            if (this.m_Circuit != null)
            {
                if (this.m_Direction.magnitude == 0)
                {
                    this.m_Direction = new Vector3(0f, -1f, 0f);
                }

                int segIdx;
                float carDist;
                Vector3 carProj;

                m_Circuit.ComputeClosestPointArcLength(m_Focus.transform.position, out segIdx, out carProj,
                    out carDist);

                Vector3 pathDir = -m_Circuit.GetSegment(segIdx);
                pathDir = new Vector3(pathDir.x, 0f, pathDir.z);
                pathDir.Normalize();

                this.m_Direction = Vector3.Lerp(this.m_Direction, pathDir, this.m_Following * Time.deltaTime * m_cameraAceleration);
                Vector3 offset = this.m_Direction * this.m_Distance;
                offset = new Vector3(offset.x, m_Elevation, offset.z);

                mainCamera.transform.position = m_Focus.transform.position + offset;
                mainCamera.transform.LookAt(m_Focus.transform.position);
            }
            else
            {
                mainCamera.transform.position = m_Focus.transform.position + m_offset;
                mainCamera.transform.LookAt(m_Focus.transform.position);
            }
        }
    }
    
    public void SwapToEndCamera(){// cambio de camara para la pantalla de resultados finales
        end = true;
        this.endCamera.SetActive(true);
        this.mainCamera.gameObject.SetActive(false);
        //GetComponent<Transform>().SetPositionAndRotation(new Vector3(-42.08f, 3.64f, 62.12f), new Quaternion (12f, 0f, 0f, 1f));
    }
}