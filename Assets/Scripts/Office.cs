using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Office : MonoBehaviour
{
    private List<Employee> employees = new List<Employee>();
    public int EmployeeCount => ( employees.Count );
    //public Boss boss;
    public Pathfinding pathfinder;

    public List<Vector2> movementPoints;

    public List<Sprite> sprites;
    public List<Color> eyeColors;
    public List<Color> tieColors;
    public List<string> names;
    private int prevNameIndex = -1;


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
        int nameIndex = Random.Range(0, names.Count);
        while ( nameIndex == prevNameIndex )
            nameIndex = Random.Range(0, names.Count);
        prevNameIndex = nameIndex;

        return names[nameIndex];
    }

    public Employee GetRandomEmployee( Employee requesting )
    {
        int index = Random.Range(0, EmployeeCount);
        while ( employees[index] == requesting )
            index = Random.Range(0, EmployeeCount);
        return employees[index];
    }

    public Vector2 GetDestination()
    {
        return movementPoints[Random.Range(0, movementPoints.Count)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach ( Vector2 point in movementPoints )
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
