using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelMeshGenerator {

	const float obstacleFrontZ = -0.5f;
	const float obstacleBackZ = 0;



	public static GameObject CreateObstacleObject() {
		GameObject obstacle = new GameObject("Obstacle");
		MeshFilter meshFilter = obstacle.AddComponent<MeshFilter>();
		obstacle.AddComponent<MeshRenderer>();
		//Add once called from outside class
		//Put in parameter as well
		//obstacle.GetComponent<Renderer>().sharedMaterial = obstacleMaterial;

		Mesh mesh = meshFilter.mesh;

		Vector3[] newVertices;
		Vector2[] newUV;
		int[] newTriangles;

		//Vertices//

		//debug
		Vector2[] obstacleCoords = new Vector2[] {
				new Vector2(-1, 1),
				new Vector2(-0.7f, 1),
				new Vector2(0.7f, -0.7f),
				new Vector2(1, -0.7f),
				new Vector2(1, -1),
				new Vector2(-1, -1)
		};

		newVertices = GenerateVertices(obstacleCoords);
		newTriangles = GenerateTriangles(obstacleCoords);
		newUV = GenerateUVs(obstacleCoords);

		mesh.Clear();

		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		//mesh.uv = newUV;
		mesh.RecalculateNormals();

		return obstacle;
	}



	static Vector3[] GenerateVertices(Vector2[] points) {
		Vector3[] vertices = new Vector3[points.Length * 12]; 

		int verticesIndex = 0;

		//Front face//

		for (int i = 0; i < points.Length; i++) {
			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);

			verticesIndex++;
		}

		//Back face//
		int j = 1;
		do {
			vertices[verticesIndex] = new Vector3(points[j].x, points[j].y, obstacleBackZ);

			verticesIndex++;
			if (j == 0) j = points.Length;
			j--;
		} while (j != 1);

		//Middle faces//

		for (int i = 0; i < points.Length - 1; i++) {

			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);
			vertices[verticesIndex + 1] = new Vector3(points[i].x, points[i].y, obstacleBackZ);
			vertices[verticesIndex + 2] = new Vector3(points[i + 1].x, points[i + 1].y, obstacleBackZ);
			vertices[verticesIndex + 3] = new Vector3(points[i + 1].x, points[i + 1].y, obstacleFrontZ);

			verticesIndex += 4;
		}

		vertices[verticesIndex] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleFrontZ);
		vertices[verticesIndex + 1] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleBackZ);
		vertices[verticesIndex + 2] = new Vector3(points[0].x, points[0].y, obstacleBackZ);
		vertices[verticesIndex + 3] = new Vector3(points[0].x, points[0].y, obstacleFrontZ);

		return vertices;
	}



	static int[] GenerateTriangles(Vector2[] points) {
		int[] triangles = new int[(points.Length - 1) * 12];

		int currentVertex = 0;

		int vertexIndex = 0;

		//Front face//

		for (int i = 0; i < points.Length - 2; i++) {
			if (i < 2) {
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex + 1;
				triangles[vertexIndex + 2] = currentVertex + 2;
				currentVertex++;
			} else if (i != points.Length - 3){
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex + 1;
				triangles[vertexIndex + 2] = currentVertex - i;
			} else {
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex - points.Length + 1;
				triangles[vertexIndex + 2] = currentVertex - i;
			}
			currentVertex++;
			vertexIndex += 3;
		}

		//Back face//

		for (int i = 0; i < points.Length - 2; i++) {
			if (i < 2) {
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex + 1;
				triangles[vertexIndex + 2] = currentVertex + 2;
				currentVertex++;
			} else if (i != points.Length - 3) {
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex + 1;
				triangles[vertexIndex + 2] = currentVertex - i;
			} else {
				triangles[vertexIndex] = currentVertex;
				triangles[vertexIndex + 1] = currentVertex - points.Length + 1;
				triangles[vertexIndex + 2] = currentVertex - i;
			}
			currentVertex++;
			vertexIndex += 3;
		}

		//Side faces//

		for (int i = 0; i < points.Length; i++) {
			for (int j = 0; j < 3; j++) {
				triangles[vertexIndex + j] = currentVertex;
				currentVertex++;
			}
			vertexIndex += 3;

			triangles[vertexIndex] = currentVertex - 1;
			triangles[vertexIndex + 1] = currentVertex;
			triangles[vertexIndex + 2] = currentVertex - 3;
			currentVertex++;
			vertexIndex += 3;
		}

		return triangles;
	}



	static Vector2[] GenerateUVs(Vector2[] points) {
		Vector2[] uvs = new Vector2[points.Length * 6];

		/*
		 * Add UV generation code 
		 */

		return uvs;
	}
}
