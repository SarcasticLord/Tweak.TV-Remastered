// this script is full ai generated. it was used to debug occulusion culling issues that were fixed
// this script has no other uses for tweak.tv

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MeshOcclusionDiagnostics : EditorWindow
{
    [MenuItem("Tools/Mesh Diagnostics/Scan Project Meshes")]
    static void ScanMeshes()
    {
        Debug.Log("========== MESH DIAGNOSTICS STARTED ==========");

        HashSet<int> checkedMeshes = new HashSet<int>();

        int warningCount = 0;
        int errorCount = 0;
        int meshCount = 0;

        string[] assetGUIDs = AssetDatabase.FindAssets("t:Model t:Mesh");

        foreach (string guid in assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in assets)
            {
                Mesh mesh = asset as Mesh;

                if (mesh == null)
                    continue;

                if (checkedMeshes.Contains(mesh.GetInstanceID()))
                    continue;

                checkedMeshes.Add(mesh.GetInstanceID());

                meshCount++;

                CheckMesh(mesh, path, ref warningCount, ref errorCount);
            }
        }

        // Check meshes currently loaded in scenes

        MeshFilter[] meshFilters =
            Resources.FindObjectsOfTypeAll<MeshFilter>();

        foreach (MeshFilter filter in meshFilters)
        {
            if (EditorUtility.IsPersistent(filter))
                continue;

            if (!filter.gameObject.scene.IsValid())
                continue;

            CheckSceneMesh(
                filter.sharedMesh,
                filter.gameObject,
                checkedMeshes,
                ref meshCount,
                ref warningCount,
                ref errorCount
            );
        }

        SkinnedMeshRenderer[] skinnedMeshes =
            Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in skinnedMeshes)
        {
            if (EditorUtility.IsPersistent(renderer))
                continue;

            if (!renderer.gameObject.scene.IsValid())
                continue;

            CheckSceneMesh(
                renderer.sharedMesh,
                renderer.gameObject,
                checkedMeshes,
                ref meshCount,
                ref warningCount,
                ref errorCount
            );
        }

        Debug.Log("========== MESH DIAGNOSTICS FINISHED ==========");
        Debug.Log("Meshes checked: " + meshCount);
        Debug.LogWarning("Potential issues found: " + warningCount);
        Debug.LogError("Definite mesh data problems found: " + errorCount);
    }

    static void CheckSceneMesh(
        Mesh mesh,
        GameObject obj,
        HashSet<int> checkedMeshes,
        ref int meshCount,
        ref int warningCount,
        ref int errorCount)
    {
        if (mesh == null)
            return;

        if (checkedMeshes.Contains(mesh.GetInstanceID()))
            return;

        checkedMeshes.Add(mesh.GetInstanceID());

        meshCount++;

        string location = obj.scene.path + " | " + GetPath(obj.transform);

        CheckMesh(mesh, location, ref warningCount, ref errorCount);
    }

    static void CheckMesh(
        Mesh mesh,
        string location,
        ref int warningCount,
        ref int errorCount)
    {
        List<string> errors = new List<string>();
        List<string> warnings = new List<string>();

        if (mesh.vertexCount == 0)
        {
            errors.Add("Mesh contains zero vertices.");
        }

        // Check vertex attributes

        if (!mesh.HasVertexAttribute(VertexAttribute.Position))
        {
            errors.Add("Missing Position vertex channel.");
        }
        else
        {
            int positionSize =
                mesh.GetVertexAttributeDimension(VertexAttribute.Position);

            if (positionSize < 3)
                errors.Add("Position vertex channel has fewer than 3 dimensions.");

            if (positionSize > 3)
                warnings.Add("Position vertex channel has more than 3 dimensions.");
        }

        if (!mesh.HasVertexAttribute(VertexAttribute.Normal))
        {
            warnings.Add("Missing Normal vertex channel.");
        }

        if (!mesh.HasVertexAttribute(VertexAttribute.Tangent))
        {
            warnings.Add("Missing Tangent vertex channel.");
        }

        if (!mesh.HasVertexAttribute(VertexAttribute.TexCoord0))
        {
            warnings.Add("Missing UV0 vertex channel.");
        }

        if (!mesh.HasVertexAttribute(VertexAttribute.Color))
        {
            warnings.Add("Missing Color vertex channel.");
        }

        // Check mesh readability

        if (!mesh.isReadable)
        {
            warnings.Add("Mesh is not readable. Vertex data cannot be fully inspected.");
        }
        else
        {
            // Check vertex positions

            Vector3[] vertices = mesh.vertices;

            foreach (Vector3 vertex in vertices)
            {
                if (float.IsNaN(vertex.x) ||
                    float.IsNaN(vertex.y) ||
                    float.IsNaN(vertex.z) ||
                    float.IsInfinity(vertex.x) ||
                    float.IsInfinity(vertex.y) ||
                    float.IsInfinity(vertex.z))
                {
                    errors.Add("Mesh contains invalid vertex positions (NaN or Infinity).");
                    break;
                }
            }

            // Check submeshes and their indices

            if (mesh.subMeshCount == 0)
            {
                warnings.Add("Mesh contains no submeshes.");
            }

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                try
                {
                    int[] indices = mesh.GetIndices(i);

                    if (indices.Length == 0)
                    {
                        warnings.Add("Submesh " + i + " contains no indices.");
                        continue;
                    }

                    foreach (int index in indices)
                    {
                        if (index < 0 || index >= mesh.vertexCount)
                        {
                            errors.Add(
                                "Submesh " + i +
                                " contains an out-of-range vertex index."
                            );

                            break;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    warnings.Add(
                        "Could not inspect submesh " + i + ": " + e.Message
                    );
                }
            }
        }

        // Report results

        if (errors.Count > 0)
        {
            errorCount += errors.Count;

            Debug.LogError(
                "[MESH ERROR] " + mesh.name +
                "\nLocation: " + location +
                "\nVertex Count: " + mesh.vertexCount +
                "\nSubmeshes: " + mesh.subMeshCount +
                "\nProblems:\n- " + string.Join("\n- ", errors),
                mesh
            );
        }

        if (warnings.Count > 0)
        {
            warningCount += warnings.Count;

            Debug.LogWarning(
                "[MESH WARNING] " + mesh.name +
                "\nLocation: " + location +
                "\nVertex Count: " + mesh.vertexCount +
                "\nSubmeshes: " + mesh.subMeshCount +
                "\nProblems:\n- " + string.Join("\n- ", warnings),
                mesh
            );
        }
    }

    static string GetPath(Transform transform)
    {
        string path = transform.name;

        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }
}
