using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;

public class Procedrualgenertion : MonoBehaviour
{
	public GameObject FloorGameObj;

	public GameObject WallGameObj;

	public GameObject NavMeshCubeGameObj;

	public GameObject sepWallGameObj;

	public GameObject EEbottle;

	public GameObject light;

	private GameObject FloorNavParent;

	private GameObject WallParent;

	private GameObject CeallingParent;

	public List<GameObject> Items = new List<GameObject>();

	public int worldSizeX = 40;

	public int worldSizeZ = 40;

	public float wallheight = 5f;

	public NavMeshSurface[] surfaces;

	private int gridOffset = 1;

	public int spawnRate = 100;

	public int enemySpawnRate = 100;

	public static int minEnemySpawn = 5;

	public GameObject Enemy;

	public Transform playerTransform;

	public Quaternion playerQuaternion;

	public GameObject Player;

	private int roomsBuilt;

	private int dividerAmount;

	private int deswall;

	private void Start()
	{
		generateRoom();
		onStartVariable();
		Debug.Log(worldSizeX);
		Debug.Log(worldSizeZ);
		Debug.Log(wallheight);
	}

	private void onStartVariable()
	{
		minEnemySpawn = 5;
	}

	private void generateRoom()
	{
		GenerateFloors();
		GenerateWalls();
		GenerateCeilling();
		combineMesh(FloorNavParent);
		combineMesh(WallParent);
		combineMesh(CeallingParent);
		addNaveMesh();
		enemySpawn();
		spawnDivider();
		EE();
		Object.Instantiate(light, new Vector3(worldSizeX / 2, wallheight, worldSizeZ / 2), Quaternion.identity);
	}

	private void GenerateFloors()
	{
		for (int i = 0; i < worldSizeX; i++)
		{
			if (i == 0)
			{
				float x = i * gridOffset;
				_ = gridOffset;
				Vector3 position = new Vector3(x, 0f, 0f);
				FloorNavParent = Object.Instantiate(NavMeshCubeGameObj, position, Quaternion.identity);
				FloorNavParent.transform.SetParent(base.transform);
			}
			for (int j = 0; j < worldSizeZ; j++)
			{
				Object.Instantiate(position: new Vector3(i * gridOffset, 0f, j * gridOffset), original: FloorGameObj, rotation: Quaternion.identity).transform.SetParent(FloorNavParent.transform);
				Vector3 position3 = new Vector3(i * gridOffset, 1.2f, j * gridOffset);
				new Vector3(i * gridOffset, 1f, j * gridOffset);
				spawnItem(position3, Quaternion.identity);
			}
		}
	}

	public void GenerateWalls()
	{
		for (int i = 0; i < worldSizeX; i++)
		{
			if (i == 0)
			{
				SpawnWall(new Vector3(i, wallheight / 2f, -1f), Quaternion.identity, 1);
				SpawnWall(new Vector3(i, wallheight / 2f, worldSizeZ), Quaternion.identity);
			}
			SpawnWall(new Vector3(i, wallheight / 2f, -1f), Quaternion.identity);
			SpawnWall(new Vector3(i, wallheight / 2f, worldSizeZ), Quaternion.identity);
		}
		for (int j = 0; j < worldSizeZ; j++)
		{
			SpawnWall(new Vector3(-1f, wallheight / 2f, j), Quaternion.Euler(0f, 90f, 0f));
			SpawnWall(new Vector3(worldSizeX, wallheight / 2f, j), Quaternion.Euler(0f, 90f, 0f));
			Random.Range(0, worldSizeZ);
		}
	}

	private void SpawnWall(Vector3 position, Quaternion rotation, int x = 0)
	{
		if (x == 1)
		{
			WallParent = Object.Instantiate(WallGameObj, position, rotation);
		}
		GameObject obj = Object.Instantiate(WallGameObj, position, rotation);
		obj.transform.localScale = new Vector3(1f, wallheight, 1f);
		obj.transform.SetParent(WallParent.transform);
	}

