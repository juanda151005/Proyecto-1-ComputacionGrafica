using UnityEngine;
using UnityEditor;

public class FixURPMaterials : EditorWindow
{
    [MenuItem("Tools/Fix LowPoly URP Materials")]
    public static void FixMaterials()
    {
        string folderPath = "Assets/LowPoly Environment Pack/FBX/Materials";
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });

        int fixedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (mat == null) continue;

            string name = mat.name;
            Color color = GetLowPolyColor(name);

            if (color != Color.clear)
            {
                mat.SetColor("_BaseColor", color);
                mat.SetColor("_Color", color);
                EditorUtility.SetDirty(mat);
                fixedCount++;
                Debug.Log($"Fixed: {name} -> R:{color.r:F3} G:{color.g:F3} B:{color.b:F3}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[FixURPMaterials] Se repararon {fixedCount} materiales.");
        EditorUtility.DisplayDialog("Listo", $"Se repararon {fixedCount} materiales.", "OK");
    }

    static Color GetLowPolyColor(string name)
    {
        // Greens - variaciones de verde para hojas, pasto, etc.
        if (name == "Green.1")  return new Color(0.126f, 0.468f, 0.037f, 1f);
        if (name == "Green.2")  return new Color(0.188f, 0.588f, 0.066f, 1f);
        if (name == "Green.3")  return new Color(0.251f, 0.690f, 0.114f, 1f);
        if (name == "Green.4")  return new Color(0.157f, 0.533f, 0.047f, 1f);
        if (name == "Green.5")  return new Color(0.098f, 0.400f, 0.027f, 1f);
        if (name == "Green.6")  return new Color(0.220f, 0.630f, 0.090f, 1f);
        if (name == "Green.7")  return new Color(0.302f, 0.720f, 0.145f, 1f);
        if (name == "Green.8")  return new Color(0.180f, 0.510f, 0.055f, 1f);
        if (name == "Green.9")  return new Color(0.345f, 0.760f, 0.200f, 1f);
        if (name == "Green.10") return new Color(0.082f, 0.350f, 0.020f, 1f);
        if (name == "Green.11") return new Color(0.400f, 0.780f, 0.260f, 1f);
        if (name == "Green.12") return new Color(0.145f, 0.490f, 0.043f, 1f);
        if (name == "Green.13") return new Color(0.270f, 0.700f, 0.130f, 1f);

        // Browns - variaciones de cafe para troncos, tierra, madera
        if (name == "Brown.2")  return new Color(0.400f, 0.255f, 0.118f, 1f);
        if (name == "Brown.3")  return new Color(0.467f, 0.298f, 0.137f, 1f);
        if (name == "Brown.4")  return new Color(0.345f, 0.220f, 0.098f, 1f);
        if (name == "Brown.5")  return new Color(0.529f, 0.341f, 0.157f, 1f);
        if (name == "Brown.6")  return new Color(0.310f, 0.196f, 0.086f, 1f);
        if (name == "Brown.7")  return new Color(0.588f, 0.376f, 0.176f, 1f);
        if (name == "Brown.8")  return new Color(0.275f, 0.173f, 0.075f, 1f);
        if (name == "Brown.9")  return new Color(0.424f, 0.271f, 0.125f, 1f);
        if (name == "Brown.10") return new Color(0.500f, 0.318f, 0.149f, 1f);
        if (name == "Brown.11") return new Color(0.373f, 0.235f, 0.106f, 1f);
        if (name == "Brown.14") return new Color(0.557f, 0.357f, 0.165f, 1f);
        if (name == "Brown.15") return new Color(0.443f, 0.282f, 0.133f, 1f);

        // Grays - piedras, rocas
        if (name == "Gray.1")  return new Color(0.502f, 0.502f, 0.502f, 1f);
        if (name == "Gray.2")  return new Color(0.600f, 0.600f, 0.600f, 1f);
        if (name == "Gray.3")  return new Color(0.420f, 0.420f, 0.420f, 1f);
        if (name == "Gray.4")  return new Color(0.710f, 0.710f, 0.710f, 1f);

        // Oranges - flores, detalles
        if (name == "Orange.1") return new Color(0.878f, 0.463f, 0.098f, 1f);
        if (name == "Orange.2") return new Color(0.945f, 0.553f, 0.153f, 1f);
        if (name == "Orange.3") return new Color(0.800f, 0.400f, 0.067f, 1f);

        // Yellows - flores, detalles
        if (name == "Yellow.1") return new Color(0.945f, 0.827f, 0.173f, 1f);
        if (name == "Yellow.2") return new Color(0.878f, 0.757f, 0.118f, 1f);
        if (name == "Yellow.3") return new Color(0.980f, 0.890f, 0.310f, 1f);
        if (name == "Yellow.4") return new Color(0.850f, 0.730f, 0.090f, 1f);

        // Pinks
        if (name == "Pink.1") return new Color(0.878f, 0.396f, 0.557f, 1f);

        // Purples
        if (name == "Purple.1") return new Color(0.557f, 0.278f, 0.678f, 1f);
        if (name == "Purple.2") return new Color(0.667f, 0.380f, 0.780f, 1f);

        return Color.clear;
    }
}
