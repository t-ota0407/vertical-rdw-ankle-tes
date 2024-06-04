using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateTriangle : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // ���_���W
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0.3f, 0.8f, 0),
            new Vector3(0, 0.6f, 0),
        };

        // ���_�C���f�b�N�X
        int[] triangles = new int[]
        {
            0, 1, 2
        };

        // ���b�V���ɒ��_�ƃC���f�b�N�X���Z�b�g
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // �@���̍Čv�Z
    }
}
