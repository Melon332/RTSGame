using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public GameObject m_fogOfWarPlane;
    private List<Transform> _units = new List<Transform>();
    public LayerMask m_fogLayer;
    public float m_radius = 5f;

    private float m_radiusSqr
    {
        get { return m_radius * m_radius; }
    }

    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Color[] m_colors;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Hello");
        foreach (var unit in UnitManager.SelectableUnits)
        {
            _units.Add(unit.transform);
        }

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
        foreach (var unit in _units)
        {
            Ray r = new Ray(transform.position, unit.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, m_fogLayer, QueryTriggerInteraction.Collide))
            {
                Debug.Log(r);
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Vector3 v = m_vertices[i];
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if (dist < m_radiusSqr)
                    {
                        float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusSqr);
                        m_colors[i].a = alpha;
                    }
                }
            }
        }
        UpdateColor();
    }


    void Initialize()
    {
        m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_vertices = m_mesh.vertices;
        m_colors = new Color[m_vertices.Length];
        for (int i = 0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.black;
        }
        for (int i = 0; i < m_vertices.Length; i++)
        {
            m_vertices[i] = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
        }
        UpdateColor();
    }
    void UpdateColor()
    {
        m_mesh.colors = m_colors;
    }
}
    
