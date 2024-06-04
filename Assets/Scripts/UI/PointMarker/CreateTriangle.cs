using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateTriangle : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // 頂点座標
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0.3f, 0.8f, 0),
            new Vector3(0, 0.6f, 0),
        };

        // 頂点インデックス
        int[] triangles = new int[]
        {
            0, 1, 2
        };

        // メッシュに頂点とインデックスをセット
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 法線の再計算
    }
}