	private void GenerateCeilling()
	{
		for (int i = 0; i < worldSizeX; i++)
		{
			if (i == 0)
			{
				float x = i * gridOffset;
				_ = gridOffset;
				Vector3 position = new Vector3(x, 0f, 0f);
				CeallingParent = Object.Instantiate(FloorGameObj, position, Quaternion.identity);
				CeallingParent.transform.SetParent(base.transform);
			}
			for (int j = 0; j < worldSizeZ; j++)
			{
				Object.Instantiate(position: new Vector3(i * gridOffset, wallheight, j * gridOffset), original: FloorGameObj, rotation: Quaternion.identity).transform.SetParent(CeallingParent.transform);
			}
		}
	}

	private void spawnItem(Vector3 position, Quaternion rotation)
	{
		int num = Random.Range(0, spawnRate);
		int index = Random.Range(0, Items.Count);
		switch (num)
		{
		case 1:
			Object.Instantiate(Items[index], position, rotation);
			break;
		case 2:
			Object.Instantiate(Items[index], position, rotation);
			break;
		case 3:
			Object.Instantiate(Items[index], position, rotation);
			break;
		}
	}

	private void combineMesh(GameObject parent)
	{
		MeshFilter[] componentsInChildren = parent.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] array = new CombineInstance[componentsInChildren.Length];
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			array[i].mesh = componentsInChildren[i].sharedMesh;
			array[i].transform = worldToLocalMatrix * componentsInChildren[i].transform.localToWorldMatrix;
			componentsInChildren[i].gameObject.SetActive(value: false);
		}
		GameObject gameObject = new GameObject("newFloor");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.indexFormat = IndexFormat.UInt32;
		meshFilter.mesh.CombineMeshes(array, mergeSubMeshes: true);
		gameObject.AddComponent<MeshRenderer>().sharedMaterial = componentsInChildren[0].GetComponent<MeshRenderer>().sharedMaterial;
		gameObject.AddComponent<MeshCollider>();
		meshFilter.mesh.RecalculateNormals();
		meshFilter.mesh.RecalculateBounds();
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		if (roomsBuilt == 1)
		{
			gameObject.transform.position = new Vector3(gameObject.transform.position.x + 30f, 0f, gameObject.transform.position.z + 30f);
		}
	}

	private void addNaveMesh()
	{
		for (int i = 0; i < surfaces.Length; i++)
		{
			surfaces[i].BuildNavMesh();
		}
		roomsBuilt++;
	}

	public void enemySpawn()
	{
		Vector3 position = new Vector3(Random.Range(1, worldSizeX - 1), 1f, Random.Range(1, worldSizeZ - 1));
		for (int i = 0; i < minEnemySpawn; i++)
		{
			Object.Instantiate(Enemy, position, Quaternion.identity);
			position = new Vector3(Random.Range(1, worldSizeX - 1), 1f, Random.Range(1, worldSizeZ - 1));
		}
	}

	private void SeperateRoomZ()
	{
		int num = Random.RandomRange(5, worldSizeZ);
		Object.Instantiate(position: new Vector3(Random.RandomRange(2, worldSizeX), 1f, Random.RandomRange(2, worldSizeZ)), original: sepWallGameObj, rotation: Quaternion.identity).transform.localScale = new Vector3(0.02f, wallheight, num);
	}

	private void SeperateRoomX()
	{
		int num = Random.RandomRange(5, worldSizeZ);
		Object.Instantiate(position: new Vector3(Random.RandomRange(2, worldSizeX), 1f, Random.RandomRange(2, worldSizeZ)), original: sepWallGameObj, rotation: Quaternion.identity).transform.localScale = new Vector3(num, wallheight, 0.02f);
	}

	private void spawnDivider()
	{
		dividerAmount = Random.Range(10, 25);
		Debug.Log(dividerAmount);
		for (int i = 0; i < dividerAmount; i++)
		{
			SeperateRoomX();
			SeperateRoomZ();
		}
	}

	private void Update()
	{
		deswall = GameObject.FindGameObjectsWithTag("DesObjWall").Length;
		if (deswall == 0)
		{
			spawnDivider();
			Debug.Log(deswall);
		}
	}

	private void EE()
	{
		Vector3 position = new Vector3(Random.Range(10, worldSizeX - 1), 1f, Random.Range(10, worldSizeZ - 1));
		for (int i = 0; i < 4; i++)
		{
			Object.Instantiate(EEbottle, position, Quaternion.identity);
			position = new Vector3(Random.Range(10, worldSizeX - 1), 1f, Random.Range(10, worldSizeZ - 1));
		}
	}
}
