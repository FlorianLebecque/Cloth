namespace Gravity.utils;



public class IcoSphere{

    private Vector3[] vertices;

    public IcoSphere(float radius,int subdivision){

        vertices = Icosahedron();
        vertices = SubdivideTriangles(vertices,subdivision);
        vertices = FilterDuplicateVertices(vertices);
        vertices = MapToSphere(vertices,radius);
    }

    //generate an array of triangles making a icosahedron
    private static Vector3[] Icosahedron(){
        //generate an array of coordinate vector3 of a ICOSAHEDRON
        Vector3 n0  = new Vector3(0,1,0.5f);
        Vector3 n1  = new Vector3(0,1,-0.5f);
        Vector3 n2  = new Vector3(0,-1,0.5f);
        Vector3 n3  = new Vector3(0,-1,-0.5f);
        Vector3 n4  = new Vector3(1,0.5f,0);
        Vector3 n5  = new Vector3(1,-0.5f,0);
        Vector3 n6  = new Vector3(-1,0.5f,0);
        Vector3 n7  = new Vector3(-1,-0.5f,0);
        Vector3 n8  = new Vector3(0.5f,0,1);
        Vector3 n9  = new Vector3(-0.5f,0,1);
        Vector3 n10 = new Vector3(0.5f,0,-1);
        Vector3 n11 = new Vector3(-0.5f,0,-1);

        //triangles of the icosahedron
        Vector3[] icosahedron = new Vector3[]{
            n0,n1,n6,
            n0,n6,n9,
            n0,n9,n8,
            n0,n8,n4,
            n0,n4,n1,
            n1,n10,n11,
            n1,n11,n6,
            n1,n10,n4,
            n2,n3,n5,
            n2,n5,n8,
            n2,n8,n9,
            n2,n9,n7,
            n2,n7,n3,
            n3,n7,n11,
            n3,n11,n10,
            n3,n10,n5,
            n4,n8,n5,
            n4,n5,n10,
            n6,n11,n7,
            n6,n7,n9
        };

        return icosahedron;
    }

    //subdivide a triangle
    private static Vector3[] SubdivideTriangle(Vector3[] triangle){
        
        //midle of point 1 and 2
        Vector3 middle1 = (triangle[0] + triangle[1])/2;
        //midle of point 2 and 3
        Vector3 middle2 = (triangle[1] + triangle[2])/2;
        //midle of point 1 and 3
        Vector3 middle3 = (triangle[0] + triangle[2])/2;

        //subdivide the triangle
        Vector3[] subdivide = new Vector3[12];
        subdivide[0] = triangle[0];
        subdivide[1] = middle1;
        subdivide[2] = middle3;

        subdivide[3] = triangle[1];
        subdivide[4] = middle2;
        subdivide[5] = middle1;

        subdivide[6] = triangle[2];
        subdivide[7] = middle3;
        subdivide[8] = middle2;

        subdivide[9] = middle1;
        subdivide[10] = middle2;
        subdivide[11] = middle3;

        return subdivide;
    }

    //subdivide an array of triangles
    private static Vector3[] SubdivideTriangles(Vector3[] triangles){

        if(triangles.Length % 3 != 0){
            throw new Exception("triangles.Length must be a multiple of 3");
        }

        Vector3[] newTriangles = new Vector3[0];

        //for each triangle
        for(int i = 0; i < triangles.Length; i+=3){
            //subdivide the triangle
            Vector3[] subdivide = SubdivideTriangle(new Vector3[]{triangles[i],triangles[i+1],triangles[i+2]});
            //add the subdivided triangle to the array
            newTriangles = newTriangles.Concat(subdivide).ToArray();
        }
        
        return newTriangles;
    }

    //subdivide an array of triangles n times
    private static Vector3[] SubdivideTriangles(Vector3[] triangles,int n){
        Vector3[] newTriangles = triangles;
        for(int i = 0; i < n; i++){
            newTriangles = SubdivideTriangles(newTriangles);
        }
        return newTriangles;
    }

    //filter duplicate vertices
    private static Vector3[] FilterDuplicateVertices(Vector3[] vertices){
        //create a dictionary of vertices
        Dictionary<Vector3,int> verticesDict = new Dictionary<Vector3,int>();
        //for each vertex
        for(int i = 0; i < vertices.Length; i++){
            //if the vertex is not in the dictionary
            if(!verticesDict.ContainsKey(vertices[i])){
                //add it to the dictionary
                verticesDict.Add(vertices[i],i);
            }
        }

        //create a new array of vertices
        Vector3[] newVertices = new Vector3[verticesDict.Count];
        //for each vertex
        int index = 0;
        foreach(KeyValuePair<Vector3,int> vertex in verticesDict){
            //add the vertex to the new array
            newVertices[index] = vertex.Key;
            index++;
        }

        return newVertices;
    }

    //map to sphere vertices
    private static Vector3[] MapToSphere(Vector3[] vertices,float radius){
        //create a new array of vertices
        Vector3[] newVertices = new Vector3[vertices.Length];
        //for each vertex
        for(int i = 0; i < vertices.Length; i++){
            //map the vertex to the sphere
            newVertices[i] = Vector3.Normalize(vertices[i]) * radius;
        }
        return newVertices;
    }

    public Vector3[] GetVertices(){
        return vertices;
    }

}