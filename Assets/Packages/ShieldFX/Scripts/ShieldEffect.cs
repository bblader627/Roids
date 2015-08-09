using UnityEngine;
using System.Collections;

public class ShieldEffect : MonoBehaviour {
	
	/// <summary>
	/// Compute Shader For The Shield.
	/// </summary>
	public ComputeShader ShieldComputeEffect;
	
	/// <summary>
	/// The wave speed.
	/// </summary>
	public float WaveSpeed = 1.0f;
	
	/// <summary>
	/// The shield stiffness. Determins how quickly shield ripples fade.
	/// </summary>
	public float ShieldStiffness = 0.5f;
	
	/// <summary>
	/// The shield effect colour. Determines the colour the shield will glow when hit.
	/// </summary>
	public Color ShieldEffectColour = Color.white;
	
	/// <summary>
	/// The shield material.
	/// </summary>
	private Material ShieldMaterial;
	
	/// <summary>
	/// The length of the glow effect.
	/// </summary>
	public float GlowEffectLength = 1.0f;
	
	/// <summary>
	/// The shield layer. Used for linecasts.
	/// </summary>
	public LayerMask ShieldLayer;
	
	/// <summary>
	/// The max distortion value. Determines how deep a collision can dent the shield.
	/// </summary>
	public float MaxDistorVal = 0.01f;
	
	/// <summary>
	/// The velocity modifier. Modifies how much the velocity impacts the dent made into the shield.
	/// </summary>
	public float VelocityModifier = 0.5f;
	
	/// <summary>
	/// The stability factor. Speeds up the simulation. Temporary fix. Seems to reduce the instances in which the simulation becomes unstable.
	/// </summary>
	public float StabilityFactor = 5.0f;
	
	/// <summary>
	/// Should we update the normals? Setting this to false can improve performance. However you will lose some effects.
	/// Most important for the "Advanced" shader.
	/// </summary>
	public bool ShouldUpdateNormals = true;

	// Data buffers for the compute shader.
	private ComputeBuffer ShieldBufferN;
	private ComputeBuffer ShieldBufferNp1;
	private ComputeBuffer ShieldBufferNm1;

	// float array to get the simulation data from the compute shader.
	private float[] ShieldBufferData;

	private Mesh ShieldMesh;
	private Vector3[] SharedVertices;
	private Vector3[] SharedNormals;

	private Vector3[] vertices;
	private Color[] colours;
	private int[] triangles;

	private float Timer = 0.01f;
	private bool HasError = false;

	// Use this for initialization
	void Start () {

		if(SystemInfo.graphicsShaderLevel < 50)
			Debug.LogError("The Object: " + gameObject.ToString() + " requires DirectX 11 to run. Please Run on a machine which supports this, or change to the mobile script");
		// Get Shield Mesh data (and shared mesh data).
		ShieldMesh = GetComponent<MeshFilter>().mesh;
		SharedVertices = GetComponent<MeshFilter>().sharedMesh.vertices;
		SharedNormals = GetComponent<MeshFilter>().sharedMesh.normals;
		
		// Check we got the data.
		if(ShieldMesh == null)
		{
			Debug.LogError("Mesh Filer Missing On Object: " + gameObject.ToString() + ". Please Add a Mesh Filter.");
			HasError = true;
			return;
		}
		
		// Get the material
		ShieldMaterial = GetComponent<MeshRenderer>().material;
		
		// Check we got the material.
		if(ShieldMaterial == null)
		{
			Debug.LogError("Material Missing On Object: " + gameObject.ToString() + ". Please Add a Material.");
			HasError = true;
			return;
		}

		// Create Compute Shader Buffers.
		ShieldBufferN = new ComputeBuffer(ShieldMesh.vertexCount, sizeof(float));
		ShieldBufferNp1 = new ComputeBuffer(ShieldMesh.vertexCount, sizeof(float));
		ShieldBufferNm1 = new ComputeBuffer(ShieldMesh.vertexCount, sizeof(float));

		// Check if any of them failed to allocate.
		if(ShieldBufferN == null || ShieldBufferNp1 == null || ShieldBufferNm1 == null)
		{
			Debug.LogError("Shield Data Buffers Have Failed To Allocate On Object: " + gameObject.ToString() + ". Maybe Out Of Memory?");
			HasError = true;
			return;
		}

		// Create Array to Get Data From Compute Buffer.
		ShieldBufferData = new float[ShieldMesh.vertexCount];

		// Check if the array allocated.
		if(ShieldMaterial == null)
		{
			Debug.LogError("Shield Data Array Failed To Allocate On Object: " + gameObject.ToString() + ". Maybe Out Of Memory?");
			HasError = true;
			return;
		}

		vertices = ShieldMesh.vertices;
		colours = ShieldMesh.colors;
		triangles = ShieldMesh.triangles;
		
		for(int i = 0; i < ShieldMesh.colors.Length; i++)
			colours[i] = Color.black;
		
		ShieldMesh.colors = colours;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// If the object is not asleep, and there has been no errors.
		if(Timer > 0.0f && !HasError)
			// decrement the sleep timer.
			Timer -= Time.deltaTime;
		else   
			// If asleep, or run with errors, return.
			return;
	
		// Get the current data from the compute shader.
		ShieldBufferNp1.GetData(ShieldBufferData);

		// 
		float stability = ((ShieldStiffness + Mathf.Sqrt(32.0f * WaveSpeed * WaveSpeed)) / (8.0f * WaveSpeed * WaveSpeed)) * 0.9f;
		
		ShieldComputeEffect.SetFloat("mMU", ShieldStiffness);
		
		if(Time.deltaTime * StabilityFactor >= stability)
		{
			ShieldComputeEffect.SetFloat("mWaveSpeed", WaveSpeed);
			ShieldComputeEffect.SetFloat("mDeltaTime", stability * 0.1f);
		}
		else
		{
			ShieldComputeEffect.SetFloat("mWaveSpeed", WaveSpeed);
			ShieldComputeEffect.SetFloat("mDeltaTime", Time.deltaTime * StabilityFactor);
		}
		
		ShieldComputeEffect.SetBuffer(0, "np1PointBuffer", ShieldBufferNp1);
		ShieldComputeEffect.SetBuffer(0, "nPointBuffer", ShieldBufferN);
		ShieldComputeEffect.SetBuffer(0, "nm1PointBuffer", ShieldBufferNm1);
		
		ShieldComputeEffect.Dispatch(0, 4, 4, 1);
		
		for(int i = 0; i < vertices.Length; i++)
		{
			// Displace vertices based on values from simulation.
			vertices[i] = SharedVertices[i] + (SharedNormals[i] * ShieldBufferData[i] * 0.1f);
			
			// If a displacement has occured, Update vertex colours.
			if (ShieldBufferData[i] < 0)
			{
				colours[i] = ShieldEffectColour * -ShieldBufferData[i];
				colours[i].a = -ShieldBufferData[i];
			}
			else
			{
				colours[i] = ShieldEffectColour * ShieldBufferData[i];
				colours[i].a = 0.0f;
			}
		}
		
		
		// Update data.
		ShieldMesh.vertices = vertices;
		ShieldMesh.colors = colours;

		if(ShouldUpdateNormals)
			ShieldMesh.RecalculateNormals();
	}
	
