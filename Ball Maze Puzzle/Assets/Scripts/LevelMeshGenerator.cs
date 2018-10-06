using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelMeshGenerator {

	const float obstacleFrontZ = -0.1f;
	const float obstacleBackZ = 0;



	public static GameObject CreateObstacleObject(Vector2[] obstacleCoords) {
		GameObject obstacle = new GameObject("Obstacle");
		MeshFilter meshFilter = obstacle.AddComponent<MeshFilter>();
		obstacle.AddComponent<MeshRenderer>();
		MeshCollider collider = obstacle.AddComponent<MeshCollider>();
		//Add once called from outside class
		//Put in parameter as well
		//obstacle.GetComponent<Renderer>().sharedMaterial = obstacleMaterial;

		Mesh mesh = meshFilter.mesh;
		collider.sharedMesh = mesh;

		Vector3[] newVertices;
		Vector2[] newUV;
		int[] newTriangles;

		//Vertices//

		//debug
		/*obstacleCoords = new Vector2[] {
				new Vector2(-1, 1),
				new Vector2(-0.7f, 1),
				new Vector2(0.7f, -0.7f),
				new Vector2(1, -0.7f),
				new Vector2(1, -1),
				new Vector2(-1, -1)
		};*/
		MorphObstacleToConvexShape(obstacleCoords);
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
		Vector3[] vertices = new Vector3[(points.Length * 6) - 4]; 

		int verticesIndex = 0;

		//Front face//

		for (int i = 0; i < points.Length; i++) {
			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);

			verticesIndex++;
		}

		//Middle faces//

		for (int i = 0; i < points.Length - 3; i+=2) {

			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);
			vertices[verticesIndex + 1] = new Vector3(points[i].x, points[i].y, obstacleBackZ);
			vertices[verticesIndex + 2] = new Vector3(points[i + 2].x, points[i + 2].y, obstacleBackZ);
			vertices[verticesIndex + 3] = new Vector3(points[i + 2].x, points[i + 2].y, obstacleFrontZ);
			verticesIndex += 4;
		}

		vertices[verticesIndex] = new Vector3(points[points.Length - 2].x, points[points.Length - 2].y, obstacleFrontZ);
		vertices[verticesIndex + 1] = new Vector3(points[points.Length - 2].x, points[points.Length - 2].y, obstacleBackZ);
		vertices[verticesIndex + 2] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleBackZ);
		vertices[verticesIndex + 3] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleFrontZ);
		verticesIndex += 4;

		for (int i = points.Length - 1; i > 2; i -= 2) {

			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);
			vertices[verticesIndex + 1] = new Vector3(points[i].x, points[i].y, obstacleBackZ);
			vertices[verticesIndex + 2] = new Vector3(points[i - 2].x, points[i - 2].y, obstacleBackZ);
			vertices[verticesIndex + 3] = new Vector3(points[i - 2].x, points[i - 2].y, obstacleFrontZ);
			verticesIndex += 4;
		}

		vertices[verticesIndex] = new Vector3(points[1].x, points[1].y, obstacleFrontZ);
		vertices[verticesIndex + 1] = new Vector3(points[1].x, points[1].y, obstacleBackZ);
		vertices[verticesIndex + 2] = new Vector3(points[0].x, points[0].y, obstacleBackZ);
		vertices[verticesIndex + 3] = new Vector3(points[0].x, points[0].y, obstacleFrontZ);

		return vertices;
	}



	static int[] GenerateTriangles(Vector2[] points) {
		int[] triangles = new int[(((points.Length / 2) - 1) * 6) + (points.Length * 6)];

		int currentVertex = 0;

		int vertexIndex = 0;

		//Front face//

		for (int i = 0; i < (((points.Length - 4) * 3) + 6); i+=6) {
			triangles[i] = currentVertex;
			triangles[i+1] = currentVertex+2;
			triangles[i+2] = currentVertex+3;

			triangles[i+3] = currentVertex+3;
			triangles[i+4] = currentVertex+1;
			triangles[i+5] = currentVertex;

			currentVertex += 2;
			vertexIndex += 6;
		}

		currentVertex += 2;

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



	static Vector2[] MorphObstacleToConvexShape(Vector2[] obstacleCoords) {
		Vector2[] oldPoints = obstacleCoords;
		Queue<Vector2> newPoints = new Queue<Vector2>();

		Vector2 previousPoint = new Vector2();
		Vector2 nextPoint = new Vector2();

		for (int i = 0; i < oldPoints.Length; i++) {
			if(i != oldPoints.Length - 1) {
				nextPoint = oldPoints[i + 1];
			} else {
				nextPoint = oldPoints[0];
			}

			if(previousPoint != null) {
				if (Vector3.Cross((oldPoints[i] - previousPoint).normalized, (nextPoint - oldPoints[i]).normalized).z < 0) {
					//Is right turn or straight
					newPoints.Enqueue(oldPoints[i]);
					previousPoint = oldPoints[i];
				}
			} else {
				newPoints.Enqueue(oldPoints[i]);
				previousPoint = oldPoints[i];
			}
		}

		return newPoints.ToArray();
	}



	static Vector2[] GenerateUVs(Vector2[] points) {
		Vector2[] uvs = new Vector2[points.Length * 6];

		/*
		 * Add UV generation code 
		 */

		return uvs;
	}
}
