using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public class Raytracer : MonoBehaviour
{
    public ComputeShader raytraceShader;
    public string raytraceKernelName;
    public Camera cam;
    public Renderer outputRenderer;
    public Vector2Int outputTexSize;

    private int raytraceKernelIdx;
    private uint raytraceGroupSizeX, raytraceGroupSizeY;
    private RenderTexture outputRT;
    private ComputeBuffer sphereBuffer;

    private void Awake()
    {
        raytraceKernelIdx = raytraceShader.FindKernel(raytraceKernelName);
        raytraceShader.GetKernelThreadGroupSizes(raytraceKernelIdx, out raytraceGroupSizeX, out raytraceGroupSizeY, out _);

        CreateOutputRT();
        outputRenderer.material.SetTexture("_MainTex", outputRT);

        ConstructShapeBuffers();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            CreateOutputRT();
        }
    }
#endif

    private void OnDisable()
    {
        DisposeOfShapeBuffers();   
    }

    private void Update()
    {
        Raytrace();    
    }

    private void Raytrace()
    {
        ConstructShapeBuffers();

        //shapes
        raytraceShader.SetBuffer(raytraceKernelIdx, "spheres", sphereBuffer);
        raytraceShader.SetInt("numSpheres", sphereBuffer.count);

        //camera
        raytraceShader.SetFloats("camPos", cam.transform.position.ToArray());
        raytraceShader.SetFloats("camRight", cam.transform.right.ToArray());
        raytraceShader.SetFloats("camUp", cam.transform.up.ToArray());
        raytraceShader.SetFloats("camForward", cam.transform.forward.ToArray());
        raytraceShader.SetFloat("camNearClipDist", cam.nearClipPlane);
        float nearClipHeight = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.nearClipPlane * 2;
        raytraceShader.SetFloat("camNearClipWidth", nearClipHeight * cam.aspect);
        raytraceShader.SetFloat("camNearClipHeight", nearClipHeight);


        //output texture info
        raytraceShader.SetTexture(raytraceKernelIdx, "outputTex", outputRT);
        raytraceShader.SetInt("outputTexWidth", outputRT.width);
        raytraceShader.SetInt("outputTexHeight", outputRT.height);

        int groupsX = Mathf.Max(outputRT.width / (int)raytraceGroupSizeX, 1);
        int groupsY = Mathf.Max(outputRT.height / (int)raytraceGroupSizeY, 1);
        raytraceShader.Dispatch(raytraceKernelIdx, groupsX, groupsY, 1);
    }

    private void CreateOutputRT()
    {
        if (outputTexSize.x <= 0)
        {
            outputTexSize.x = 1;
        }

        if (outputTexSize.y <= 0)
        {
            outputTexSize.y = 1;
        }

        var rtDesc = new RenderTextureDescriptor(outputTexSize.x, outputTexSize.y, RenderTextureFormat.ARGB32);
        rtDesc.dimension = TextureDimension.Tex2D;
        rtDesc.enableRandomWrite = true;

        outputRT = new RenderTexture(rtDesc);
        outputRT.Create();
    }

    private void ConstructShapeBuffers()
    {
        //spheres
        var spheres = GetComponentsInChildren<RTSphere>();
        var sphereList = new List<GPUSphere>(spheres.Length); 
        foreach(var sphere in spheres)
        {
            sphereList.Add(new GPUSphere(sphere.transform.position, sphere.radius, sphere.mat));
        }

        sphereBuffer?.Dispose();
        sphereBuffer = new ComputeBuffer(spheres.Length, Marshal.SizeOf(typeof(GPUSphere)));
        sphereBuffer.SetData(sphereList);
    }

    private void DisposeOfShapeBuffers()
    {
        sphereBuffer?.Dispose();
    }
}

//raytraced shape structs
public struct GPUSphere
{
    public Vector3 pos;
    public float r;
    public RaytraceMaterial mat;

    public GPUSphere(Vector3 pos, float r, RaytraceMaterial mat)
    {
        this.pos = pos;
        this.r = r;
        this.mat = mat;
    }
}

//material for raytraced objects
[System.Serializable]
public struct RaytraceMaterial
{
    public Color col;
    public float smoothness;
    public float emissive;
    public int isMetal;
}

//vector -> array extension methods
public static class VecToArray
{
    public static float[] ToArray(this Vector3 vec)
    {
        return new float[3] { vec.x, vec.y, vec.z };
    }

    public static float[] ToArray(this Vector4 vec)
    {
        return new float[4] { vec.x, vec.y, vec.z, vec.w };
    }

    public static int[] ToArray(this Vector3Int vec)
    {
        return new int[3] { vec.x, vec.y, vec.z };
    }
}