	void OnCollisionStay(Collision other)
	{
		for(int i = 0; i < other.contacts.Length; i++)
		{
			RaycastHit hit;
			
			// Get clamped speed.
			float mag = Mathf.Clamp(other.relativeVelocity.magnitude * VelocityModifier, -MaxDistorVal, MaxDistorVal);
			
			// Linecast between collision object and this object.
			if(Physics.Linecast(other.contacts[i].point, transform.position, out hit, ShieldLayer))
			{
				// If the linecast hits, update the simulation dataset to indicate a hit.
				GlowEffect(hit.triangleIndex, mag);
			}
		}
	}
	
	// Enumerator to enable extra work later on.
	public void GlowEffect(int TriangleID, float mag)
	{	
		// Update sleep timer, since we know we were just hit.
		Timer = 30.0f;

		// Make sure the data is up-to-date... May not be required!
		ShieldBufferNp1.GetData(ShieldBufferData);

		// If the triangle is already significantly displaced, ignore the effect.
		if(ShieldBufferData[triangles[(TriangleID * 3)]] < -2.0f || ShieldBufferData[triangles[(TriangleID * 3)]] > 2.0f)
			return;

		// Update the hit triangle, along with the surrounding triangles.
		ShieldBufferData[triangles[(TriangleID * 3)]] += -mag;
		ShieldBufferData[triangles[(TriangleID * 3) + 1]] += -mag;
		ShieldBufferData[triangles[(TriangleID * 3) + 2]] += -mag;

		// Check triangle bounds.
		if(TriangleID != 0)
		{
			ShieldBufferData[triangles[((TriangleID - 1) * 3)]] += -mag;
			ShieldBufferData[triangles[((TriangleID - 1) * 3) + 1]] += -mag;
			ShieldBufferData[triangles[((TriangleID - 1) * 3) + 2]] += -mag;
		}
		
		if(TriangleID < triangles.Length - 1)
		{
			ShieldBufferData[triangles[((TriangleID + 1) * 3)]] += -mag;
			ShieldBufferData[triangles[((TriangleID + 1) * 3)+ 1]] += -mag;
			ShieldBufferData[triangles[((TriangleID + 1) * 3) + 2]] += -mag;
		}

		/*Mathf.Clamp(ShieldBufferData[triangles[(TriangleID * 3)]], -1.0f, 1.0f);
		Mathf.Clamp(ShieldBufferData[triangles[(TriangleID * 3) + 1]], -1.0f, 1.0f);
		Mathf.Clamp(ShieldBufferData[triangles[(TriangleID * 3) + 2]], -1.0f, 1.0f);*/

		// Update the shield compute shader buffer.
		ShieldBufferNp1.SetData(ShieldBufferData);
	}
	
	void OnDestroy()
	{
		// Dispose all of the compute buffers.
		ShieldBufferNp1.Dispose();
		ShieldBufferN.Dispose();
		ShieldBufferNm1.Dispose();
	}
	
	//void OnCollisionEnter(
}
