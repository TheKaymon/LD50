using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Office : MonoBehaviour
{
    private List<Employee> employees = new List<Employee>();
    public int EmployeeCount => ( employees.Count );
    //public Boss boss;
    public Pathfinding pathfinder;
    public Vector2 breakroom;
    public Vector2 bathroom;

    public List<Color> eyeColors;
    public List<Color> tieColors;
    public List<string> names;


    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public string RegisterEmployee( Employee employee )
    {
        employees.Add(employee);
        return names[Random.Range(0, names.Count)];
    }

    public Employee GetRandomEmployee( Employee requesting )
    {
        int index = Random.Range(0, EmployeeCount);
        while ( employees[index] == requesting )
            index = Random.Range(0, EmployeeCount);
        return employees[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(breakroom, 0.1f);
        Gizmos.DrawSphere(bathroom, 0.1f);
    }
}
