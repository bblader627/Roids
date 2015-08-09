using UnityEngine;
using System.Collections;

public class ShieldEffectMobile : MonoBehaviour
{

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
	
	private float[] ShieldDataNm1;
    private float[] ShieldDataNp1;
    private float[] ShieldDataN;
	
	private Mesh ShieldMesh;
	private Vector3[] SharedVertices;
	private Vector3[] SharedNormals;
	private Vector3[] vertices;
	private Color[] colours;
	private int[] triangles;

	/// <summary>
	/// The sleep timer.
	/// </summary>
	private float Timer = 0.01f;
	
	private bool HasError = false;

	// Use this for initialization
	void Start () {

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

		// Used for saving simulation results from timesteps N+1, N, N-1.
        ShieldDataNm1 = new float[ShieldMesh.vertexCount];
        ShieldDataN = new float[ShieldMesh.vertexCount];
        ShieldDataNp1 = new float[ShieldMesh.vertexCount];

		// Check if any of them failed to allocate.
		if(ShieldDataN == null || ShieldDataNm1 == null || ShieldDataNp1 == null)
		{
			Debug.LogError("Shield Data Arrays Have Failed To Allocate On Object: " + gameObject.ToString() + ". Maybe Out Of Memory?");
			HasError = true;
			return;
		}

		// Set everything to 0.
        for (int i = 0; i < ShieldMesh.vertexCount; i++)
        {
            ShieldDataNm1[i] = 0.0f;
            ShieldDataN[i] = 0.0f;
            ShieldDataNp1[i] = 0.0f;
        }

		// Setup the individual mesh data.
		vertices = ShieldMesh.vertices;
		colours = ShieldMesh.colors;
		triangles = ShieldMesh.triangles;
		
		for(int i = 0; i < ShieldMesh.colors.Length; i++)
			colours[i] = Color.black;

		// Update shield vertex colours.
		ShieldMesh.colors = colours;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{		
		// If the object is not asleep, and there has been no errors.
		if(Timer > 0.0f && !HasError)
			// decrement the sleep timer.
			Timer -= Time.deltaTime;
		else   
			// If asleep, or run with errors, return.
			return;

		// Update Shield Simulation.
        SimulateShield();
		
		
		for(int i = 0; i < vertices.Length; i++)
		{
			// Displace vertices based on values from simulation.
            vertices[i] = SharedVertices[i] + (SharedNormals[i] * ShieldDataNp1[i] * 0.1f);

			// If a displacement has occured, Update vertex colours.
            if (ShieldDataNp1[i] < 0)
			{
                colours[i] = ShieldEffectColour * -ShieldDataNp1[i];
				colours[i].a = -ShieldDataNp1[i] * 0.9f;
			}
			else if (Mathf.Approximately(ShieldDataNp1[i], 0.0f))
			{
                colours[i] = ShieldEffectColour * ShieldDataNp1[i];
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
		//for(int i = 0; i < other.contacts.Length; i++)
		//{
			RaycastHit hit;
			
			// Get clamped speed.
		float mag = 0.1f;//Mathf.Clamp(other.relativeVelocity.magnitude * VelocityModifier, -MaxDistorVal, MaxDistorVal);
			
			// Linecast between collision object and this object.
			if(Physics.Linecast(other.contacts[0].point, transform.position, out hit, ShieldLayer))
			{
				// If the linecast hits, update the simulation dataset to indicate a hit.
				GlowEffect(hit.triangleIndex, 0.1f);
			}
		//}
	}

	public void GlowEffect(int TriangleID, float mag)
	{	
		// Update sleep timer, since we know we were just hit.
		Timer = 30.0f;

		// If the triangle is already significantly displaced, ignore the effect.
        if (ShieldDataNp1[triangles[(TriangleID * 3)]] < -2.5f || ShieldDataNp1[triangles[(TriangleID * 3)]] > 2.5f)
			return;

		// Update the hit triangle, along with the surrounding triangles.
        ShieldDataNp1[triangles[(TriangleID * 3)]] += -mag;
        ShieldDataNp1[triangles[(TriangleID * 3) + 1]] += -mag;
		ShieldDataNp1[triangles[(TriangleID * 3) + 2]] += -mag;

		// Check triangle bounds.
		if(TriangleID != 0)
		{
            ShieldDataNp1[triangles[((TriangleID - 1) * 3)]] += -mag;
			ShieldDataNp1[triangles[((TriangleID - 1) * 3) + 1]] += -mag;
			ShieldDataNp1[triangles[((TriangleID - 1) * 3) + 2]] += -mag;
		}
		
		if(TriangleID < triangles.Length - 1)
		{
            ShieldDataNp1[triangles[((TriangleID + 1) * 3)]] += -mag;
			ShieldDataNp1[triangles[((TriangleID + 1) * 3) + 1]] += -mag;
			ShieldDataNp1[triangles[((TriangleID + 1) * 3) + 2]] += -mag;
		}
	}

    void SimulateShield()
    {
		// Used to hold potentially modified delta time and wave speed values.
        float tempWaveSpeed;
        float tempDeltaTime;

		// Calculate maximum stable delta time.
        float stability = ((ShieldStiffness + Mathf.Sqrt(32.0f * WaveSpeed * WaveSpeed)) / (8.0f * WaveSpeed * WaveSpeed));

		// If delta time is greater than, or equal to maximum stable time, manually slow the simulation for a frame.
		if (Time.deltaTime >= stability * 0.6f)
        {
			tempWaveSpeed = WaveSpeed;
			tempDeltaTime = stability * 0.6f;
        }
        else
        {
            tempWaveSpeed = WaveSpeed;
			tempDeltaTime = Time.deltaTime;
        }


		// For each point on the shield mesh.
        for(int i = 0; i < 64; i++)
        {
            for(int j = 0; j < 64; j++)
            {
				// Get indicies for surrounding verts.
                int indexXZ = (i * 64) + j;
                int indexXZm1 = ((i - 1) * 64) + j;
                int indexXZp1 = ((i + 1) * 64) + j;
                int indexXm1Z = (i * 64) + (j - 1);
                int indexXp1Z = (i * 64) + (j + 1);

				// This series of if-elses checks bounds.
                if (i <= 0 && j > 0)
                {
                    indexXZm1 = ((64) * 64) + j;
                }
                else if (j <= 0 && i > 0)
                {
                    indexXm1Z = (i * 64) + 64;
                }
                else if (j <= 0 && i <= 0)
                {
                    indexXZm1 = ((64) * 64) + j;
                    indexXm1Z = (i * 64) + 64;
                }
                else if (i >= 63 && j < 64)
                {
                    indexXZp1 = (0 * 64) + j;
                }
                else if (j >= 63 && i < 63)
                {
                    indexXp1Z = (i * 64) + 0;
                }
                else if (j >= 63 && i >= 63)
                {
                    indexXZp1 = (0 * 64) + j;
                    indexXp1Z = (i * 64) + 0;
                }


				// Update n-1 and n timesteps.
                ShieldDataNm1[indexXZ] = ShieldDataN[indexXZ];
                ShieldDataN[indexXZ] = ShieldDataNp1[indexXZ];

                // 2Z(i,j)n - Z(i,j)n-1
				float height = (2.0f * ShieldDataN[indexXZ]) - ShieldDataNm1[indexXZ];

				// This next section is based on a numerical approximation of the dampened wave equation. Check the comment above each statement
				// for what part of the equation the statement corresponds to.

                // D^2z/Dx^2
                height += (((tempWaveSpeed * tempWaveSpeed) * (tempDeltaTime * tempDeltaTime)) *
                            (ShieldDataN[indexXm1Z] -
                            (2.0f * ShieldDataN[indexXZ]) +
                            ShieldDataN[indexXp1Z]));

                // D^2z/Dy^2
                height += (((tempWaveSpeed * tempWaveSpeed) * (tempDeltaTime * tempDeltaTime)) *
                            (ShieldDataN[indexXZm1] -
				 			(2.0f * ShieldDataN[indexXZ]) +
                            ShieldDataN[indexXZp1]));

				// This additional statement here, adds the aforementioned dampening to the effect.

                // t^2(u * Z(i,j)n - Z(i,j)n-1) / (2.0f * t)
				height -= (tempDeltaTime * tempDeltaTime) * (ShieldStiffness * (ShieldDataN[indexXZ] - ShieldDataNm1[indexXZ])) / (2.0f * tempDeltaTime);

				ShieldDataNp1[indexXZ] = Mathf.Clamp(height, -MaxDistorVal, 0.0f);
            }
        }


    }

	// Used for 2D collisions. If a 2D object detects this on any 2D callback function, use this function to get a reaction from the shield.
    public void Raycast2D(Vector3 point, float mag)
    {
        RaycastHit rayHit;

        if (Physics.Linecast(point, transform.position, out rayHit, ShieldLayer))
        {
            GlowEffect(rayHit.triangleIndex, VelocityModifier);
        }
    }
}
