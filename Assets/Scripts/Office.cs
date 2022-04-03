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




    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public int RegisterEmployee( Employee employee )
    {
        employees.Add(employee);
        return EmployeeCount - 1;
    }

    public Employee GetRandomEmployee( int excludeID )
    {
        int index = Random.Range(0, EmployeeCount);
        while ( index == excludeID )
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
